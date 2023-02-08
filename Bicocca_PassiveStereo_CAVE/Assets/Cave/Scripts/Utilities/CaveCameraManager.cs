using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCameraManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] Wall wall;

    Camera _cam;

    private void Start()
    {
        _cam = GetComponent<Camera>();
    }
    void OnEnable()
    {
        _cam = GetComponent<Camera>();

        if (Application.isPlaying)
        {
            if (wall.ShouldBeActive == false)
            {
                _cam.enabled = false;
                Debug.Log($"[ ! ] Disabling {_cam.gameObject.name % Colorize.Yellow} on this machine");

                Destroy(this.gameObject);
                return;
            }
        }
    }

}
