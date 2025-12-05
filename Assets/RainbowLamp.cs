using UnityEngine;

[RequireComponent(typeof(Light))]
public class RainbowLamp : MonoBehaviour
{
    public float speed = 1f;   // How fast the colors cycle

    Light lamp;

    void Start()
    {
        lamp = GetComponent<Light>();
    }

    void Update()
    {
        // Hue goes from 0..1 over time, wraps automatically
        float h = Mathf.PingPong(Time.time * speed, 1f);
        Color rainbow = Color.HSVToRGB(h, 1f, 1f);
        lamp.color = rainbow;
    }
}
