using Mirror;
using System.Collections;
using System.Collections.Generic;
using UniCAVE;
using UnityEngine;

public class ClusterNode : NetworkBehaviour
{
    [Header("Cluster Actor Components")]
    public Camera camera;

    [SyncVar]
    int playerNo;

    [SyncVar(hook = nameof(OnEyeSeparationChanged))]
    float eyeSeparation;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Set SyncVar values
        playerNo = connectionToClient.connectionId;
        eyeSeparation = 0.067f;

        Debug.Log($"[SERVER] OnStart! Setting EyeSeparation:{eyeSeparation}");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //Debug.Log($"[CLIENT] OnStart! -> Apply eye separation value... {eyeSeparation.ToString("F3")}");
    }

    #region SyncVar Hooks
    private void OnEyeSeparationChanged(float oldEyeSeparation, float newEyeSeparation)
    {
        // Show the data in the UI
        //Debug.Log($"[CLIENT] On Eye separation change: {newEyeSeparation.ToString("F3")}");


        if (camera != null)
        {
            // Apply Eye Separation to local Camera
            camera.stereoSeparation = newEyeSeparation;
        }
    }


    private void Update()
    {
        GetInputOnServer();
    }

    [ServerCallback]
    private void GetInputOnServer()
    {
        //if (Input.GetKey(KeyCode.F1))
        //{
        //    eyeSeparation += 0.1f * Time.deltaTime;
        //}

        //if (Input.GetKey(KeyCode.F2))
        //{
        //    eyeSeparation -= 0.1f * Time.deltaTime;
        //}
    }
    #endregion
}