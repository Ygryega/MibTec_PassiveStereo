using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Wand : NetworkBehaviour
{
    [SerializeField] private float defaultLength = 2.0f;
    [SerializeField] private EventSystem eventSystem = null;
    [SerializeField] private ExtendedStandaloneInputModule inputModule = null;

    [Header("Visual")]
    [SerializeField] private Gradient defaultGradient;
    [SerializeField] private Gradient interactionGradient;

    private GameObject previousInteractive;
    private GameObject currentInteractive;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLength();
        UpdateAppearence();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isServer)
        {
            inputModule.enabled = true;
            inputModule.inputOverride.enabled = true;
        }
    }

    #region Raycasting & LineRenderer
    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, GetEnd());
    }

    private void UpdateAppearence()
    {
        if (currentInteractive != null && currentInteractive.CompareTag("Interactive") && previousInteractive != currentInteractive)
        {
            previousInteractive = currentInteractive;
            lineRenderer.colorGradient = interactionGradient;
        }
        else if (currentInteractive == null && previousInteractive != null)
        {
            previousInteractive = currentInteractive;
            lineRenderer.colorGradient = defaultGradient;
        }
    }

    private Vector3 GetEnd()
    {
        // Try to hit on collider

        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition = DefaultEnd(defaultLength);

        if (hit.collider)
        {
            // If we hit a collider, set endPosition
            endPosition = hit.point;
            currentInteractive = hit.collider.gameObject;
            return endPosition;
        }

        // Try to hit on Canvas
        float distance = GetCanvasDistance();
        endPosition = CalculateEnd(defaultLength);

        if (distance != 0.0f)
        {
            endPosition = CalculateEnd(distance);
        }

        return endPosition;
    }

    private float GetCanvasDistance()
    {
        // Get data
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = inputModule.inputOverride.mousePosition;

        // Raycast using data
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);

        // Get closest
        RaycastResult closestResult = FindFirstRaycast(results);
        float distance = closestResult.distance;

        if (closestResult.gameObject != null)
        {
            currentInteractive = closestResult.gameObject;
        }
        else
        {
            currentInteractive = null;
        }

        // Clamp
        distance = Mathf.Clamp(distance, 0.0f, defaultLength);
        return distance;
    }

    private RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            if (!result.gameObject)
            {
                continue;
            }

            return result;
        }

        return new RaycastResult();
    }

    private Vector3 CalculateEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }

    private Vector3 DefaultEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }

    private RaycastHit CreateForwardRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }

#endregion
}
