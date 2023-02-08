using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniCAVE;
using UnityEngine;

[CreateAssetMenu(fileName = "ClusterConfig", menuName = "ScriptableObjects/ClusterConfig", order = 2)]
public class ClusterConfig : ScriptableObject
{
    private static ClusterConfig instance;
    public static ClusterConfig Instance => instance;

    [SerializeField]
    private List<MachineConfig> machines = new List<MachineConfig>();


    // Return the <MachineConfig> of the running machine
    public MachineConfig RunningMachine
    {
        get => machines.FirstOrDefault(s => s.Name == GetMachineName());
    }

    // Return HEAD (Host) <MachineConfig>
    public MachineConfig HeadMachine
    {
        get => machines.FirstOrDefault(s => s.NetworkRole == NetworkRole.Host);
    }

    // Return all CLIENTS <MachineConfig>
    public List<MachineConfig> ClientMachines
    {
        get => machines.Where(s => s.NetworkRole == NetworkRole.Client).ToList<MachineConfig>();
    }

    // Return all <MachineConfig>
    public List<MachineConfig> Machines
    {
        get => machines;
    }

    public bool IsCurrentMachine(NetworkRole role)
    {
        return machines.Any(s => s.NetworkRole == role && s.Name == GetMachineName());
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<ClusterConfig>(string.Empty)[0];
        Debug.Log($"Loaded {nameof(ClusterConfig)} instance");
    }

    public static string GetMachineName()
    {
        return System.Environment.MachineName;
    }
}
