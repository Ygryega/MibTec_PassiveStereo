using Mirror;
using Mirror.LiteNetLib4Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

public class CaveNetworkManager : NetworkManager
{

    [Header("Initialization")]
    [SerializeField] private Transform clusterNodeParent;
    [SerializeField] private bool autoInitialize;
    [SerializeField] private bool overrideRole;
    [SerializeField] private NetworkRole role;

    public bool IsServer => role == NetworkRole.Host;

    [Header("Discovery")]
    [SerializeField] private bool discoveryEnabled = true;
    [SerializeField] private float discoveryInterval = 1f;
    [SerializeField] private string discoveryMsg;

    private bool _noDiscovering = true;

    public UnityEvent OnClientStart;
    public UnityEvent OnClientStop;
    public UnityEvent OnClientStartDiscovery;

    public override void Start()
    {
        base.Start();

        LogCurrentConfiguration();

        if (autoInitialize == false)
        {
            return;
        }

        if (overrideRole == false)
        {
            if (ClusterConfig.Instance.IsCurrentMachine(NetworkRole.Host))
            {
                role = NetworkRole.Host;
            }
            else if (ClusterConfig.Instance.IsCurrentMachine(NetworkRole.Client))
            {
                role = NetworkRole.Client;
            }
            else
            {
                role = NetworkRole.Unknown;
            }
        }
        else
        {
            Debug.Log($"[NETWORK] **** Forcing role [{this.role.ToString() % Colorize.Red}] ****");
        }

        StartAs(role);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab, clusterNodeParent);

        MachineConfig machine = ClusterConfig.Instance.RunningMachine;
        player.name = $"ClusterNode [{machine?.name}]";

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        OnClientStop.Invoke();

        StartAs(role);

        //Application.Quit();
    }

    private void StartAs(NetworkRole role)
    {
        switch (this.role)
        {
            case NetworkRole.Unknown:

                Debug.Log($"[NETWORK] Failed Initialization! NetowrkRole: {role.ToString() % Colorize.Yellow} -> Machine: {ClusterConfig.GetMachineName() % Colorize.Yellow} isn't listed in <b>ClusterConfig</b>");
                return;

            case NetworkRole.Host:

                StartHost();
                Debug.Log($"[NETWORK] Start as [{NetworkRole.Host.ToString() % Colorize.Yellow}]");
                break;

            case NetworkRole.Client:

                if (discoveryEnabled)
                {
                    StartCoroutine(StartDiscovery());
                }
                else
                {
                    StartClient();
                    Debug.Log($"[NETWORK] Start as [{NetworkRole.Client.ToString() % Colorize.Yellow}]");
                }
                break;
        }

    }

    private void LogCurrentConfiguration()
    {
        foreach (MachineConfig machine in ClusterConfig.Instance.Machines)
        {
            if (machine.IsRunningMachine)
            {
                Debug.Log($"---(Node)---> [{machine.Name}] will run as [{machine.NetworkRole.ToString() % Colorize.Yellow}] -> I'm this machine");
            }
            else
            {
                Debug.Log($"---(Node)---> [{machine.Name}] will run as [{machine.NetworkRole.ToString() % Colorize.Yellow}]");
            }
        }
    }

    private IEnumerator StartDiscovery()
    {
        yield return new WaitUntil(() => (!NetworkClient.isConnected && !NetworkServer.active));

        OnClientStartDiscovery.Invoke();
        Debug.Log($"[NETWORK] Start DISCOVERY [{this.role.ToString() % Colorize.Yellow}] - Sending request every {discoveryInterval} s");

        _noDiscovering = false;

        LiteNetLib4MirrorDiscovery.InitializeFinder();
        LiteNetLib4MirrorDiscovery.Singleton.onDiscoveryResponse.AddListener(OnClientDiscoveryResponse);

        while (!_noDiscovering)
        {
            LiteNetLib4MirrorDiscovery.SendDiscoveryRequest(discoveryMsg);
            yield return new WaitForSeconds(discoveryInterval);
        }

        Debug.Log($"[NETWORK] Stop DISCOVERY");
        LiteNetLib4MirrorDiscovery.Singleton.onDiscoveryResponse.RemoveListener(OnClientDiscoveryResponse);
        LiteNetLib4MirrorDiscovery.StopDiscovery();
    }

    private void OnClientDiscoveryResponse(IPEndPoint endpoint, string text)
    {
        string ip = endpoint.Address.ToString();

        NetworkManager.singleton.networkAddress = ip;
        NetworkManager.singleton.maxConnections = 2;

        LiteNetLib4MirrorTransport.Singleton.clientAddress = ip;
        LiteNetLib4MirrorTransport.Singleton.port = (ushort)endpoint.Port;
        LiteNetLib4MirrorTransport.Singleton.maxConnections = 2;

        Debug.Log($"[NETWORK] On CLIENT DISCOVERY RESPONSE! MSG: {text}- Connecting to IP: {endpoint.Address.ToString() % Colorize.Yellow} PORT: {endpoint.Port.ToString() % Colorize.Yellow}");

        NetworkManager.singleton.StartClient();
        _noDiscovering = true;
    }

    public override void OnStartClient() 
    {
        OnClientStart.Invoke();
    }
}
