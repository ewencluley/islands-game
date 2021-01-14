using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LandGeneration.FeatureDetector
{
    public class ShorelineFeatureDetector: IFeatureDetector
    {
        private readonly HashSet<HeightMapNode> _shorelineNodes = new HashSet<HeightMapNode>();
        private readonly Island _island;
        private readonly Terrain _terrain;

        private readonly float _heightmapSeaLevel;

        public ShorelineFeatureDetector(Island island, Terrain terrain)
        {
            _island = island;
            _terrain = terrain;
            _heightmapSeaLevel = Sea.Instance.GetSeaLevel() / terrain.terrainData.size.y;
        }
        
        public void PreProcess(int x, int y)
        {
            var node = GetNode(x, y);
            var neighbours = GetNonDiagonalNeighbours(node);
            if (IsWaterCloseToLand(node, neighbours))
            {
                _shorelineNodes.Add(node);
            }
        }

        private bool IsWaterCloseToLand(HeightMapNode node, HashSet<HeightMapNode> neighbours)
        {
            return IsBelowSeaLevel(node) && neighbours.Any(IsAboveSeaLevel);
        }

        public void Detect()
        {
            _island.shoreline = _shorelineNodes.Select(HeightmapNodeToWorldSpace).ToArray();
            var remaining = new HashSet<HeightMapNode>(_shorelineNodes);
            var lastRemainingCount = remaining.Count;
            while (remaining.Count > 0)
            {
                var start = GetAnElement(remaining);
                HashSet<HeightMapNode> visited = new HashSet<HeightMapNode>();
                var shorePath = JoinShore(start, start, start, remaining, visited);
                
                if (shorePath != null)
                {
                    _island.ShorelinePaths.Add(shorePath);
                }

                remaining = new HashSet<HeightMapNode>(remaining.Except(visited));
                
                if (remaining.Count == lastRemainingCount)
                {
                    Debug.Log($"Nothing found this time round. {lastRemainingCount} nodes unassigned to a shoreline");
                    break;
                }

                lastRemainingCount = remaining.Count;
            }
            
            Debug.Log($"Detected shorelines {_island.ShorelinePaths.Count}");
        }

        private List<Vector3> JoinShore(HeightMapNode lastSeen, HeightMapNode current, HeightMapNode target, HashSet<HeightMapNode> shorelineCandidates, HashSet<HeightMapNode> visited)
        {
            var allNeighbours = new HashSet<HeightMapNode>(GetNeighbours(current).Intersect(shorelineCandidates));
            var unvisitedNeighbours = new HashSet<HeightMapNode>(allNeighbours.Except(visited));
            if (allNeighbours.Contains(target) && visited.Count > 2)
            {
                return new List<Vector3>();
            }

            visited.Add(current);
            List<HeightMapNode> sortedNeighbours = SortNeighbours(unvisitedNeighbours, lastSeen, current);
            foreach (var neighbour in sortedNeighbours)
            {
                var shore = JoinShore(current, neighbour, target, shorelineCandidates, visited);
                if (shore != null)
                {
                    shore.Add(HeightmapNodeToWorldSpace(neighbour));
                    return shore;
                }
            }
            return null;
        }

        private List<HeightMapNode> SortNeighbours(HashSet<HeightMapNode> neighbours, HeightMapNode last, HeightMapNode current)
        {
            return neighbours
                .OrderBy(n => Mathf.Abs(Vector2Int.Distance(current.coords, n.coords)))
                .ToList();
        }

        private bool IsNeighbour(HeightMapNode a, HeightMapNode b)
        {
            return (Mathf.Abs(a.GetX() - b.GetX()) <= 1 && Mathf.Abs(a.GetY() - b.GetY()) <= 1); //X and Y are always positive
        }

        private bool IsBelowSeaLevel(HeightMapNode node)
        {
            return !IsAboveSeaLevel(node);
        }
        private bool IsAboveSeaLevel(HeightMapNode node)
        {
            return node.height > _heightmapSeaLevel;
        }

        private HashSet<HeightMapNode> GetNeighbours(HeightMapNode node)
        {
            return new HashSet<HeightMapNode> (new[]
                {
                    GetNode(node.GetX() + 1, node.GetY()),
                    GetNode(node.GetX() - 1, node.GetY()),
                    GetNode(node.GetX() + 1, node.GetY() + 1),
                    GetNode(node.GetX() - 1, node.GetY() + 1),
                    GetNode(node.GetX() + 1, node.GetY() - 1),
                    GetNode(node.GetX() - 1, node.GetY() - 1),
                    GetNode(node.GetX(), node.GetY()+ 1),
                    GetNode(node.GetX(), node.GetY()-1),
                }.Where(pos => InBounds(pos.GetX(), pos.GetY()))
            );
        }
        
        private HashSet<HeightMapNode> GetNonDiagonalNeighbours(HeightMapNode node)
        {
            return new HashSet<HeightMapNode> (new[]
                {
                    GetNode(node.GetX() + 1, node.GetY()),
                    GetNode(node.GetX() - 1, node.GetY()),
                    GetNode(node.GetX(), node.GetY()+ 1),
                    GetNode(node.GetX(), node.GetY()-1),
                }.Where(pos => InBounds(pos.GetX(), pos.GetY()))
            );
        }

        private static T GetAnElement<T>(HashSet<T> set) where T: struct
        {
            foreach (var element in set)
            {
                return element;
            }

            throw new Exception("Cannot get element from empty set.");
        }

        private Vector3 HeightmapNodeToWorldSpace(HeightMapNode node)
        {
            var terrainData = _terrain.terrainData;
            var terrainSize = terrainData.size;
            var heightmapResolution = terrainData.heightmapResolution;
            
            return new Vector3(
                node.GetX() * terrainSize.x / heightmapResolution,
                node.height * terrainSize.y,
                node.GetY() * terrainSize.z / heightmapResolution
            );
        }


        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < _island.HeightMapResolution && y >= 0 && y < _island.HeightMapResolution;
        }

        private HeightMapNode GetNode(int x, int y)
        {
            var height = InBounds(x, y) ? _island.Heights[y * _island.HeightMapResolution + x] : -100f;
            return new HeightMapNode(x, height, y);
        }

        struct HeightMapNode
        {
            internal readonly Vector2Int coords;
            internal readonly float height;

            public HeightMapNode(int x, float height, int y) : this()
            {
                coords = new Vector2Int(x, y);
                this.height = height;
            }

            internal int GetX()
            {
                return coords.x;
            }

            internal int GetY()
            {
                return coords.y;
            }
            
            public override int GetHashCode()
            {
                return coords.GetHashCode();
            }

            public override bool Equals(object other)
            {
                return other is HeightMapNode other1 && coords.Equals(other1.coords);
            }

            public override string ToString()
            {
                return $"({coords.x}, {coords.y}) : {height}";
            }
        }
    }
}