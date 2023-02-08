using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StereoCamera : MonoBehaviour
{
    public const float DefaultSeparation = 0.067f;
    public const float MinSeparation = 0.01f;
    public const float MaxSeparation = 0.10f;
    public const float DefaultConvergence = 5.0f;
    public const float MinConvergence = 0.01f;
    public const float MaxConvergence = 10.0f;

    public Camera leftCamera;
    public Camera rightCamera;

    public float separation = DefaultSeparation;
    public float convergence = DefaultConvergence;

    float previousSeparation;
    float previousConvergence;

    [Header("UI Controls")]
    [SerializeField] GameObject m_SettingsPanel;
    [SerializeField] Text m_EyeSeparationTextValue;
    [SerializeField] Text m_ConvergenceTextValue;

    [Header("ConvergencePlane")]
    [SerializeField] GameObject m_DebugConvergencePlane;

    float m_Timer = 0.0f;
    float m_TimerDefaultValue = 3.0f;

    // Use this for initialization
    void Start()
    {
        Display.displays[0].Activate();
        Display.displays[1].Activate();
        Debug.Assert(rightCamera != null, "rightCamera can't be null!");
        Debug.Assert(leftCamera != null, "leftCamera can't be null!");

        // apply fixed properties
        Initialize();

        m_DebugConvergencePlane.SetActive(false);
        m_DebugConvergencePlane.transform.localPosition = new Vector3(0, 0, convergence);
    }

    // Update is called once per frame
    void Update()
    {
        #region GetInput
        if (Input.GetKey(KeyCode.F2))
        {
            EnableSettingsUI();
            separation += 0.1f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.F1))
        {
            EnableSettingsUI();
            separation -= 0.1f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.F3))
        {
            EnableSettingsUI();
            convergence += 1.0f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.F4))
        {
            EnableSettingsUI();
            convergence -= 1.0f * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_DebugConvergencePlane != null)
            {
                m_DebugConvergencePlane.SetActive(!m_DebugConvergencePlane.activeSelf);
            }
        }
        #endregion

        separation = Mathf.Clamp(separation, MinSeparation, MaxSeparation);
        convergence = Mathf.Clamp(convergence, MinConvergence, MaxConvergence);

        // handle property changes
        if (previousSeparation != separation || previousConvergence != convergence)
        {
            ApplyLayout();

            // update UI 
            if (m_EyeSeparationTextValue != null)
            {
                m_EyeSeparationTextValue.text = separation.ToString("F3");
            }

            if (m_ConvergenceTextValue != null)
            {
                m_ConvergenceTextValue.text = convergence.ToString("F3");
            }

            if (m_DebugConvergencePlane != null)
            {
                m_DebugConvergencePlane.transform.localPosition = new Vector3(0, 0, convergence);
            }
        }

        if (m_Timer <= 0)
        {
            m_SettingsPanel.SetActive(false);
        }
        else
        {
            m_Timer -= Time.deltaTime;
        }
    }

    void Reset()
    {
        separation = DefaultSeparation;
        convergence = DefaultConvergence;
    }

    void OnValidate()
    {
        // clamp values
        separation = Mathf.Clamp(separation, MinSeparation, MaxSeparation);
        convergence = Mathf.Clamp(convergence, MinConvergence, MaxConvergence);
    }

    void EnableSettingsUI()
    {
        m_Timer = m_TimerDefaultValue;
        m_SettingsPanel.SetActive(true);
    }

    public void Initialize()
    {
        leftCamera.orthographic = false;
        rightCamera.orthographic = false;

        //Half Resolution Rect
        //leftCamera.rect = new Rect(0, 0, 0.5f, 1);
        //rightCamera.rect = new Rect(0.5f, 0, 0.5f, 1);

        //Full Screen Rect
        leftCamera.rect = new Rect(0, 0, 1, 1);
        rightCamera.rect = new Rect(0, 0, 1, 1);

        //leftCamera.aspect *= 2;
        //rightCamera.aspect *= 2;

        // apply other configurations
        ApplyLayout();
    }

    void ApplyLayout()
    {
        // eye separation
        var halfSeparation = separation / 2.0f;
        leftCamera.transform.localPosition = new Vector3(-separation, 0.0f, 0.0f);
        rightCamera.transform.localPosition = new Vector3(separation, 0.0f, 0.0f);

        // convergence
        var angle = 90.0f - Mathf.Atan2(convergence, halfSeparation) * Mathf.Rad2Deg;
        leftCamera.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
        rightCamera.transform.localRotation = Quaternion.AngleAxis(-angle, Vector3.up);

        // keep values
        previousSeparation = separation;
        previousConvergence = convergence;
    }
}