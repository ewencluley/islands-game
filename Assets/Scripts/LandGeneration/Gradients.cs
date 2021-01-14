using UnityEngine;

namespace LandGeneration
{
    public static class Gradients
    {
        public static float RadialGradient(int x, int y, int width, int height)
        {
            var centre = new Vector2Int(Mathf.FloorToInt(width/2f), Mathf.FloorToInt(height/2f));
            var maxDistFromCentre = (Mathf.Max(width, height) / 2f);
            
            var distanceFromCentre = Vector2.Distance(new Vector2Int(x, y), centre);
            return 1 - (distanceFromCentre / maxDistFromCentre);
        }
    }
}