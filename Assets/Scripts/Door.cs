using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Door Object")]
    [Tooltip("The actual door mesh that should rotate open/closed.")]
    public Transform doorToRotate;

    [Header("Door Settings")]
    public float openAngle = 90f;      // How far to open (use -90 if wrong way)
    public float openSpeed = 2f;       // Higher = faster

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    bool isOpen = false;
    bool isMoving = false;
    bool playerInRange = false;

    Quaternion closedRot;
    Quaternion openRot;

    void Start()
    {
        // Fallback so it doesn't crash if you forget to assign it
        if (doorToRotate == null)
        {
            doorToRotate = transform;
        }

        closedRot = doorToRotate.rotation;
        openRot = Quaternion.Euler(
            doorToRotate.eulerAngles + new Vector3(0f, openAngle, 0f)
        );
    }

    void Update()
    {
        // Only interact when player is inside trigger and door isn't moving
        if (!playerInRange || isMoving) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (isOpen)
                StartCoroutine(RotateDoor(openRot, closedRot));   // close
            else
                StartCoroutine(RotateDoor(closedRot, openRot));   // open

            isOpen = !isOpen;
        }
    }

    IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        isMoving = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorToRotate.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        doorToRotate.rotation = to;
        isMoving = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
