using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController door;   // Assign in Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Open();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Close();
        }
    }
}
