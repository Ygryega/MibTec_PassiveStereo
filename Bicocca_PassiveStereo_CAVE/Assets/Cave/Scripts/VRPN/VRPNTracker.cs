using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniCAVE;
using UnityEngine;
using UnityEngine.Diagnostics;

public class VRPNTracker : MonoBehaviour
{
    [SerializeField] private string trackerAddress = "Isense900@C6_V1_HEAD";
    [SerializeField] private int channel = 0;
    [SerializeField] private bool trackPosition = true;
    [SerializeField] private bool trackRotation = true;

    public Vector3 trackerPositionOffset;
    public Vector3 trackerRotationOffset;

    public bool debugOutput = false;

    public bool convertToLeft = false;

    [Header("Emulator")]
    [GUIColor(1,0,0)]
    [SerializeField] private bool ignoreMachineName = false;

    [SerializeField] private bool enableKeyboardControls = false;
    [SerializeField] private bool emulateTracking = false;
    [SerializeField] private EmulatorTrackingController emulatorController;

    [SerializeField] private KeyCode moveUp;
    [SerializeField] private KeyCode moveDown;
    [SerializeField] private KeyCode moveLeft;
    [SerializeField] private KeyCode moveRight;
    [SerializeField] private KeyCode moveForward;
    [SerializeField] private KeyCode moveBack;
    [SerializeField] private KeyCode reset;

    public int Channel
    {
        get { return channel; }
        set
        {
            channel = value;
        }
    }

    public bool TrackPosition
    {
        get { return trackPosition; }
        set
        {
            trackPosition = value;
            StopCoroutine("Position");
            if (trackPosition && Application.isPlaying)
            {
                StartCoroutine("Position");
            }
        }
    }

    public bool TrackRotation
    {
        get { return trackRotation; }
        set
        {
            trackRotation = value;
            StopCoroutine("Rotation");
            if (trackRotation && Application.isPlaying)
            {
                StartCoroutine("Rotation");
            }
        }
    }

    #region MONO_BEHAVIOUR
    private void Start()
    {
        if ((ClusterConfig.Instance.HeadMachine.Name != ClusterConfig.GetMachineName()) && !ignoreMachineName)
        {
            Debug.Log($"<b>[VRPN {gameObject.name}]</b>" % Colorize.Yellow + " Tracking DISABLED [X] -> This machine is running as <b>CLIENT</b>");

            Debug.Assert(ClusterConfig.Instance.HeadMachine.Name == Util.GetMachineName(), "Wrong head machine name");
            Destroy(this);
        }
        else
        {
            Debug.Log($"<b>[VRPN {gameObject.name}]</b>" % Colorize.Yellow + " Tracking ENABLED [✓] -> This machine is running as <b>HOST</b>");
        }

        if (emulateTracking)
        {
            var emulator = FindObjectOfType<EmulatorTrackingController>();

            if (emulator != null)
            {
                Debug.Log($"<b>[VRPN {gameObject.name}]</b>" % Colorize.Yellow + " Tracking DISABLED [X] and overrided by EMULATOR!");
            }
            else
            {
                emulateTracking = false;
                Debug.Log($"<b>[VRPN {gameObject.name}]</b>" % Colorize.Yellow + " Emulator isn't setup correctly, using VRPN tracking");
            }
        }

        if (trackPosition && !enableKeyboardControls)
        {
            StartCoroutine(Position());
        }

        if (trackRotation && !enableKeyboardControls)
        {
            StartCoroutine(Rotation());
        }
    }

    private void Update()
    {
        if (enableKeyboardControls)
        {
            if (Input.GetKey(moveLeft))
            {
                Vector3 position = transform.position;
                position.x-= 0.01f ;
                transform.position = position;
            }

            if (Input.GetKey(moveRight))
            {
                Vector3 position = transform.position;
                position.x += 0.01f ;
                transform.position = position;
            }

            if (Input.GetKey(moveUp))
            {
                Vector3 position = transform.position;
                position.y += 0.01f;
                transform.position = position;
            }

            if (Input.GetKey(moveDown))
            {
                Vector3 position = transform.position;
                position.y -= 0.01f;
                transform.position = position;
            }

            if (Input.GetKey(moveForward))
            {
                Vector3 position = transform.position;
                position.z += 0.01f;
                transform.position = position;
            }

            if (Input.GetKey(moveBack))
            {
                Vector3 position = transform.position;
                position.z -= 0.01f;
                transform.position = position;
            }

            if (Input.GetKeyDown(reset))
            {
                ResetTransform();
            }
        }
    }

    #endregion

    private IEnumerator Position()
    {
        while (true)
        {
            if (emulateTracking)
            {
                if (emulatorController.Pov != null)
                {
                    transform.localPosition = emulatorController.Pov.localPosition;
                }
            }
            else
            {

                Vector3 pos = VRPN.vrpnTrackerPos(trackerAddress, channel) + trackerPositionOffset;

                if (convertToLeft)
                {
                    pos.x = Interlocked.Exchange(ref pos.z, pos.x);
                    pos.y *= -1;

                    transform.localPosition = pos;
                }
                else
                {
					// Old Coordinate System
					var new_Z = pos.y;
					var new_Y = pos.x;
					var new_X = pos.z;

					// TODO : Expose in Inspector a conversion tool
					// Made Competence Center coordinate systems
					//var new_X = - pos.y;
     //               var new_Y = pos.x;
     //               var new_Z = pos.z;

                    transform.localPosition = new Vector3(new_X, new_Y, new_Z);
                }
            }

            yield return null;
        }
    }
    private IEnumerator Rotation()
    {
        while (true)
        {
            if (emulateTracking)
            {
                if (emulatorController.Pov != null)
                {
                    transform.localRotation = emulatorController.Pov.localRotation;
                }
            }
            else
            {
                Quaternion rotation = VRPN.vrpnTrackerQuat(trackerAddress, channel);

                if (convertToLeft)
                {
                    // rotation.x = Interlocked.Exchange(ref rotation.z, rotation.x);
                    // rotation.y *= -1;
                    // transform.localRotation = rotation * Quaternion.Euler(trackerRotationOffset);
                }
                else
                {
					rotation.x = Interlocked.Exchange(ref rotation.y, rotation.x);
					rotation.z = -rotation.y;
                    rotation.y = rotation.x;
					//rotation.x = Interlocked.Exchange(ref rotation.y, rotation.x);
					//               rotation.x *= -1;
					transform.localRotation = rotation * Quaternion.Euler(trackerRotationOffset);
                }
            }

            yield return null;
        }
    }

    public void ResetTransform()
    {
        transform.localPosition = Vector3.zero;
    }
}
