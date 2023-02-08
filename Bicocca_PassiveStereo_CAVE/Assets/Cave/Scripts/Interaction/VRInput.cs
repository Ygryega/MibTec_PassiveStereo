using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInput : BaseInput
{
    [SerializeField] private Camera eventCamera = null;
    [SerializeField] private KeyCode clickButton = KeyCode.Return;

    public Transform EventCamera => eventCamera.transform;

    protected override void Awake()
    {
        GetComponent<BaseInputModule>().inputOverride = this;
    }

    public override bool GetMouseButton(int button)
    {
        return Input.GetKey(clickButton);
    }

    public override bool GetMouseButtonDown(int button)
    {
        return Input.GetKeyDown(clickButton);
    }

    public override bool GetMouseButtonUp(int button)
    {
        return Input.GetKeyUp(clickButton);
    }

    public override Vector2 mousePosition
    {
        get
        {
            return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
        }
    }

}
