using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeInputs
{
    //to automatically bind the main camera to the prefab
    public class SetCanvasCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }

}