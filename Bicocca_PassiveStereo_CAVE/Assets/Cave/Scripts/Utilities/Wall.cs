using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wall : MonoBehaviour
{
    public const float DefaultSeparation = 0.067f;
    public const float MinSeparation = 0.01f;
    public const float MaxSeparation = 0.10f;
    public const float DefaultConvergence = 5.0f;
    public const float MinConvergence = 0.01f;
    public const float MaxConvergence = 10.0f;

    [Header("Camera Parameters")]
    public float separation = DefaultSeparation;
    public float convergence = DefaultConvergence;

    [Header("Viewport")]
    public float width;
    public float height;

    [Header("Cameras")]
    public Camera leftCam;
    public Camera rightCam;

    [Header("Camera Indexes")]
    public int leftCameraIndex = 1;
    public int rightCameraIndex = 2;

    [Header("Machine Configurations")]
    [SerializeField] MachineConfig machineConfig;

    [HideInEditorMode]
    [SerializeField] private bool shouldBeActive;

    [Header("Debug Options")]
    [SerializeField] private bool forceActive;

    public bool ForceActive
    {
        get => forceActive;
        set => forceActive = value;
    }

    public bool ShouldBeActive => CheckMachineName();

    public float halfWidth() { return width * 0.5f; }
    public float halfHeight() { return height * 0.5f; }

    public int ConvertIndex(int n) { return n - 1; }

    #region UtilFUNCTIONS
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

    void Start()
    {
        Initialize();
        shouldBeActive = CheckMachineName();
    }

    void Update()
    {
        //if (Input.GetKey(KeyCode.F2))
        //{
        //    separation += 0.1f * Time.deltaTime;
        //}

        //if (Input.GetKey(KeyCode.F1))
        //{
        //    separation -= 0.1f * Time.deltaTime;
        //}

        //if (Input.GetKey(KeyCode.F3))
        //{
        //    convergence += 1.0f * Time.deltaTime;
        //}

        //if (Input.GetKey(KeyCode.F4))
        //{
        //    convergence -= 1.0f * Time.deltaTime;
        //}

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (machineConfig.Name.Equals(Util.GetMachineName()))
            {
                SwapEye();
            }
        }

        ApplyLayot();
    }

    private void LateUpdate()
    {
        if (leftCam != null)
        {
            Matrix4x4 leftMat = GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, leftCam.transform.position, leftCam.nearClipPlane, leftCam.farClipPlane);
            leftCam.projectionMatrix = leftMat;
            leftCam.transform.rotation = transform.rotation;
        }

        if (rightCam != null)
        {
            Matrix4x4 rightMat = GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, rightCam.transform.position, rightCam.nearClipPlane, rightCam.farClipPlane);
            rightCam.projectionMatrix = rightMat;
            rightCam.transform.rotation = transform.rotation;
        }
    }


    /// <summary>
    /// Initialize camera parameters
    /// </summary>
    private void Initialize()
    {
        //Making sure that the cameras are not set to orthographic
        leftCam.orthographic = false;
        rightCam.orthographic = false;

        //Keeping a full resolution on both cameras
        leftCam.rect = new Rect(0, 0, 1, 1);
        rightCam.rect = new Rect(0, 0, 1, 1);

        //Assigns the correct index by subtracting 1
        int tempLeftCamIn = ConvertIndex(leftCameraIndex);
        int tempRightCamIn= ConvertIndex(rightCameraIndex);

        if (Display.displays.Length >= 2)
        {
            //Activates the selected displays
            Display.displays[tempLeftCamIn].Activate();
            Display.displays[tempRightCamIn].Activate();
        }
        
        //Sets the target display for each camera 
        leftCam.targetDisplay = tempLeftCamIn;
        rightCam.targetDisplay = tempRightCamIn;

        ApplyLayot();
    }

    private void ApplyLayot()
    {
        //// eye separation
        //var halfSeparation = separation / 2.0f;
        //leftCam.transform.localPosition = new Vector3(-separation, 0.0f, 0.0f);
        //rightCam.transform.localPosition = new Vector3(separation, 0.0f, 0.0f);

        //// convergence
        //var angle = 90.0f - Mathf.Atan2(convergence, halfSeparation) * Mathf.Rad2Deg;
        //leftCam.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
        //rightCam.transform.localRotation = Quaternion.AngleAxis(-angle, Vector3.up);
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

    private bool CheckMachineName()
    {
        return (forceActive || machineConfig.Name.Equals(Util.GetMachineName()));
    }

    private void SwapEye()
    {
        int tempLeft = leftCam.targetDisplay;
        int tempRight = rightCam.targetDisplay;

        leftCam.targetDisplay = tempRight;
        rightCam.targetDisplay = tempLeft;
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
