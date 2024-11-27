using System.Collections.Generic;
using UnityEngine;

namespace OmmyShapeML
{
    public static class ShapeInput
    {
 
        private static Shapes lastShape = Shapes.NONE;
        private static float lastShapeConfidence = 0f;
        private static float lastShapeDrawDurationMs = 0f;

/// <summary>
/// Retrieves the most recent shape input. This value can only be accessed once before being cleared.
/// If no input is queued, it returns <see cref="Shapes.NONE"/>.
/// If a drawing was made but could not be recognized as a valid shape, it returns <see cref="Shapes.NONSENSE"/>.
/// </summary>
/// <returns>
/// A <see cref="ShapeInputResult"/> containing the recognized shape, the confidence level of the recognition, and the duration of the drawing in milliseconds.
/// </returns>
/// 
        public static ShapeInputResult GetShape()
        {
            Shapes r = lastShape;
            float c = lastShapeConfidence;
            float d = lastShapeDrawDurationMs;
            
            lastShape = Shapes.NONE;
            lastShapeConfidence = 0f;
            lastShapeDrawDurationMs = 0f;

            return new ShapeInputResult() { confidence = c, shape = r, drawTimeMs = d };
        }

        public static void SetInput(Shapes shape, float confidenceValue, float durationInMs) { 
            lastShape = shape;
            lastShapeConfidence = confidenceValue;
            lastShapeDrawDurationMs = durationInMs;

        }
       
    }
}
