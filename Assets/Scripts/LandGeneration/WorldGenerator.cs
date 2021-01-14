using System;
using System.Collections.Generic;
using System.Linq;
using LandGeneration.Providers;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace LandGeneration
{
    
    [Serializable]
    public class WorldLoadedEvent: UnityEvent { }
    
    public class WorldGenerator : MonoBehaviour
    {
        public int islandCount = 10;
        public float jitter = 1.5f;
        public int seed;
        public int islandSeed;
        public bool useRandomIslandSeed = false;
        [Range(0,100)] public float spread = 1;
        public IslandLoader islandPrefab;
        public Transform world;
        private SampledIslandProvider _islandProvider;
        private readonly Dictionary<Vector2, IslandLoader> _islandsBeingGenerated = new Dictionary<Vector2, IslandLoader>();

        public WorldLoadedEvent worldLoadedEvent = new WorldLoadedEvent();
        private bool _worldLoadedEventTriggered = false;
        private float _start;

        void Start()
        {
            _start = Time.realtimeSinceStartup;
            _islandProvider = GetComponent<SampledIslandProvider>();

            foreach (var position in GenerateIslandPositions())
            {
                var islandGenSeed = useRandomIslandSeed ? Random.Range(-10000, 10000) : islandSeed;
                _islandProvider.ScheduleJob(position, (islandPos, island) =>
                {
                    IslandLoader islandLoader = _islandsBeingGenerated[position];
                    islandLoader.Load(island);
                    Debug.Log("Island rendered");
                    _islandsBeingGenerated.Remove(position);
                });
                _islandsBeingGenerated.Add(position, Instantiate(islandPrefab, world));
            }
        }

        List<Vector2> GenerateIslandPositions()
        {
            Random.InitState(seed);
            List<Vector2> placedIslands = new List<Vector2>();
            var i = 0;
            while (placedIslands.Count < islandCount)
            {
                var position = GetSpiralLocation(i);
                i++;
                if (OverlapsAnotherPlacedIsland(position, placedIslands))
                {
                    continue;
                }
                placedIslands.Add(position);
            }

            return placedIslands;
        }

        private bool OverlapsAnotherPlacedIsland(Vector2 position, List<Vector2> placed)
        {
            return placed
                 .Any(i => Vector2.Distance(i, position) < 1000f); //TODO make this use the island radius
        }
    
        private Vector2 GetSpiralLocation(float step)
        {
            float circleGrowSpeed = step * 10;
            float circleSize = step * circleGrowSpeed;

            float randomXShift = Random.Range(0, jitter * step);
            float randomYShift = Random.Range(0, jitter * step);

            float xPos = Mathf.Sin(step * spread) * (circleSize + randomXShift);
            float yPos = Mathf.Cos(step * spread) * (circleSize + randomYShift);

            return new Vector2(xPos, yPos);
        }

        private void Update()
        {
            if (_islandsBeingGenerated.Count == 0)
            {
                if (!_worldLoadedEventTriggered)
                {
                    Debug.Log($"World generated and instantiated in {Time.realtimeSinceStartup - _start} seconds");
                    worldLoadedEvent.Invoke();
                    _worldLoadedEventTriggered = true;
                }
                return;
            }
        }

        // private Vector2? RenderNextLoadedIsland()
        // {
        //     foreach (var position in _islandsBeingGenerated)
        //     {
        //         var loaded = _islandProvider.GetResultIfAvailable(position);
        //         if (loaded != null)
        //         {
        //             IslandLoader islandLoader = Instantiate(islandPrefab, world);
        //             islandLoader.Load(loaded);
        //             Debug.Log("Island rendered");
        //             _islandsBeingGenerated.Remove(position);
        //             return position;
        //         }
        //     }
        //
        //     return null;
        // }

        private void OnDrawGizmos()
        {
            foreach (var position in GenerateIslandPositions())
            {
                Gizmos.DrawSphere(position.ToVector3(), 500f);
            }
        }
    }
}
