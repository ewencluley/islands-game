using System;
using Unity.Mathematics;
using UnityEngine;

namespace LandGeneration.FeatureDetector
{
    public interface IFeatureDetector
    {
        void PreProcess(int x, int y);
        void Detect();
    }
}