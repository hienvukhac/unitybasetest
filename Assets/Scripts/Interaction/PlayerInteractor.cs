using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("Cài đặt quét tương tác")]
    [SerializeField] private float interactRadius = 2.8f;
    [SerializeField] private LayerMask interactableLayer = ~0;

    [Header("UI Prompt")]
    [SerializeField] private GameObject sharedPromptUI;
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 1.6f, 0f);

    private PlayerInput playerInput;
    private IInteractable currentInteractable;
    private Collider[] hitColliders = new Collider[64];

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (sharedPromptUI != null)
        {
            sharedPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
        FindNearestInteractable();
        UpdatePromptPosition();
        HandleInteractionInput();
    }

    private void FindNearestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactRadius,
            hitColliders,
            interactableLayer,
            QueryTriggerInteraction.Collide
        );

        IInteractable nearest = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider col = hitColliders[i];
            if (col == null) continue;

            if (col.transform.root == this.transform.root) continue;

            IInteractable interactable = col.GetComponent<IInteractable>() 
                                      ?? col.GetComponentInParent<IInteractable>()
                                      ?? col.GetComponentInChildren<IInteractable>();

            if (interactable != null && interactable.CanInteract)
            {
                Vector3 closestPoint = (col is MeshCollider mc && !mc.convex)
                    ? col.bounds.ClosestPoint(transform.position)
                    : col.ClosestPoint(transform.position);

                float dist = Vector3.Distance(transform.position, closestPoint);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = interactable;
                }
            }
        }

        currentInteractable = nearest;
    }

    private void UpdatePromptPosition()
    {
        if (sharedPromptUI == null) return;

        if (currentInteractable != null && currentInteractable.CanInteract)
        {
            if (!sharedPromptUI.activeSelf)
            {
                sharedPromptUI.SetActive(true);
            }

            Vector3 basePos;
            if (currentInteractable is MonoBehaviour mb)
            {
                Collider col = mb.GetComponent<Collider>() ?? mb.GetComponentInChildren<Collider>();
                basePos = col != null ? col.bounds.center : mb.transform.position;
            }
            else
            {
                basePos = currentInteractable.PromptTransform != null ? currentInteractable.PromptTransform.position : transform.position;
            }

            sharedPromptUI.transform.position = basePos + promptOffset;
        }
        else
        {
            if (sharedPromptUI.activeSelf)
            {
                sharedPromptUI.SetActive(false);
            }
        }
    }

    private void HandleInteractionInput()
    {
        if (currentInteractable != null && playerInput.IsInteractButtonPressed)
        {
            IInteractable target = currentInteractable;
            target.Interact();

            if (!target.CanInteract && sharedPromptUI != null)
            {
                sharedPromptUI.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
#endif
}
