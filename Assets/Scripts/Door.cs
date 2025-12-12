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

    public bool isLocked = false;
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
        if (isLocked) return;

        // Only interact when player is inside trigger and door isn't moving
        if (!playerInRange || isMoving) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (isOpen)
                StartCoroutine(RotateDoor(openRot, closedRot));   // close
            else
                StartCoroutine(RotateDoor(closedRot, openRot));   // open

            isOpen = !isOpen;

            // Update the prompt while still in range
            if (playerInRange)
            {
                string prompt = isOpen ? "Close" : "Open";
                InteractionManager.Instance?.RequestPrompt(prompt, InteractionType.Door);
            }
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

    public void UnlockDoor()
    {
        Debug.Log("Door unlocked!");
        isLocked = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerInRange = true;

            if (isLocked)
            {
                UIManager.Instance.HideKey();
                InteractionManager.Instance?.RequestPrompt("Locked", InteractionType.Door);
                return;
            }

            string prompt = isOpen ? "Close" : "Open";
            InteractionManager.Instance?.RequestPrompt(prompt, InteractionType.Door);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerInRange = false;

            if (isLocked)
            {
                UIManager.Instance.ShowKey();
            }
            // Only clears if Door currently owns the prompt
            InteractionManager.Instance?.ClearPrompt(InteractionType.Door);
        }
    }
}
