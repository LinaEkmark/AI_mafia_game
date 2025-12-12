using UnityEngine;

public class DestructionZone : MonoBehaviour
{
    [Header("Objects to Destroy")]
    public GameObject objectToDestroy1;
    public GameObject objectToDestroy2;
    public GameObject objectToDestroy3;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the zone
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // Destroy the assigned objects
            if (objectToDestroy1 != null)
                Destroy(objectToDestroy1);
            
            if (objectToDestroy2 != null)
                Destroy(objectToDestroy2);
            
            if (objectToDestroy3 != null)
                Destroy(objectToDestroy3);

            Debug.Log("Objects destroyed by DestructionZone!");
        }
    }
}
