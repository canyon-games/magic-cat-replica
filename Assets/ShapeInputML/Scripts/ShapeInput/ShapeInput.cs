using System.Collections.Generic;
using UnityEngine;

namespace ShapeInputs
{
    public static class ShapeInput
    {
 
        private static Shapes _lastShape = Shapes.NONE;
        private static float _lastShapeConfidence = 0f;
        private static float _lastShapeDrawDurationMs = 0f;

        /// <summary>
        /// Get the latest shape input, which can only be read once before being cleared.
        /// On frames with no input queued, it will return Shape.NONE.
        /// If an input was received (something was drawn) but not recognized as a shape, it will return Shape.NONSENSE. 
        /// </summary>
        /// <returns></returns>
        public static ShapeInputResult GetShape()
        {
            Shapes r = _lastShape;
            float c = _lastShapeConfidence;
            float d = _lastShapeDrawDurationMs;
            
            _lastShape = Shapes.NONE;
            _lastShapeConfidence = 0f;
            _lastShapeDrawDurationMs = 0f;

            return new ShapeInputResult() { confidence = c, shape = r, drawTimeMs = d };
        }

        public static void SetInput(Shapes s, float confidence, float durationMs) { 
            _lastShape = s;
            _lastShapeConfidence = confidence;
            _lastShapeDrawDurationMs = durationMs;

        }
       
    }
}
