//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Benny Wysong-Grass
//University of Wisconsin - Madison Virtual Environments Group
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

/// <summary>
/// Class responsible for creating the cameras necessary to render for this machine's displays
/// Cameras are created as children of this object, so movement and rotation correctly positions cameras
/// Head and eyes should correspond to the viewer's real head and eyes
/// </summary>
[Serializable]
public class HeadConfiguration : MonoBehaviour
{ 
    /// <summary>
    /// Prefab to create, must contain a Camera component
    /// </summary>
    [Tooltip("Prefab object with camera component")]
    public GameObject camPrefab = null;

    [Header("Eye Separation")]
    public Vector3 leftEyeOffset;
    public Vector3 centerEyeOffset { get { return (leftEyeOffset + rightEyeOffset) * 0.5f; } }
    public Vector3 rightEyeOffset;

    public float nearClippingPlane = 0.01f, farClippingPlane = 100.0f;
    
    /// <summary>
    /// Create left eye if it doesn't already exist
    /// </summary>
    /// <param name="name">name of display that the camera is for</param>
    /// <returns>The left eye camera</returns>
    public Camera CreateLeftEye(string name) {
        GameObject obj;
        Camera res;
        if (camPrefab != null) {
            obj = Instantiate(camPrefab);
            res = obj.GetComponent<Camera>();
        } else {
            obj = new GameObject();
            res = obj.AddComponent<Camera>();
        }
        obj.name = "Left Eye For: " + name;
        res.nearClipPlane = nearClippingPlane;
        res.farClipPlane = farClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = leftEyeOffset;
        res.backgroundColor = Color.black;
        res.clearFlags = CameraClearFlags.SolidColor;

        //Instantiate a canvas with the current screen name 
        CreateCanvas("Left", res.transform);

        return res;
    }

    /// <summary>
    /// Create right eye if it doesn't already exist
    /// </summary>
    /// <param name="name">name of display that the camera is for</param>
    /// <returns>The right eye camera</returns>
    public Camera CreateRightEye(string name) {
        GameObject obj;
        Camera res;
        if (camPrefab != null) {
            obj = Instantiate(camPrefab);
            res = obj.GetComponent<Camera>();
        } else {
            obj = new GameObject();
            res = obj.AddComponent<Camera>();
        }
        obj.name = "Right Eye For: " + name;
        res.nearClipPlane = nearClippingPlane;
        res.farClipPlane = farClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = rightEyeOffset;
        res.backgroundColor = Color.black;
        res.clearFlags = CameraClearFlags.SolidColor;

        //Instantiate a canvas with the current screen name 
        CreateCanvas("Right", res.transform);
        return res;
    }

    private void CreateCanvas(string monitorName, Transform parentTransform)
    {
        GameObject myGO;
        GameObject myText;
        Canvas myCanvas;
        Text text;
        RectTransform rectTransform;

        // Canvas
        myGO = new GameObject();
        myGO.name = monitorName + "Canvas";
        myGO.AddComponent<Canvas>();

        myCanvas = myGO.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        myCanvas.targetDisplay = 0;
        myGO.AddComponent<CanvasScaler>();
        myGO.AddComponent<GraphicRaycaster>();

        // Text
        myText = new GameObject();
        myText.transform.parent = myGO.transform;
        myText.name = monitorName;

        text = myText.AddComponent<Text>();
        //text.font = (Font)Resources.Load("Arial");
        text.font = text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text = monitorName;
        text.fontSize = 100;

        // Text position
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(400, 200);
        myGO.transform.parent = parentTransform;
    }

#if UNITY_EDITOR

    /// <summary>
    /// Draw a sphere at the position of the head, and smaller spheres for each eye (including center eye)
    /// </summary>
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(leftEyeOffset.x, leftEyeOffset.y, leftEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(centerEyeOffset.x, centerEyeOffset.y, centerEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(rightEyeOffset.x, rightEyeOffset.y, rightEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(leftEyeOffset.magnitude, rightEyeOffset.magnitude) + 0.02f);
    }
    #endif
}
