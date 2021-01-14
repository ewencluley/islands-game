using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LandGeneration.FeatureDetector
{
    public class FeatureDetectionPipeline : MonoBehaviour
    {
        private readonly List<IFeatureDetector> _detectors = new List<IFeatureDetector>();
        private Terrain _terrain;
        private Island _island;

        public void Init(Island island, Terrain terrain)
        {
            _island = island;
            _terrain = terrain;
            _detectors.Add(new ShorelineFeatureDetector(island, terrain));
        }

        public void DetectFeatures()
        {
            for (int y = 0; y < _island.HeightMapResolution; y++)
            {
                for (int x = 0; x < _island.HeightMapResolution; x++)
                {
                    foreach (var detector in _detectors)
                    {
                        detector.PreProcess(x, y);
                    }
                }
            }

            foreach (var detector in _detectors)
            {
                detector.Detect();
            }
        }

        private void OnDrawGizmos()
        {
            // if (_island?.shoreline == null)
            // {
            //     return;
            // }
            //
            // Gizmos.color = Color.white;
            // foreach (var p in _island.shoreline)
            // {
            //     Gizmos.DrawSphere(p, 0.5f);
            // }
            
            
            
            if (_island?.ShorelinePaths == null)
            {
                return;
            }

            var terrainDataSize = _terrain.terrainData.size;
            var offset = new Vector3(_island.HeightMapResolution / terrainDataSize.x / 2f, 0, _island.HeightMapResolution / terrainDataSize.z / 2f);
            Gizmos.color = Color.red;
            foreach (var shore in _island.ShorelinePaths)
            {
                for (int i = 0; i < shore.Count; i++)
                {
                    //Gizmos.color = i == 0 || i == shore.Count -1 ? Color.cyan : Color.red;
                    var next = (i == shore.Count - 1) ? 0 : i + 1;
                    //Gizmos.DrawSphere(shore[i], 0.3f);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(shore[i] + offset, shore[next] + offset);
                    Handles.Label(shore[i] + offset, $"{i}");
                }
            }
            
        }
    }
}