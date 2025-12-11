using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupController : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference interactAction; // assign to Interact from InputSystem
    [Tooltip("Optional: assign an ItemHolder on a child object (e.g. HandPoint). If empty, the script will look for an ItemHolder on the same GameObject.")]
    public ItemHolder externalItemHolder;

    [Header("Detection")]
    public Transform detectionOrigin; // e.g., player camera or hand
    public float detectionRadius = 1.2f;
    public LayerMask detectionMask = ~0;

    ItemHolder holder;

    void Awake()
    {
        holder = externalItemHolder != null ? externalItemHolder : GetComponent<ItemHolder>();
        if (detectionOrigin == null)
            detectionOrigin = transform;
    }

    void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
            interactAction.action.performed += OnInteractPerformed;
    }

    void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
            interactAction.action.performed -= OnInteractPerformed;
    }

    void Update()
    {
        // Fallback to keyboard E if no InputAction assigned
        if ((interactAction == null || interactAction.action == null))
        {
            bool pressed = false;

            // Try new Input System's keyboard if available
            if (Keyboard.current != null)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                    pressed = true;
            }

            // Also support legacy Input system fallback
            if (Input.GetKeyDown(KeyCode.E))
                pressed = true;

            if (pressed)
                TryTogglePickup();
        }

        UpdateInteractionPrompt();
    }

    void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        TryTogglePickup();
    }

    void TryTogglePickup()
    {
        if (holder.HasItem)
        {
            holder.Drop();
            // Let InteractionManager handle prompt clearing / updating
            InteractionManager.Instance?.ClearPrompt(InteractionType.Pickup);
            return;
        }

        // find nearest Pickupable
        Pickupable nearest = FindNearestPickup();

        if (nearest != null)
        {
            holder.PickUp(nearest);
            InteractionManager.Instance?.ClearPrompt(InteractionType.Pickup);
        }
    }

    Pickupable FindNearestPickup()
    {
        Collider[] cols = Physics.OverlapSphere(detectionOrigin.position, detectionRadius, detectionMask);
        Pickupable nearest = null;
        float best = float.MaxValue;

        foreach (var c in cols)
        {
            var p = c.GetComponentInParent<Pickupable>();
            if (p == null) continue;

            float d = Vector3.Distance(detectionOrigin.position, p.transform.position);
            if (d < best)
            {
                best = d;
                nearest = p;
            }
        }

        return nearest;
    }

    void UpdateInteractionPrompt()
    {
        // If we’re holding an item, request a pickup-level prompt
        if (holder.HasItem)
        {
            InteractionManager.Instance?.RequestPrompt(
                "Drop ",
                InteractionType.Pickup
            );
            return;
        }

        // Not holding: check if there is something to pick up
        Pickupable nearest = FindNearestPickup();

        if (nearest != null)
        {
            InteractionManager.Instance?.RequestPrompt(
                "Pick up ",
                InteractionType.Pickup
            );
        }
        else
        {
            // Now safe to clear our own prompt:
            // This will only clear if the current priority == Pickup.
            // If an NPC (higher priority) owns it, this does nothing.
            InteractionManager.Instance?.ClearPrompt(InteractionType.Pickup);
        }
    }


    void OnDrawGizmosSelected()
    {
        var origin = (detectionOrigin != null) ? detectionOrigin.position : transform.position;
        Gizmos.color = new Color(0.1f, 0.6f, 0.1f, 0.4f);
        Gizmos.DrawSphere(origin, detectionRadius);
    }
}
