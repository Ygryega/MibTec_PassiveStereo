using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EmulatorTrackingController : MonoBehaviour
{
    [SerializeField] private Transform localViewPointTransform;
    [SerializeField] private bool isTracking;

    [Header("Debug")]
    [SerializeField] private Vector3 positionOffset;

    private Camera emulatorCamera;

    public Transform Pov => localViewPointTransform;

    public bool IsTracking => isTracking;
 
    protected void OnTriggerEnter(Collider other)
    {
        if (emulatorCamera == null)
        {
            emulatorCamera = other.GetComponentInChildren<Camera>();

            if (emulatorCamera != null)
            {
                isTracking = true;
                // Debug.Log($"[EMULATOR] Start tracking!");
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        var camera = other.GetComponentInChildren<Camera>();

        if (camera != null && object.ReferenceEquals(camera, emulatorCamera))
        {
            // Debug.Log($"[EMULATOR] Stop tracking!");
            emulatorCamera = null;
            isTracking = false;
        }
    }

    protected void Update()
    {
        if (emulatorCamera != null)
        {
            localViewPointTransform.localPosition = positionOffset + transform.InverseTransformPoint(emulatorCamera.transform.position);
            localViewPointTransform.rotation = emulatorCamera.transform.rotation;
        }
    }
}
