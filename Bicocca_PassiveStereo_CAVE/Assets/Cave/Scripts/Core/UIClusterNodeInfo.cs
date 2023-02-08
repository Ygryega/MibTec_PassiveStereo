using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIClusterNodeInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nodeInfoText;
    [SerializeField] TextMeshProUGUI networkInfoText;

    private CaveNetworkManager netManager;
    private Camera offlineCamera;

    private void Awake()
    {
        offlineCamera = GetComponentInChildren<Camera>();
        netManager = FindObjectOfType<CaveNetworkManager>();
        
        if (netManager != null)
        {
            netManager.OnClientStartDiscovery.AddListener(ShowMessage);
            netManager.OnClientStart.AddListener(DisableOverlay);
            netManager.OnClientStop.AddListener(EnableOverlay);
        }
    }

    private void OnDestroy()
    {
        if (netManager != null)
        {
            netManager.OnClientStartDiscovery.RemoveListener(ShowMessage);
        }
    }

    private void Start()
    {
        MachineConfig config = ClusterConfig.Instance.RunningMachine;

        if (config != null)
        {
            nodeInfoText.text = $"{config.Name} \n {config.name.ToUpper()} \n running as {config.NetworkRole}";
        }
        else
        {
            nodeInfoText.text = $"Configuration WARNING \n <b>{ClusterConfig.GetMachineName()}</b> is not present in the configuration.";
        }

        networkInfoText.text = string.Empty;
    }

    private void ShowMessage()
    {
        networkInfoText.text = $"Waiting Head Machine \n {ClusterConfig.Instance.HeadMachine.Name}";
    }

    private void DisableOverlay()
    {
        gameObject.SetActive(false);
    }

    private void EnableOverlay()
    {
        gameObject.SetActive(true);
    }
}
