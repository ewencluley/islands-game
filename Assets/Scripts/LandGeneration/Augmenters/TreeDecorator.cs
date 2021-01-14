using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace LandGeneration.Augmenters
{
    class TreeDecorator : JobManager<TreeDecorator.TreeDecoratorJob, Island, Vector3[]>, IIslandDecorator
    {
        public int maxTrees = 1000;
        [Range(0,1)] public float treeProbability = 0.2f;
        private Island _island;
        private Terrain _terrain;

        protected override TreeDecoratorJob CreateJob(Island island)
        {
            return new TreeDecoratorJob()
            {
                IslandPosition = island.Position,
                TerrainSize = island.TerrainSize,
                HeightmapResolution = island.HeightMapResolution,
                SeaLevel = Sea.Instance.GetSeaLevel(),
                HeightData = new NativeArray<float>(island.Heights, Allocator.Persistent),
                TreeData = new NativeArray<Vector3>(new Vector3[maxTrees], Allocator.Persistent),
                TreeProbability = treeProbability
            };
        }

        protected override Vector3[] GetResultFromJob(TreeDecorator.TreeDecoratorJob job)
        {
            var trees = job.TreeData.ToArray();
            job.TreeData.Dispose();
            job.HeightData.Dispose();
            return trees;
        }
        
        public struct TreeDecoratorJob : IJob
        {
            public Vector2 IslandPosition;
            public Vector3 TerrainSize;
            public float SeaLevel;
            public int HeightmapResolution;
            public float TreeProbability;
            public NativeArray<float> HeightData;
            public NativeArray<Vector3> TreeData;
        
            public void Execute()
            {
                var terrainSpaceSeaLevel = SeaLevel / TerrainSize.y;
                int treeIndex = 0;
                
                for (int i = 0; i < HeightData.Length; i++)
                {
                    int x = i % HeightmapResolution;
                    int y = i / HeightmapResolution;
                    var prng = new System.Random();
                    var random = prng.Next(0, 100) / 100f;
                    if (random > TreeProbability)
                    {
                        continue;
                    };
                    Vector3 proposedTreePosition = new Vector3(x / (float)HeightmapResolution, HeightData[i], y / (float)HeightmapResolution);
                    if (proposedTreePosition.y > terrainSpaceSeaLevel)
                    {
                        TreeData[treeIndex] = proposedTreePosition;
                        treeIndex++;
                    }
                    if (treeIndex >= TreeData.Length)
                    {
                        return;
                    }
                }
            }
        }

        public void Decorate(Island island, Terrain terrain)
        {
            if (_terrain == null)
            {
                _terrain = terrain;
            }

            if (_island == null)
            {
                _island = island;
            }
            Debug.Log($"terrain heightmap res = {terrain.terrainData.heightmapResolution}");
            ScheduleJob(island, (i, treePositions) =>
            {
                Debug.Log($"Instantiating trees on island {i.Position}");
                i.trees = treePositions;
                var treeInstances = treePositions
                    .Where(p => p != Vector3.zero)
                    .Select(p => new TreeInstance()
                {
                    position = p,
                    color = Color.green,
                    prototypeIndex = 0,
                    heightScale = 10,
                    widthScale = 10
                }).ToArray();
                Debug.Log($"Instantiating {treeInstances.Length} trees");
                //terrain.terrainData.SetTreeInstances(treeInstances, false);
            });
        }
    }
}