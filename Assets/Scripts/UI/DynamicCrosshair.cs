using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DynamicCrosshair : MonoBehaviour
{
    [Header("Settings")]
    public float swayIntensity = 5.0f;
    public float returnSmoothing = 15.0f;
    public float maxOffset = 50.0f;

    private RectTransform rectTransform;
    private Vector2 targetPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Get mouse movement delta
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Calculate a target offset based on the move speed
        // This is "homogeneous but fast" when moving mouse
        targetPosition += mouseDelta * swayIntensity * 0.05f;

        // Clamp to prevent the dot from leaving the central area
        targetPosition.x = Mathf.Clamp(targetPosition.x, -maxOffset, maxOffset);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -maxOffset, maxOffset);

        // Smoothly bring it back to (0, 0)
        targetPosition = Vector2.Lerp(targetPosition, Vector2.zero, Time.deltaTime * returnSmoothing);

        // Apply to UI position
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = targetPosition;
        }
    }
}
