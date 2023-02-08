using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppNetworkBehaviour : NetworkBehaviour
{
    [SerializeField] private int playersRequired;

    private int playersConnected = 0;
    private int playerCount = 0;

    private ApplicationController appController;

    private void Start()
    {
        appController = ApplicationController.Instance;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isServer)
        {
            RPC_Quit();
        }

        CheckClients();
    }

    [ClientRpc]
    public void RPC_Quit()
    {
        Application.Quit();
    }

    [ServerCallback]
    private void CheckClients()
    {
        if(isServer)
        {
            playersConnected = NetworkServer.connections.Count;
        }

        if(playerCount != playersConnected)
        {
            Debug.Log("Number of players connected = " + playersConnected);
            playerCount = playersConnected;
        }

        if (playersConnected == playersRequired)
        {
            appController.StartVideo = true;
        }
        else
        {
            appController.StartVideo = false;
        }
    }
}
