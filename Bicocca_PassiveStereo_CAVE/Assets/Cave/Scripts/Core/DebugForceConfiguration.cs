using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugForceConfiguration : MonoBehaviour
{
    [Header("User Interface Components")]
    [SerializeField] private Button frontWallButton;
    [SerializeField] private Button rightWallButton;
    [SerializeField] private Button leftWallButton;
    [SerializeField] private Button floorWallButton;
    [Space]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [Header("Walls")]
    [SerializeField] private Wall frontWall;
    [SerializeField] private Wall rightWall;
    [SerializeField] private Wall leftWall;
    [SerializeField] private Wall floorWall;

    [Header("Network Manager")]
    [SerializeField] NetworkManager networkManager;

    private void Awake()
    {
        frontWallButton.onClick.AddListener(() => ForceWallActive(frontWall));
        rightWallButton.onClick.AddListener(() => ForceWallActive(rightWall));
        leftWallButton.onClick.AddListener(() => ForceWallActive(leftWall));
        floorWallButton.onClick.AddListener(() => ForceWallActive(floorWall));

        hostButton.onClick.AddListener(() => SetNetworkRole(NetworkRole.Host));
        clientButton.onClick.AddListener(() => SetNetworkRole(NetworkRole.Client));
    }

    private void ForceWallActive(Wall wall)
    {
        wall.ForceActive = true;
    }

    private void SetNetworkRole(NetworkRole role)
    {
        switch (role)
        {
            case NetworkRole.Host:
                networkManager.StartHost();
                Debug.Log("Starting as HOST");
                break;

            case NetworkRole.Client:
                networkManager.StartClient();
                Debug.Log("Starting as CLIENT");
                break;
        }
    }
}
