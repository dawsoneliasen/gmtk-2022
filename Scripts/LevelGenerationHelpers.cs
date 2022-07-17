using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class LevelGenerationHelper {

    public static int PerlinStep(int x, float smoothness, float seed, bool untrend = true) {
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

    public static Vector3 RandomSafePosition(int levelWidth, int levelHeight, Tilemap tilemap) {
        int x = Random.Range(0, levelWidth);
        int y;
        for (y = levelHeight; !tilemap.HasTile(new Vector3Int(x, y, 0)); y--);
        return new Vector3(x, y + 12, 0);
    }
}
