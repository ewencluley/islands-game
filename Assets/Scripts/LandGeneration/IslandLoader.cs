using System;
using System.Collections.Generic;
using LandGeneration.Augmenters;
using LandGeneration.FeatureDetector;
using UnityEngine;

namespace LandGeneration
{
    public class IslandLoader : MonoBehaviour
    {
        public bool loaded { get; private set; } = false;

        public bool debug;
        private MeshRenderer _debugMesh;
        private Terrain _terrain;
        private TerrainCollider _terrainCollider;
        private Island _island;

        private List<IIslandDecorator> _decorators = new List<IIslandDecorator>();
        private FeatureDetectionPipeline _featureDetectionPipeline;

        private void Awake()
        {
            _debugMesh = GetComponentInChildren<MeshRenderer>();
            if (!debug)
            {
                _debugMesh.gameObject.SetActive(false);
            }
            _terrain = GetComponent<Terrain>();
            _terrainCollider = GetComponent<TerrainCollider>();
            _featureDetectionPipeline = GetComponent<FeatureDetectionPipeline>();
            _decorators.Add(GetComponent<TreeDecorator>());
        }

        public void Load(Island island)
        {
            Debug.Log($"Loading island generated from seed {island.Seed}");
            _island = island;
            var heightmapResolution = island.HeightMapResolution;
            var terrainData = new TerrainData
            {
                heightmapResolution = heightmapResolution,
                size = island.TerrainSize,
                treePrototypes = _terrain.terrainData.treePrototypes
            };

            terrainData.SetHeights(0, 0, island.Heights.UnFlatten(heightmapResolution));
            _terrain.terrainData = terrainData;
            _terrainCollider.terrainData = terrainData;
            transform.position = island.Position.ToVector3();
            
            DrawDebugMesh(island);
            _featureDetectionPipeline.Init(island, _terrain);
            _featureDetectionPipeline.DetectFeatures();
            foreach (var decorator in _decorators)
            {
                decorator.Decorate(island, _terrain);
            }
            loaded = true;
        }

        private void DrawDebugMesh(Island island)
        {
            if (!debug)
            {
                return;
            }
            
            Color[] pixels = new Color[island.Heights.Length];
            for (int i = 0; i < island.Heights.Length; i++)
            {
                var h = island.Heights[i];
                pixels[i] = new Color(h,h,h);
            }
            
            Texture2D texture2D = new Texture2D(island.HeightMapResolution, island.HeightMapResolution);
            texture2D.SetPixels(pixels);
            texture2D.Apply();
            _debugMesh.material.SetTexture("_BaseMap", texture2D);
        }

        private void OnDrawGizmos()
        {
            if (_island == null)
            {
                return;
            }
        }
    }
}