using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StereoRig : MonoBehaviour
{
    private const float DefaultSeparation = 0.067f;

    [SerializeField] private Transform leftEyeOrigin;
    [SerializeField] private Transform rightEyeOrigin;

    [Header("Properties")]
    [SerializeField] float separation = DefaultSeparation;

    private float previousSeparation;

    void Start()
    {
        Debug.Assert(rightEyeOrigin != null, "rightCamera can't be null!");
        Debug.Assert(leftEyeOrigin != null, "leftCamera can't be null!");

        ApplyLayout();
    }

    void Update()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        if (Input.GetKey(KeyCode.F2))
        {
            separation += 0.1f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.F1))
        {
            separation -= 0.1f * Time.deltaTime;
        }

        //if (Input.GetKeyDown(KeyCode.Keypad1))
        //{
        //    if (leftEyeOrigin != null)
        //    {
        //        leftEyeOrigin.enabled = !leftEyeOrigin.enabled;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Keypad2))
        //{
        //    if (rightEyeOrigin != null)
        //    {
        //        rightEyeOrigin.enabled = !rightEyeOrigin.enabled;
        //    }
        //}
    }

    [Button]
    private void ApplyLayout()
    {
        leftEyeOrigin.transform.localPosition = new Vector3(-separation / 2, 0.0f, 0.0f);
        rightEyeOrigin.transform.localPosition = new Vector3(separation / 2, 0.0f, 0.0f);

        previousSeparation = separation;
    }

    [Button]
    private void RestoreDefault()
    {
        separation = 0.067f;
        ApplyLayout();
    }
}