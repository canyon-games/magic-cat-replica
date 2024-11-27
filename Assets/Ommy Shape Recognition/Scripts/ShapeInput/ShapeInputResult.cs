using System;
using UnityEngine;

namespace OmmyShapeML
{

    public struct ShapeInputResult
    {
        public Shapes shape;

/// <summary>
/// The confidence level in the recognition of the shape, ranging from 0 to 1, 
/// with 1 representing the highest confidence in the shape's identification.
/// </summary>

        [Range(0, 1)]
        public float confidence;

        public float drawTimeMs;

        public override string ToString()
        {
            return $"Shape name: {shape}, confidence level: {confidence * 100f, 8:F2}%, draw time: {drawTimeMs,8:F2}ms";
        }
    }
}
