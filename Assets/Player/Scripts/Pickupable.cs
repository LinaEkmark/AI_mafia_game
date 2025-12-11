using UnityEngine;

public class Pickupable : MonoBehaviour
{
    // Optional offset when held
    public Vector3 heldLocalPosition = Vector3.zero;
    public Vector3 heldLocalEuler = Vector3.zero;

    // Whether physics should be disabled while held
    public bool disablePhysicsWhileHeld = true;
    // Whether to make colliders triggers while held so they won't physically collide with the player
    public bool makeCollidersTriggerWhileHeld = true;

    Rigidbody rb;
    Collider[] colliders;
    bool[] previousIsTrigger;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called when object is picked up by the player
    public void OnPickUp(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = heldLocalPosition;
        transform.localEulerAngles = heldLocalEuler;

        if (disablePhysicsWhileHeld && rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (makeCollidersTriggerWhileHeld)
        {
            colliders = GetComponentsInChildren<Collider>();
            if (colliders != null && colliders.Length > 0)
            {
                previousIsTrigger = new bool[colliders.Length];
                for (int i = 0; i < colliders.Length; i++)
                {
                    previousIsTrigger[i] = colliders[i].isTrigger;
                    colliders[i].isTrigger = true;
                }
            }
        }
    }

    // Called when object is dropped
    public void OnDrop()
    {
        transform.SetParent(null, true);

        if (disablePhysicsWhileHeld && rb != null)
        {
            rb.isKinematic = false;
        }

        if (makeCollidersTriggerWhileHeld && colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == null) continue;
                colliders[i].isTrigger = (previousIsTrigger != null && i < previousIsTrigger.Length) ? previousIsTrigger[i] : false;
            }
            colliders = null;
            previousIsTrigger = null;
        }
    }
}
