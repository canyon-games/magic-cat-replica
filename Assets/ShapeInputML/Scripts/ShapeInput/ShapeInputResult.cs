using System;
using UnityEngine;

namespace ShapeInputs
{

    public struct ShapeInputResult
    {
        public Shapes shape;

        /// <summary>
        /// Confidence in the recognition of the shape. 1 being the most confident. 
        /// </summary>
        [Range(0, 1)]
        public float confidence;

        public float drawTimeMs;

        public override string ToString()
        {
            return $"Shape: {shape}, confidence: {confidence * 100f, 8:F2}%, draw duration: {drawTimeMs,8:F2}ms";
        }
    }
}
