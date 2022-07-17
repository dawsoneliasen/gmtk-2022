using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour {
    
    public bool generateFlag;
    public GameObject player;
    public GameObject die;
    public GameObject exit;
    public GameObject walkingEnemy;
    public GameObject flyingEnemy;
    public int levelWidth;
    public int levelHeight;
    public float smoothness;
    public float abyssSmoothness;
    public float fractureSmoothness;
    public float tunnelSmoothness;
    public RuleTile grass;
    public RuleTile dirt;
    public RuleTile stone;
    public Tilemap tilemap;
    public RuleTile chain;
    private float seed;
    private float stoneBoundarySeed;
    private float abyssSeed;
    private float timeSinceSpawn;
    private GameObject playerInstance;
    private GameObject exitInstance;
    private int currentLevel;

    void Start() {
        playerInstance = GameObject.Instantiate(player);
        exitInstance = GameObject.Instantiate(exit);
        generateFlag = true;
        currentLevel = 0;
    }

    void Update() {
        if (generateFlag) {
            GenerateLevel();
            generateFlag = false;
        }
        int numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        timeSinceSpawn += Time.deltaTime;
        if (numEnemies < 50) {
            if (timeSinceSpawn > 5) {
                GameObject enemy;
                if (Random.Range(0.0f, 1.0f) > 0.5f) {
                    enemy = walkingEnemy;
                } else {
                    enemy = flyingEnemy;
                }
                SpawnEnemy(enemy);
                timeSinceSpawn = 0;
                numEnemies++;
            }
        }
    }

    void SpawnEnemy(GameObject enemy) {
        Vector3 enemyPos = LevelGenerationHelper.RandomSafePosition(levelWidth, levelHeight, tilemap);
        GameObject enemyInstance = GameObject.Instantiate(enemy);
        enemyInstance.transform.position = enemyPos;
    }

    void GenerateDice() {
        int numDice = Random.Range(2, 5);
        for (int i = 0; i < numDice; i++) {
            int dieLevel = Random.Range(1, 11);
            int dieElementIndex = Random.Range(-1, 5);
            Spell spell = SpellMaker.MakeSpellLevel(dieLevel, dieElementIndex);
            GameObject dieInstance = GameObject.Instantiate(die);
            Vector3 diePos = LevelGenerationHelper.RandomSafePosition(levelWidth, levelHeight, tilemap);
            dieInstance.transform.position = diePos;
            dieInstance.GetComponent<Die>().spell = spell;
        }
    }

    public void GenerateLevel() {
        currentLevel++;
        Camera.main.transform.position = new Vector3(levelWidth / 2, levelHeight / 2, 0);
        tilemap.ClearAllTiles();
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block")) {
            Destroy(block);
        }
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }
        foreach (GameObject dieOnGround in GameObject.FindGameObjectsWithTag("Die")) {
            Destroy(dieOnGround);
        }
        int currentElevation = 333;
        int currentAbyssCeiling = 100;
        int currentStoneBoundary = 275;
        for (int x = 0; x <= levelWidth; x++) {
            currentElevation += LevelGenerationHelper.PerlinStep(x, smoothness, seed);
            currentAbyssCeiling += LevelGenerationHelper.PerlinStep(x, abyssSmoothness, abyssSeed);
            currentStoneBoundary += LevelGenerationHelper.PerlinStep(x, smoothness / 5, stoneBoundarySeed);
            int yFloor = Mathf.Max(currentAbyssCeiling, 0);
            for (int y = yFloor; y <= currentElevation; y++) {
                RuleTile useTile;
                if (y == currentElevation) {
                    useTile = grass;
                } else if (y > currentStoneBoundary) {
                    useTile = dirt;
                } else {
                    useTile = stone;
                }
                tilemap.SetTile(new Vector3Int(x, y, 0), useTile);
            }
        }
        seed = Random.Range(-9999999, 9999999);
        stoneBoundarySeed = Random.Range(-9999999, 9999999);
        abyssSeed = Random.Range(-9999999, 9999999);
        // GenerateFloatingStructures(2, levelWidth / 2, 50, 100, true); 
        GenerateFloatingStructures(2 * (currentLevel - 1), levelWidth / 10, 5, 10, false);
        // GenerateFloatingStructures(50, levelWidth / 50, 5, 10, false);
        GenerateDice();
        // GenerateFractures();
        // GenerateTunnels();
        GenerateExit();
        PlacePlayerInstance();
    }

    void PlacePlayerInstance() {
        int playerSpawnX = levelWidth / 2;
        int y;
        for (y = levelHeight; !tilemap.HasTile(new Vector3Int(playerSpawnX, y, 0)); y--);
        Debug.Log(playerSpawnX);
        Debug.Log(y);
        playerInstance.transform.position = new Vector3(playerSpawnX, y + 6, 0);
        Camera.main.transform.GetComponent<CameraController>().follow = playerInstance;
    }

    void GenerateFractures() {
        int numFractures = 8;
        for (int i = 0; i <= numFractures; i++) {
            int fractureBranchStartX = Random.Range(0, levelWidth);
            GenerateFractureBranch(fractureBranchStartX, 0, 0);
        }
    }

    void GenerateFractureBranch(int startX, int startY, int trend) {
        int fractureBranchSeed = Random.Range(-9999999, 9999999);
        float newNodeProbability = 0.02f;
        int currentFractureX = startX;
        bool fractureStarted = false;
        for (int y = startY; y <= levelHeight; y++) {
            if (!fractureStarted) {
                if (tilemap.HasTile(new Vector3Int(currentFractureX, y, 0))) {
                    fractureStarted = true;
                } else {
                    continue;
                }
            }
            int xStep = LevelGenerationHelper.PerlinStep(y, 5, fractureBranchSeed, trend == 0);
            if (trend != 0) {
                xStep *= trend;
            }
            currentFractureX += xStep;
            if (!tilemap.HasTile(new Vector3Int(currentFractureX, y, 0))) {
                break;
            }
            int fractureRadius = 10;
            for (int deleteTileAtX = currentFractureX - fractureRadius; deleteTileAtX <= currentFractureX + fractureRadius; deleteTileAtX++) {
                tilemap.SetTile(new Vector3Int(deleteTileAtX, y, 0), null);
            }
            if (Random.Range(0.0f, 1.0f) < newNodeProbability) {
                GenerateFractureBranch(currentFractureX, y, -1);
                GenerateFractureBranch(currentFractureX, y, 1);
                return;
            }
        }
    }

    void GenerateFloatingStructures(int numFloatingStructures, int structureRadius, int structureLiftAmountMin, int structureLiftAmountMax, bool generateChain) {
        for (int i = 0; i < numFloatingStructures; i++) {
            float structureSeed = Random.Range(-9999999, 9999999);
            int structureStartX = Random.Range(0, levelWidth - (structureRadius * 2));
            int structureEndX = structureStartX + (structureRadius * 2);
            int structureLiftAmount = Random.Range(structureLiftAmountMin, structureLiftAmountMax);
            int initialStructureFractureY = levelHeight;
            for (int y = levelHeight; y >= 0; y--) {
                if (tilemap.HasTile(new Vector3Int(structureStartX, y, 0))) {
                    initialStructureFractureY = y;
                    break;
                }
            }
            int currentStructureFractureY = initialStructureFractureY;
            for (int x = structureStartX; x <= structureEndX; x++) {
                for (int y = levelHeight; y >= currentStructureFractureY; y--) {
                    // TODO: fragments suspened in fracture
                    if (tilemap.HasTile(new Vector3Int(x, y, 0))) {
                        RuleTile useTile = (RuleTile) tilemap.GetTile(new Vector3Int(x, y, 0));
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        tilemap.SetTile(new Vector3Int(x, y + structureLiftAmount, 0), useTile);
                    }
                }
                int yStep = LevelGenerationHelper.PerlinStep(x, fractureSmoothness, structureSeed, false);
                if ((x - structureStartX) >= structureRadius) {
                    yStep *= -1;
                }
                // yStep *= structureFractureSharpness;
                currentStructureFractureY += yStep;
            }
            if (generateChain) {
                int chainX = structureStartX;
                int chainY = initialStructureFractureY + structureLiftAmount;
                GenerateChain(chainX, chainY);
                chainX = structureEndX;
                chainY = currentStructureFractureY + structureLiftAmount;
                GenerateChain(chainX, chainY);
            }
        }
    }

    void GenerateChain(int x, int chainStartY) {
        int y;
        for (y = chainStartY; !tilemap.HasTile(new Vector3Int(x, y, 0)); y--) {
            if (y <= 0) {
                break;
            }
        }
        int chainEndY = y + 4;
        for (y = chainStartY; y >= chainEndY; y--) {
            tilemap.SetTile(new Vector3Int(x, y, 0), chain);
        }
    }

    void GenerateTunnels() {
        int numTunnels = Random.Range(1, 4);
        for (int i = 0; i <= numTunnels; i++) {
            float tunnelSeed = Random.Range(-9999999, 9999999);
            int currentTunnelX = Random.Range(0, levelWidth);
            for (int y = 0; y <= levelHeight; y++) {
                currentTunnelX += LevelGenerationHelper.PerlinStep(y, tunnelSmoothness, tunnelSeed);
                int currentTunnelRadius = Random.Range(10, 14);
                for(int x = currentTunnelX - currentTunnelRadius; x <= currentTunnelX + currentTunnelRadius; x++) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    void GenerateExit() {
        Vector3 exitPos = LevelGenerationHelper.RandomSafePosition(levelWidth, levelHeight, tilemap);
        exitPos.y -= 6;
        exitInstance.transform.position = exitPos;
    }

}
