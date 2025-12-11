using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Header("Holder")]
    public Transform holdPoint; // typically a child transform representing the hand

    public Pickupable HeldItem { get; private set; }

    public bool HasItem => HeldItem != null;

    public void PickUp(Pickupable item)
    {
        if (item == null) return;

        if (HasItem)
        {
            Drop();
        }

        HeldItem = item;
        if (holdPoint != null)
            HeldItem.OnPickUp(holdPoint);
        else
            HeldItem.OnPickUp(transform);
    }

    public void Drop(Vector3 force = default)
    {
        if (!HasItem) return;

        HeldItem.OnDrop();

        Rigidbody rb = HeldItem.GetComponent<Rigidbody>();
        if (rb != null && force != default)
        {
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        HeldItem = null;
    }
}
