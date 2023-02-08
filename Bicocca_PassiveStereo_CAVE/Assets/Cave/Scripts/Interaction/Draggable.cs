using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(PointerEvents))]
public class Draggable : NetworkBehaviour
{
    [SerializeField] private bool isDraggable;
    [SerializeField] private bool isDragging;

    [Header("Appearence")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color draggingColor;

    private PointerEvents pointerEvents;
    private ParentConstraint constraint;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        pointerEvents = GetComponent<PointerEvents>();
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = defaultColor;
        }
    }

    private void OnDestroy()
    {
        pointerEvents.OnDown.RemoveListener(StartDragging);
        pointerEvents.OnUp.RemoveListener(StopDragging);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isClientOnly && rb != null)
        {
            // Destroy Rigidbody component on Client side
            Destroy(rb);
        }

        if (isServer)
        {
            pointerEvents.OnEnter.AddListener(RpcSetHighlightColor);
            pointerEvents.OnExit.AddListener(RpcSetDefaultColor);
            pointerEvents.OnDown.AddListener(StartDragging);
            pointerEvents.OnUp.AddListener(StopDragging);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isServer)
        {
            pointerEvents.OnEnter.RemoveListener(RpcSetHighlightColor);
            pointerEvents.OnExit.RemoveListener(RpcSetDefaultColor);
            pointerEvents.OnDown.RemoveListener(StartDragging);
            pointerEvents.OnUp.RemoveListener(StopDragging);
        }
    }

    [ServerCallback]
    private void StartDragging()
    {
        if (isDraggable == false)
        { return; }

        isDragging = true;
        RpcSetDraggingColor();

        Transform pointerSource = pointerEvents.PointerSource;
        constraint = GetComponent<ParentConstraint>();
       
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (constraint == null)
        {
            constraint = gameObject.AddComponent<ParentConstraint>();
        }

        constraint.constraintActive = true;

        Vector3 localOffset = pointerSource.InverseTransformPoint(transform.position);
        Quaternion localRotation = Quaternion.Inverse(pointerSource.rotation) * transform.rotation;

        if (constraint.sourceCount == 0)
        {
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = pointerSource;
            source.weight = 1.0f;

            constraint.AddSource(source);
        }

        constraint.SetTranslationOffset(0, localOffset);
        constraint.SetRotationOffset(0, localRotation.eulerAngles);
    }

    [ServerCallback]
    private void StopDragging()
    {
        if (isDraggable == false)
        { return; }

        RpcSetDefaultColor();

        constraint.constraintActive = false;
        Destroy(constraint);

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        isDragging = false;
    }

    [ClientRpc]
    private void RpcSetHighlightColor()
    {
        meshRenderer.material.color = highlightColor;
    }

    [ClientRpc]
    private void RpcSetDefaultColor()
    {
        meshRenderer.material.color = defaultColor;
    }

    [ClientRpc]
    private void RpcSetDraggingColor()
    {
        meshRenderer.material.color = draggingColor;
    }
}
