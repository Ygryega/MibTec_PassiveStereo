using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = nameof(MachineConfig), menuName =  "ScriptableObjects/MachineConfig asset", order = 3000)]
public class MachineConfig : ScriptableObject
{
    [SerializeField]
    private string m_MachineName;

    [SerializeField]
    [LabelText("Run as")]
    [GUIColor("GetRoleColor")]
    [PropertySpace(spaceBefore:0, spaceAfter:25)]
    private NetworkRole m_Role;

    public string Name
    {
        get => m_MachineName;
        set => m_MachineName = value;
    }

    public NetworkRole NetworkRole
    {
        get => m_Role;
        set => m_Role = value;
    }

    public bool IsRunningMachine
    {
        get => string.Equals(ClusterConfig.GetMachineName(), m_MachineName);
    }

    [Button(ButtonSizes.Large)]
    public void GetLocalMachineName()
    {
        m_MachineName = System.Environment.MachineName;
    }

    private Color GetRoleColor()
    {
        if (m_Role == NetworkRole.Host)
        {
            return Color.green;
        }
        else
        {
            return Color.white;
        }
    }

}
