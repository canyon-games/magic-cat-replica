using System;
using System.Linq;
using Unity.Barracuda;
using UnityEditor;
using UnityEngine;
namespace ShapeInputs
{

    [RequireComponent(typeof(ShapeDrawingController))]
    public class MLShapeInputController : MonoBehaviour
    {

        [SerializeField] private NNModel model; //ONNX file https://github.com/jackwish/tflite2onnx
        
        private IWorker engine;
        private bool isProcessing = false;
      
        private void OnEnable()
        {
            GetComponent<ShapeDrawingController>().drawComplete.AddListener(ProcessInput);
            engine = model.CreateWorker(WorkerFactory.Device.CSharp);
        }

        private void OnDestroy()
        {
            GetComponent<ShapeDrawingController>()?.drawComplete.RemoveListener(ProcessInput);
            engine?.Dispose();
        }

        private void ProcessInput(RenderTexture texture, float drawDurationMs)
        {
            if (isProcessing) return;
            isProcessing = true;
            try
            {
                Invoke(texture, drawDurationMs);
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
        void SaveRTToFile(RenderTexture rt)
        {
            var prevRT = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            RenderTexture.active = prevRT;

            byte[] bytes;
            bytes = tex.EncodeToPNG();

            string path = AssetDatabase.GetAssetPath(rt) + "rt.png";
            System.IO.File.WriteAllBytes(path, bytes);
            UnityEditor.AssetDatabase.ImportAsset(path);
            Debug.Log("Saved to " + path);
        }
#endif

        private void Invoke(RenderTexture texture, float drawDurationMs)
        {
#if DEBUG
            SaveRTToFile(texture);
#endif

            using (var input = new Tensor(texture, 3)) //teachablemachine uses RGB images (3 channels) - we used only black and white to train.
            {
                using (var output = engine.Execute(input).PeekOutput())
                {
                    var results = output.AsFloats();
                    int maxValueIndex = -1;
                    if (results.Max() <= 0)
                    {
                        maxValueIndex = -1; //Shape.NONE - YOU MAY WANT TO USE 6 INSTEAD (SHAPES.NONSENSE)
                    }
                    else
                    {
                        float _max = 0f;
                        for (int i = 0; i < results.Length; i++)
                        {
                            if (results[i] > _max)
                            {
                                maxValueIndex = i;
                                _max = results[i];
                            }
                        }
                    }
                    //Shape.NONE is global (-1) and not part of the model classes.                 
                    var result = (Shapes)maxValueIndex;                    
                    ShapeInput.SetInput(result, maxValueIndex == -1 ? 0f : results[maxValueIndex], drawDurationMs);
                }
            }
        }
    }
}