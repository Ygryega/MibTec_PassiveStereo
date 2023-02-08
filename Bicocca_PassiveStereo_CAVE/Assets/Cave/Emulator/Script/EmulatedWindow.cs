using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmulatedWindow : MonoBehaviour
{
    [SerializeField] private RenderTexture m_RenderTexture;
    [SerializeField] private Camera m_Camera;

    public RenderTexture RenderTexture
    {
        get
        {
            return m_RenderTexture;
        }
        set
        {
            m_RenderTexture = value;
        }
    }

    private void Start()
    {
        if (m_Camera != null)
        {
            // Debug.Log($"Setted {m_Camera.gameObject.name} render to RenderTexture");
            m_Camera.targetTexture = m_RenderTexture;
        }
    }
}

