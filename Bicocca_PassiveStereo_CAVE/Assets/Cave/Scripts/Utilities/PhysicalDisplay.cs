using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalDisplay : MonoBehaviour
{
    public HeadConfiguration head;
    public Camera leftCam;
    public Camera rightCam;
    public RectInt leftViewport;
    public RectInt rightViewport;

    public float width;
    public float height;

    public float halfWidth() { return width * 0.5f; }
    public float halfHeight() { return height * 0.5f; }

    public float farClippingPlne  = 0.0f; 

    #region UtilFUNCTIONS
    /// <summary>
    /// A list of every camera associated with this display
    /// </summary>
    /// <returns>A list of every camera associated with this display
    /// </summary></returns>
    public List<Camera> GetAllCameras()
    {
        List<Camera> res = new List<Camera>();
        //if (centerCam != null) res.Add(centerCam);
        if (leftCam != null) res.Add(leftCam);
        if (rightCam != null) res.Add(rightCam);
        return res;
    }

    /// <summary>
    /// Convert a coordinate in screenspace of this display to worldspace
    /// </summary>
    /// <param name="ss">Screenspace coordinate in range {[-1,1], [-1,1]} </param>
    /// <returns>The world space position of ss</returns>
    public Vector3 ScreenspaceToWorld(Vector2 ss)
    {
        return transform.localToWorldMatrix * new Vector4(ss.x * halfWidth(), ss.y * halfHeight(), 0.0f, 1.0f);
    }

    /// <summary>
    /// Upper right (quadrant 1) corner world space coordinate
    /// </summary>
    public Vector3 UpperRight
    {
        get
        {
            return transform.localToWorldMatrix * new Vector4(halfWidth(), halfHeight(), 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Upper left (quadrant 2) corner world space coordinate
    /// </summary>
    public Vector3 UpperLeft
    {
        get
        {
            return transform.localToWorldMatrix * new Vector4(-halfWidth(), halfHeight(), 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Lower left (quadrant 3) corner world space coordinate
    /// </summary>
    public Vector3 LowerLeft
    {
        get
        {
            return transform.localToWorldMatrix * new Vector4(-halfWidth(), -halfHeight(), 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Lower right (quadrant 4) corner world space coordinate
    /// </summary>
    public Vector3 LowerRight
    {
        get
        {
            return transform.localToWorldMatrix * new Vector4(halfWidth(), -halfHeight(), 0.0f, 1.0f);
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // CREATE BOTH EYES ONLY WHEN IN THE EDITOR
#if UNITY_EDITOR
        leftCam = head.CreateLeftEye(gameObject.name);
        rightCam = head.CreateRightEye(gameObject.name);
#endif

        if (GetArg("eye") == "left")
        {
            leftCam = head.CreateLeftEye(gameObject.name);
        }
        if (GetArg("eye") == "right")
        {
            rightCam = head.CreateRightEye(gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (leftCam != null)
        {
            leftCam.pixelRect = new Rect(leftViewport.x, leftViewport.y, leftViewport.width, leftViewport.height);
            farClippingPlne = leftCam.farClipPlane;
        }
        if (rightCam != null)
        {
            rightCam.pixelRect = new Rect(rightViewport.x, rightViewport.y, rightViewport.width, rightViewport.height);
            farClippingPlne = rightCam.farClipPlane;
        }
    }

    private void LateUpdate()
    {
        if (leftCam != null)
        {
            Matrix4x4 leftMat = GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, leftCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
            leftCam.projectionMatrix = leftMat;
            leftCam.transform.rotation = transform.rotation;
        }

        if (rightCam != null)
        {
            Matrix4x4 rightMat = GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, rightCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
            rightCam.projectionMatrix = rightMat;
            rightCam.transform.rotation = transform.rotation;
        }
    }

    private void OnValidate()
    {
        if (leftCam != null && rightCam != null)
        {
            leftCam.farClipPlane = farClippingPlne;
            rightCam.farClipPlane = farClippingPlne;
        }
    }

    public static Matrix4x4 GetAsymProjMatrix(Vector3 lowerLeft, Vector3 lowerRight, Vector3 upperLeft, Vector3 from, float ncp, float fcp)
    {
        //compute orthonormal basis for the screen - could pre-compute this...
        Vector3 vr = (lowerRight - lowerLeft).normalized;
        Vector3 vu = (upperLeft - lowerLeft).normalized;
        Vector3 vn = Vector3.Cross(vr, vu).normalized;

        //compute screen corner vectors
        Vector3 va = lowerLeft - from;
        Vector3 vb = lowerRight - from;
        Vector3 vc = upperLeft - from;

        //find the distance from the eye to screen plane
        float n = ncp;
        float f = fcp;
        float d = Vector3.Dot(va, vn); // distance from eye to screen
        float nod = n / d;
        float l = Vector3.Dot(vr, va) * nod;
        float r = Vector3.Dot(vr, vb) * nod;
        float b = Vector3.Dot(vu, va) * nod;
        float t = Vector3.Dot(vu, vc) * nod;

        //put together the matrix - bout time amirite?
        Matrix4x4 m = Matrix4x4.zero;

        //from http://forum.unity3d.com/threads/using-projection-matrix-to-create-holographic-effect.291123/
        m[0, 0] = 2.0f * n / (r - l);
        m[0, 2] = (r + l) / (r - l);
        m[1, 1] = 2.0f * n / (t - b);
        m[1, 2] = (t + b) / (t - b);
        m[2, 2] = -(f + n) / (f - n);
        m[2, 3] = (-2.0f * f * n) / (f - n);
        m[3, 2] = -1.0f;

        return m;
    }

    /// <summary>
    /// Used to increate performance of GetArg(...)
    /// </summary>
    private static Dictionary<string, string> cachedArgs = new Dictionary<string, string>();

    /// <summary>
    /// Return the string following a certain string in the invokation, null if it doesn't exist
    /// </summary>
    /// <param name="name">parameter name to retrieve</param>
    public static string GetArg(string name)
    {
        if (cachedArgs.ContainsKey(name))
            return cachedArgs[name];

        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
            if (args[i] == name && args.Length > i + 1)
                return (cachedArgs[name] = args[i + 1]);

        return null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw debug view in editor
    /// </summary>
    void EditorDraw()
    {
        var mat = transform.localToWorldMatrix;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(UpperRight, UpperLeft);
        Gizmos.DrawLine(UpperLeft, LowerLeft);
        Gizmos.DrawLine(LowerLeft, LowerRight);
        Gizmos.DrawLine(LowerRight, UpperRight);
    }

    /// <summary>
    /// Draw debug view in editor
    /// </summary>
    void OnDrawGizmos()
    {
        EditorDraw();
    }

    /// <summary>
    /// Draw advanced debug view in editor when selected
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        var mat = transform.localToWorldMatrix;
        Vector3 right = mat * new Vector4(halfWidth() * 0.75f, 0.0f, 0.0f, 1.0f);
        Vector3 up = mat * new Vector4(0.0f, halfHeight() * 0.75f, 0.0f, 1.0f);
        Gizmos.color = new Color(0.75f, 0.25f, 0.25f);
        Gizmos.DrawLine((transform.position * 2.0f + right) / 3.0f, right);
        Gizmos.color = new Color(0.25f, 0.75f, 0.25f);
        Gizmos.DrawLine((transform.position * 2.0f + up) / 3.0f, up);
    }
#endif
}
