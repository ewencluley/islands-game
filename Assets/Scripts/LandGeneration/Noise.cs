using UnityEngine;

namespace LandGeneration
{
    public static class Noise
    {
        public static float[] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
            float[] noiseMap = new float[mapWidth*mapHeight];

            System.Random prng = new System.Random (seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++) {
                float offsetX = prng.Next (-100000, 100000) + offset.x;
                float offsetY = prng.Next (-100000, 100000) + offset.y;
                octaveOffsets [i] = new Vector2 (offsetX, offsetY);
            }

            if (scale <= 0) {
                scale = 0.0001f;
            }

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            float minNoiseHeight = float.MaxValue;
            float maxNoiseHeight = float.MinValue;

            for (int i = 0; i < mapHeight * mapWidth; i++)
            {
                int x = i % mapWidth;
                int y = Mathf.FloorToInt(i / mapHeight);
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int o = 0; o < octaves; o++) {
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[o].x;
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[o].y;

                    float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);
                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                noiseMap [i] = noiseHeight;
            }

            return Normalize(noiseMap, minNoiseHeight, maxNoiseHeight);
        }

        private static float[] Normalize(float[] input, float min, float max)
        {
            for (int i = 0; i < input.Length; i++) {
                input [i] = Mathf.InverseLerp (min, max, input [i]);
            }
            return input;
        } 
        
        public static float[] Normalize(float[] input)
        {
            float minNoiseHeight = float.MaxValue;
            float maxNoiseHeight = float.MinValue;
            foreach (var h in input)
            {
                minNoiseHeight = Mathf.Min(minNoiseHeight, h);
                maxNoiseHeight = Mathf.Max(maxNoiseHeight, h);
            }

            return Normalize(input, minNoiseHeight, maxNoiseHeight);
        }
    }
}