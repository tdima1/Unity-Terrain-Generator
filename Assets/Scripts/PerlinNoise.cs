﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
   public static float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight, float scale, int seed,
      int octaves, float persistance, float lacunarity, Vector2 offset)
   {
      float[,] perlinNoiseMap = new float[mapWidth, mapHeight];

      System.Random prng = new System.Random(seed);
      Vector2[] octaveOffsets = new Vector2[octaves];
      for (int i = 0; i < octaves; i++) {
         float offsetX = prng.Next(-100000, 100000) + offset.x;
         float offsetY = prng.Next(-100000, 100000) + offset.y;
         octaveOffsets[i] = new Vector2(offsetX, offsetY);
      }
      
      if (scale <= 0) {
         scale = 0.0001f;
      }

      float maxNoiseHeight = float.MinValue;
      float minNoiseHeight = float.MaxValue;
      float halfWidth = mapWidth / 2f;
      float halfHeight = mapHeight / 2f;

      for (int y = 0; y < mapHeight; y++) {
         for (int x = 0; x < mapWidth; x++) {
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;

            for (int o = 0; o < octaves; o++) {
               float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[o].x;
               float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[o].y;

               float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
               noiseHeight += perlinValue * amplitude;

               amplitude *= persistance;
               frequency *= lacunarity;
            }

            if (noiseHeight > maxNoiseHeight) {
               maxNoiseHeight = noiseHeight;
            }
            if (noiseHeight < minNoiseHeight) {
               minNoiseHeight = noiseHeight;
            }

            perlinNoiseMap[x, y] = noiseHeight;
         }
      }

      for (int y = 0; y < mapHeight; y++) {
         for (int x = 0; x < mapWidth; x++) {
            perlinNoiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, perlinNoiseMap[x, y]);
         }
      }

      return perlinNoiseMap;
   }
}
