using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace LandGeneration.Providers
{
    /**
     * Manages Jobs for island generation
     */
    public class SampledIslandProvider : JobManager<IslandProviderJob, Vector2, Island>
    {
        
        public int octaves;
        public float scale = 20f;
        [Range(0, 1)] public float persistence;
        [Range(0, 10)] public float lacunarity;
        public int heightmapResolution = 1025;
        public int seed = 0;
        public bool useRandomSeed = false;

        protected override IslandProviderJob CreateJob(Vector2 position)
        {
            return new IslandProviderJob()
            {
                HeightData = new NativeArray<float>(heightmapResolution * heightmapResolution, Allocator.Persistent),
                HeightmapResolution = heightmapResolution,
                Octaves = octaves,
                Lacunarity = lacunarity,
                Persistence = persistence,
                Position = position,
                Scale = scale,
                Seed = useRandomSeed ? Random.Range(-100_000, 100_000) : seed
            };
        }

        protected override Island GetResultFromJob(IslandProviderJob job)
        {
            float[] heights = job.HeightData.ToArray();
            job.HeightData.Dispose();
            return new Island(heights, job.HeightmapResolution, new Vector3(1000, 200, 1000), job.Position, job.Seed);
        }
    }
    
    public struct IslandProviderJob : IJob
    {
        public int HeightmapResolution;
        public Vector2 Position;
        public int Seed;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;
        public float Scale;
        
        public NativeArray<float> HeightData;
        
        public void Execute()
        {
            float[] heights = Noise.GenerateNoiseMap(HeightmapResolution, HeightmapResolution, Seed, Scale, Octaves,
                Persistence, Lacunarity, Vector2.zero);
            heights = ApplyRadialMask(heights);

            for (int i = 0; i < heights.Length; i++)
            {
                HeightData[i] = heights[i];
            }
            Debug.Log("Completed job");
        }
        
        private float[] ApplyRadialMask(float[] map)
        {
            var output = new float[map.Length];
            for (int i = 0; i < map.Length; i++)
            {
                int x = i % HeightmapResolution;
                int y = i / HeightmapResolution;
                output[i] = map[i] * Gradients.RadialGradient(x, y, HeightmapResolution, HeightmapResolution);
            }
            return output;
        }
    }
}