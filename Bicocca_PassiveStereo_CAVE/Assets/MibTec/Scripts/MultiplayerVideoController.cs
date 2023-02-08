using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MultiplayerVideoController : NetworkBehaviour
{
    #region PRIVATE VARIABLES
    private ApplicationController appController;
    private VideoPlayer videoPlayer;
    #endregion

    #region MONOBEHAVIOUR METHODS
    void Start()
    {
        appController = ApplicationController.Instance;
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        RpcPlayVideo();
    }
    #endregion

    #region PRIVATE METHODS
    [ClientRpc]
    private void RpcPlayVideo()
    {
        if (appController.StartVideo == true)
        {
            videoPlayer.Play();
        }
    }
    #endregion
}
