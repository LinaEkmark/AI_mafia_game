using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;      // Angle when fully open
    public float closedAngle = 0f;     // Angle when closed
    public float openSpeed = 2f;       // How fast the door moves

    bool isOpen = false;
    float targetAngle;

    void Start()
    {
        targetAngle = closedAngle;
    }

    void Update()
    {
        // Smoothly rotate towards target angle
        Quaternion current = transform.localRotation;
        Quaternion target = Quaternion.Euler(0f, targetAngle, 0f);
        transform.localRotation = Quaternion.Slerp(
            current, target, Time.deltaTime * openSpeed
        );
    }

    public void Open()
    {
        isOpen = true;
        targetAngle = openAngle;
    }

    public void Close()
    {
        isOpen = false;
        targetAngle = closedAngle;
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }
}
