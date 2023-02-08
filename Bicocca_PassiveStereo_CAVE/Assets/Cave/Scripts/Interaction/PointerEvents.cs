using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class PointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private bool isPressed = false;

    // Unity Events
    [HideInInspector] public UnityEvent OnEnter;
    [HideInInspector] public UnityEvent OnExit;

    [HideInInspector] public UnityEvent OnDown;
    [HideInInspector] public UnityEvent OnUp;
    [HideInInspector] public UnityEvent OnClick;

    private ParentConstraint constraint;
    private Transform pointerSource;

    public Transform PointerSource => pointerSource;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter.Invoke();
        // Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit.Invoke();
        // Debug.Log("Exit");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        pointerSource = (eventData.currentInputModule.inputOverride as VRInput).EventCamera;

        OnDown.Invoke();
        // Debug.Log("Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        pointerSource = null;

        OnUp.Invoke();
        // Debug.Log("Up");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke();
        // Debug.Log("Click");
    }
}
