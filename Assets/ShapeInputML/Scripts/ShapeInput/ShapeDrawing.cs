using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ShapeInputs
{

    [RequireComponent(typeof(RawImage))]
    public class ShapeDrawing : MonoBehaviour
    {
        [Serializable] 
        public class DrawCompleted : UnityEvent<RenderTexture,float> { }

        internal DrawCompleted drawComplete = new DrawCompleted();

        //do not change these
        [SerializeField]
        private Shader shader;

        [SerializeField] 
        private Material _blitMaterial;


        [SerializeField, Tooltip("Time to wait after drawing ends before interpreting. Default: 0.5f (500ms)")] 
        private float debounceSeconds = 0.5f; //configurable debounce

        //diameter of drawing brush, result should match training images in line thickness. 
        int _diameter = 3; 
        
        //incremented after drawing begins, reset when drawing, invokes the interpreter when it reaches debounceSeconds
        private float debounceTime = 0;
        
        //whether the debounce timer is running
        private bool isDebouncing = false;

        //whether the user is drawing on the panel or not
        private bool isDrawing = false;

        //time in seconds that the current drawing began
        private float lastDrawTimeStart = 0;

        //UI object that can follow the user finger during their drawing (think trail or particle system)
        public RectTransform tracer;


        private RawImage imageView = null;

        //the last recording finger location relative to the drawn textures original (bottom left)
        private Vector3 lastPoint = Vector3.zero;

        
        //a 224x224 texture we will ship to the ML for interpreting. 
        //white background, black drawing, no antialiasing
        private RenderTexture _renderTexture;
        private RenderTexture _bufferTexture;

        private void OnEnable()
        {
            imageView = GetComponent<RawImage>();

            //shader is Hidden, so we have to attach it to material here:
            _blitMaterial.shader = shader;

            //set up the RenderTexture attached to our RawImage ( do not change these arguments )
            _renderTexture = new RenderTexture(224, 224, 0, RenderTextureFormat.ARGB32);
            _renderTexture.filterMode = FilterMode.Point;
            imageView.texture = _renderTexture; //doesnt show result..

            _bufferTexture = RenderTexture.GetTemporary(_renderTexture.width, _renderTexture.height);
        }

        private void Start()
        {
            ClearTexture();
        }

        public void ClearTexture()
        {
            Graphics.Blit(Texture2D.whiteTexture, _renderTexture);
        }

        private void OnDisable()
        {
            _renderTexture?.Release();
            _bufferTexture?.Release();
        }

        private void Update()
        {
            if (isDebouncing)
            {
                debounceTime += Time.deltaTime;
                if (debounceTime >= debounceSeconds)
                {
                    debounceTime = 0;
                    isDebouncing = false;
                    isDrawing = false;
                    drawComplete?.Invoke(_renderTexture, (Time.unscaledTime - lastDrawTimeStart) *1000.0f);

                    ClearTexture();
                    //we are guaranteed at minimum { debounceSeconds } in between event invocations.
                    //The listener will discard event calls if it is still processing a previous call.

                    if (tracer)
                    {
                        var tr = tracer.GetComponentInChildren<TrailRenderer>();
                        if (tr) //trail renderer can safely be removed from the prefab
                        {
                            tr.Clear(); //doesnt seem to ... work. 
                            tr.enabled = false;
                        }

                        tracer.gameObject.SetActive(false);
                    }
                    return;
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                Vector2 texturePos;
                if (!GetPixelByMousePosition(Input.mousePosition, out texturePos)) return;
                bool validDraw = Stamp(new Vector2Int(Mathf.CeilToInt(texturePos.x), Mathf.CeilToInt(texturePos.y)));
                lastPoint = texturePos;

                if (validDraw)
                {
                    if (!isDrawing)//if a subsequent line is being drawn, do not reset the start time. 
                    {
                        lastDrawTimeStart = Time.unscaledTime;
                    }
                    isDrawing = true;                   
                    if (tracer)
                    {
                        tracer.gameObject.SetActive(true);
                    }
                }
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (tracer)
                {
                    var tr = tracer.GetComponentInChildren<TrailRenderer>();
                    if (tr)
                    {
                        tr.Clear();
                        tr.enabled = false;
                    }
                }
                if (isDrawing) //had to have mouse down'd on the drawing panel to start timer on mouse up
                {
                    isDebouncing = true;                    
                }
            }
            if (Input.GetMouseButton(0))
            {
                Vector2 texturePos;
                if (!GetPixelByMousePosition(Input.mousePosition, out texturePos) || texturePos == new Vector2(lastPoint.x, lastPoint.y)) return;
                bool validDraw = Stamp(new Vector2Int(Mathf.CeilToInt(texturePos.x), Mathf.CeilToInt(texturePos.y)));


                //if you moused out of the draw panel, were drawing, make sure timer starts:
                if(!validDraw && isDrawing && !isDebouncing)
                {
                    isDebouncing = true;                   
                }


                var distance = Vector2.Distance(texturePos, lastPoint);
                var f = _diameter * 0.25f;
                if (distance >= f)
                {
                    var divs = distance / f;
                    for (int i = 0; i < divs; i++)
                    {
                        Stamp(
                                new Vector2Int(Mathf.CeilToInt(Mathf.Lerp(lastPoint.x, texturePos.x, i / divs)),
                                               Mathf.CeilToInt(Mathf.Lerp(lastPoint.y, texturePos.y, i / divs)))
                            );
                    }
                }

                lastPoint = texturePos;
                debounceTime = 0;
                if (tracer)
                {
                    // tracer.localPosition = Input.mousePosition  - new Vector3(Screen.width/2f, Screen.height/2f, 0);
                    //to support particle systems on the UI, we have to use Screen Space - Camera on the canvas, which changes rules for positioning. 
                    var v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    tracer.position = new Vector3(v.x, v.y, 0);
                    var tr = tracer.GetComponentInChildren<TrailRenderer>();
                    if (tr)
                    {
                        tr.enabled = true;
                    }
                }
            }

        }


        private bool Stamp(Vector2Int p)
        {
            var radius = _diameter / 2f;
            var size = new Vector2Int(_diameter, _diameter);
            var o = new Vector2(radius, radius);
            _blitMaterial.SetTexture("_BrushTex", _renderTexture);
            _blitMaterial.SetVector("_size", (Vector2)size);
            _blitMaterial.SetVector("_sPos", p - o);
            _blitMaterial.SetColor("_color", Color.black);
            UpdateTexture();
            return new Rect(0, 0, _renderTexture.width, _renderTexture.height).Contains(p);
        }

        private void UpdateTexture()
        {
            Graphics.Blit(_renderTexture, _bufferTexture, _blitMaterial);
            Graphics.Blit(_bufferTexture, _renderTexture);
        }

        private bool GetPixelByMousePosition(Vector2 mPos, out Vector2 texturePos)
        {
            Vector2 localCursor;
            var iRect = imageView.GetComponent<RectTransform>();

            //when canvas render mode is Screen Space - Camera:
            mPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, 0)); //otherwise, comment out. 

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(iRect, mPos, null, out localCursor))
            {
                texturePos = Vector2.zero;
                return false;
            }

            var mappedSize = localCursor / iRect.rect.size;
            texturePos = new Vector2Int(_renderTexture.width, _renderTexture.height) * mappedSize;
            return true;
        }

    }
}