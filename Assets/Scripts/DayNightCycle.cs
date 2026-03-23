using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sunLight;
    public float dayDurationInSeconds = 120f;

    private void Update()
    {
        if (sunLight == null) return;
        float rotationSpeed = 360f / dayDurationInSeconds;
        sunLight.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        sunLight.intensity = Vector3.Dot(sunLight.transform.forward, Vector3.down) > 0 ? 1f : 0f;
    }
}
