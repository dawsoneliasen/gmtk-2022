using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour {
    
    public GameObject player;
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
    private int numWalkingEnemies;
    private int numFlyingEnemies;
    private float timeSinceSpawn;

    void Start() {
        seed = Random.Range(-9999999, 9999999);
        stoneBoundarySeed = Random.Range(-9999999, 9999999);
        abyssSeed = Random.Range(-9999999, 9999999);
        GenerateChaosLevel();
        // GenerateFloatingStructures(2, levelWidth / 2, 50, 100, true); 
        // GenerateFloatingStructures(5, levelWidth / 10, 5, 10, false);
        // GenerateFloatingStructures(50, levelWidth / 50, 5, 10, false);
        GeneratePlayerSpawn();
        // GenerateFractures();
        // GenerateTunnels();
        numWalkingEnemies = 0;
        numFlyingEnemies = 0;
    }

    void Update() {
        timeSinceSpawn += Time.deltaTime;
        if (numWalkingEnemies < 50) {
            if (timeSinceSpawn > 5) {
                SpawnEnemy(walkingEnemy);
                timeSinceSpawn = 0;
                numWalkingEnemies++;
            }
        }
        if (numFlyingEnemies < 50) {
            if (timeSinceSpawn > 5) {
                SpawnEnemy(flyingEnemy);
                timeSinceSpawn = 0;
                numFlyingEnemies++;
            }
        }
    }

    void SpawnEnemy(GameObject enemy) {
        int x = Random.Range(0, levelWidth);
        int y;
        for (y = levelHeight; !tilemap.HasTile(new Vector3Int(x, y, 0)); y--);
        GameObject enemyInstance = GameObject.Instantiate(enemy);
        enemyInstance.transform.position = new Vector3(x, y + 6, 0);
    }

    int PerlinStep(int x, float smoothness, float seed, bool untrend = true) {
        float noiseScale = 10.01f;
        float noise = Mathf.PerlinNoise(x * noiseScale, seed);
        float normalizedNoise = (noise - 0.5f) * 100;
        int elevationChange = Mathf.RoundToInt(normalizedNoise / smoothness);
        if (untrend) {
            if (Random.Range(0.0f, 1.0f) < 0.5) {
                elevationChange *= -1;
            }
        }
        return elevationChange;
    }

    // void GenerateLevel() {
    //     // TODO: start elevation at random y close to levelHeight / 2
    //     int currentElevation = levelHeight / 2;
    //     for (int x = 0; x <= levelWidth; x++) {
    //         currentElevation += PerlinStep(x, smoothness, seed);
    //         for (int y = 0; y <= currentElevation; y++) {
    //             tilemap.SetTile(new Vector3Int(x, y, 0), ground);
    //         }
    //     }
    // }

    void GenerateChaosLevel() {

        // int numLargeFloatingScructures = Random.Range(0, 4);
        // int numSmallFloatingStructures = Random.Range(2, 13);  // also have lots of tiny floating structures
        // int numChasms = Random.Range(1, 5);
        // int numTunnels = Random.Range(2, 7);
        // int numPits = Random.Range(1, 4);
        // int numWatchtowers = Random.Range(0, 4);

        // int currentElevation = (int) ((float) levelHeight / 1.5f);  // TODO: add noise to this starting point
        // int currentAbyssCeiling = levelHeight / 5;
        int currentElevation = 333;
        int currentAbyssCeiling = 100;
        int currentStoneBoundary = 275;
        for (int x = 0; x <= levelWidth; x++) {
            currentElevation += PerlinStep(x, smoothness, seed);
            currentAbyssCeiling += PerlinStep(x, abyssSmoothness, abyssSeed);
            currentStoneBoundary += PerlinStep(x, smoothness / 5, stoneBoundarySeed);
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
    }

    void GeneratePlayerSpawn() {
        int playerSpawnX = levelWidth / 2;
        int y;
        for (y = levelHeight; !tilemap.HasTile(new Vector3Int(playerSpawnX, y, 0)); y--);
        GameObject playerInstance = GameObject.Instantiate(player);
        playerInstance.transform.position = new Vector3(playerSpawnX, y + 6, 0);
        Camera.main.transform.position = playerInstance.transform.position;
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
            int xStep = PerlinStep(y, 5, fractureBranchSeed, trend == 0);
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
                int yStep = PerlinStep(x, fractureSmoothness, structureSeed, false);
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
                currentTunnelX += PerlinStep(y, tunnelSmoothness, tunnelSeed);
                int currentTunnelRadius = Random.Range(10, 14);
                for(int x = currentTunnelX - currentTunnelRadius; x <= currentTunnelX + currentTunnelRadius; x++) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

}
