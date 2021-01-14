using UnityEngine;

namespace LandGeneration.Augmenters
{
    public interface IIslandDecorator
    {
        void Decorate(Island island, Terrain terrain);
    }
}