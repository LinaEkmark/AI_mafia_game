using UnityEngine;

public class DonutPlate : MonoBehaviour
{
    public GameObject panel;
    //private bool messageShown = false;    // Ensures it only shows once

    void Start()
    {
        if (panel != null)
            panel.SetActive(false); // ensure panel is hidden at start
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the donut plate area.");
            panel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exited by: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the donut plate area.");
            panel.SetActive(false);
        }
    }

}
