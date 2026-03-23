using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Transform doorLeaf;
    public float openAngle = 90f;
    public float interactionRadius = 2.5f;
    private bool isOpen = false;
    private bool isPlayerInRange = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine currentRotationCoroutine;
    private float smoothSpeed = 5f;

    private void Start()
    {
        if (doorLeaf == null) doorLeaf = transform;
        closedRotation = doorLeaf.localRotation;
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;
    }

    private void Update()
    {
        if (isPlayerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) ToggleDoor();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        if (currentRotationCoroutine != null) StopCoroutine(currentRotationCoroutine);
        currentRotationCoroutine = StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        if (doorLeaf == null) yield break;
        while (Quaternion.Angle(doorLeaf.localRotation, targetRotation) > 0.01f)
        {
            doorLeaf.localRotation = Quaternion.Slerp(doorLeaf.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
            yield return null;
        }
        doorLeaf.localRotation = targetRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}
