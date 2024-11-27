using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OmmyShapeML
{
    [RequireComponent(typeof(RawImage))]
    public class ShapeDrawingController : MonoBehaviour
    {
        [Serializable]
        public class DrawCompleted : UnityEvent<RenderTexture, float> { }

        // Events
        internal DrawCompleted drawComplete = new DrawCompleted();

        // Serialized Fields
        [SerializeField] private Shader shader;
        [SerializeField] private Material blitMaterial;
        [SerializeField, Tooltip("Time to wait after drawing ends before interpreting. Default: 0.5f (500ms)")]
        private float debounceSeconds = 0.5f;
        [SerializeField] private int brushDiameter = 3;
        [SerializeField] private RectTransform tracer;

        // Private Fields
        private RawImage imageView;
        private RenderTexture renderTexture;
        private RenderTexture bufferTexture;

        private float debounceTime = 0;
        private bool isDebouncing = false;
        private bool isDrawing = false;
        private float lastDrawStartTime = 0;
        private Vector3 lastPoint = Vector3.zero;
        public bool validDraw;

        private void OnEnable()
        {
            imageView = GetComponent<RawImage>();

            // Assign shader to the material
            blitMaterial.shader = shader;

            // Setup RenderTexture
            renderTexture = new RenderTexture(224, 224, 0, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point
            };
            imageView.texture = renderTexture;

            bufferTexture = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height);
        }

        private void Start()
        {
            ClearTexture();
        }

        public void ClearTexture()
        {
            Graphics.Blit(Texture2D.whiteTexture, renderTexture);
        }

        private void OnDisable()
        {
            renderTexture?.Release();
            bufferTexture?.Release();
        }

        private void Update()
        {
            HandleDrawingLogic();
        }

        private void HandleDrawingLogic()
        {
            if (isDebouncing)
            {
                debounceTime += Time.deltaTime;
                if (debounceTime >= debounceSeconds)
                {
                    CompleteDrawing();
                }
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartDrawing();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDrawing();
            }
            else if (Input.GetMouseButton(0))
            {
                ContinueDrawing();
            }
        }

        private void StartDrawing()
        {
            if (GetTexturePosition(Input.mousePosition, out Vector2 texturePos))
            {
                validDraw = Stamp(new Vector2Int(Mathf.CeilToInt(texturePos.x), Mathf.CeilToInt(texturePos.y)));
                lastPoint = texturePos;

                if (validDraw)
                {
                    if (!isDrawing)
                    {
                        lastDrawStartTime = Time.unscaledTime;
                    }
                    isDrawing = true;

                    if (tracer)
                    {
                        tracer.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void EndDrawing()
        {
            if (isDrawing)
            {
                isDebouncing = true;
            }

            if (tracer)
            {
                var trailRenderer = tracer.GetComponentInChildren<TrailRenderer>();
                if (trailRenderer)
                {
                    trailRenderer.Clear();
                    trailRenderer.enabled = false;
                }
            }
        }

        private void ContinueDrawing()
        {
            if (GetTexturePosition(Input.mousePosition, out Vector2 texturePos) && texturePos != (Vector2)lastPoint)
            {
                validDraw = Stamp(new Vector2Int(Mathf.CeilToInt(texturePos.x), Mathf.CeilToInt(texturePos.y)));

                if (!validDraw && isDrawing && !isDebouncing)
                {
                    isDebouncing = true;
                }

                float distance = Vector2.Distance(texturePos, lastPoint);
                float spacing = brushDiameter * 0.25f;

                if (distance >= spacing)
                {
                    int steps = Mathf.CeilToInt(distance / spacing);
                    for (int i = 0; i < steps; i++)
                    {
                        Stamp(new Vector2Int(
                            Mathf.CeilToInt(Mathf.Lerp(lastPoint.x, texturePos.x, i / (float)steps)),
                            Mathf.CeilToInt(Mathf.Lerp(lastPoint.y, texturePos.y, i / (float)steps))
                        ));
                    }
                }

                lastPoint = texturePos;
                debounceTime = 0;

                if (tracer)
                {
                    UpdateTracerPosition();
                }
            }
        }

        private void CompleteDrawing()
        {
            debounceTime = 0;
            isDebouncing = false;
            isDrawing = false;

            drawComplete?.Invoke(renderTexture, (Time.unscaledTime - lastDrawStartTime) * 1000.0f);
            ClearTexture();

            if (tracer)
            {
                tracer.gameObject.SetActive(false);
                var trailRenderer = tracer.GetComponentInChildren<TrailRenderer>();
                if (trailRenderer)
                {
                    trailRenderer.Clear();
                }
            }
        }

        private void UpdateTracerPosition()
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tracer.position = new Vector3(worldPos.x, worldPos.y, 0);

            var trailRenderer = tracer.GetComponentInChildren<TrailRenderer>();
            if (trailRenderer)
            {
                trailRenderer.enabled = true;
            }
        }

        private bool Stamp(Vector2Int position)
        {
            float radius = brushDiameter / 2f;
            Vector2Int size = new Vector2Int(brushDiameter, brushDiameter);
            Vector2 offset = new Vector2(radius, radius);

            blitMaterial.SetTexture("_BrushTex", renderTexture);
            blitMaterial.SetVector("_size", (Vector2)size);
            blitMaterial.SetVector("_sPos", position - offset);
            blitMaterial.SetColor("_color", Color.black);

            UpdateTexture();

            return new Rect(0, 0, renderTexture.width, renderTexture.height).Contains(position);
        }

        private void UpdateTexture()
        {
            Graphics.Blit(renderTexture, bufferTexture, blitMaterial);
            Graphics.Blit(bufferTexture, renderTexture);
        }

        private bool GetTexturePosition(Vector2 mousePosition, out Vector2 texturePosition)
        {
            RectTransform rectTransform = imageView.GetComponent<RectTransform>();
            Vector2 localCursor;

            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, null, out localCursor))
            {
                Vector2 mappedSize = localCursor / rectTransform.rect.size;
                texturePosition = new Vector2(renderTexture.width, renderTexture.height) * mappedSize;
                return true;
            }

            texturePosition = Vector2.zero;
            return false;
        }
    }
}