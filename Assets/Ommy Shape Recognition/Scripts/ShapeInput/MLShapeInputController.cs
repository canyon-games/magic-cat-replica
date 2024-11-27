using System;
using System.Linq;
using Unity.Barracuda;
using UnityEditor;
using UnityEngine;

namespace OmmyShapeML
{
    [RequireComponent(typeof(ShapeDrawingController))]
    public class MLShapeInputController : MonoBehaviour
    {
        [SerializeField] private NNModel model; // ONNX file, e.g., https://github.com/jackwish/tflite2onnx
        
        private IWorker engine;
        private bool isProcessing = false;

        private void OnEnable()
        {
            var shapeDrawingController = GetComponent<ShapeDrawingController>();
            if (shapeDrawingController != null)
            {
                shapeDrawingController.drawComplete.AddListener(ProcessInput);
            }
            else
            {
                Debug.LogError("ShapeDrawingController component is missing.");
            }

            if (model != null)
            {
                engine = model.CreateWorker(WorkerFactory.Device.CSharp);
            }
            else
            {
                Debug.LogError("NNModel is not assigned.");
            }
        }

        private void OnDestroy()
        {
            var shapeDrawingController = GetComponent<ShapeDrawingController>();
            shapeDrawingController?.drawComplete.RemoveListener(ProcessInput);

            engine?.Dispose();
        }

        private void ProcessInput(RenderTexture texture, float drawDurationMs)
        {
            if (isProcessing) return;

            isProcessing = true;
            try
            {
                InvokeModel(texture, drawDurationMs);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                isProcessing = false;
            }
        }

#if DEBUG
        private void SaveRenderTextureToFile(RenderTexture renderTexture)
        {
            if (renderTexture == null) return;

            var prevRT = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = prevRT;

            byte[] bytes = texture.EncodeToPNG();
            string path = $"{AssetDatabase.GetAssetPath(renderTexture)}_rt.png";

            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path);
            Debug.Log($"RenderTexture saved to {path}");
        }
#endif

        private void InvokeModel(RenderTexture texture, float drawDurationMs)
        {
#if DEBUG
            SaveRenderTextureToFile(texture);
#endif
            if (texture == null || engine == null)
            {
                Debug.LogError("Invalid input texture or uninitialized engine.");
                return;
            }

            using (var inputTensor = new Tensor(texture, 3)) // Assume RGB input (3 channels)
            {
                using (var outputTensor = engine.Execute(inputTensor).PeekOutput())
                {
                    ProcessOutput(outputTensor.AsFloats(), drawDurationMs);
                }
            }
        }

        private void ProcessOutput(float[] results, float drawDurationMs)
        {
            if (results == null || results.Length == 0)
            {
                Debug.LogWarning("Empty or null output from the model.");
                ShapeInput.SetInput(Shapes.NONE, 0f, drawDurationMs);
                return;
            }

            // Find the index of the maximum value in the results array
            int maxIndex = -1;
            float maxValue = results.Max();

            if (maxValue > 0)
            {
                maxIndex = Array.IndexOf(results, maxValue);
            }

            // Map index to shape result
            var resultShape = (Shapes)maxIndex;
            ShapeInput.SetInput(resultShape, maxIndex == -1 ? 0f : maxValue, drawDurationMs);
        }
    }
}