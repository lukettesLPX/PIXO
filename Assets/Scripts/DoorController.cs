using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Transform doorLeaf;
    public float openAngle = 90f;
    public float interactionRadius = 2.5f;

    [Header("Pivot Adjustment")]
    public bool autoFixPivot = true;
    public Vector3 pivotOffset = new Vector3(-0.5f, 0, 0);

    private bool isOpen = false;
    private bool isPlayerInRange = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine currentRotationCoroutine;
    private float smoothSpeed = 5f;
    private Transform hinge;

    private void Start()
    {
        if (doorLeaf == null) doorLeaf = transform;

        if (autoFixPivot)
        {
            // Fix pivot lateral si el original esta centrado
            GameObject hingeGO = new GameObject(doorLeaf.name + "_Hinge");
            hinge = hingeGO.transform;
            hinge.SetParent(doorLeaf.parent);
            
            hinge.localPosition = doorLeaf.localPosition + doorLeaf.localRotation * pivotOffset;
            hinge.localRotation = doorLeaf.localRotation;
            hinge.localScale = doorLeaf.localScale;

            doorLeaf.SetParent(hinge);
            doorLeaf.localPosition = -pivotOffset;
            doorLeaf.localRotation = Quaternion.identity;
            doorLeaf.localScale = Vector3.one;
        }
        else
        {
            hinge = doorLeaf;
        }

        closedRotation = hinge.localRotation;
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
        if (hinge == null) yield break;
        while (Quaternion.Angle(hinge.localRotation, targetRotation) > 0.05f)
        {
            hinge.localRotation = Quaternion.Slerp(hinge.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
            yield return null;
        }
        hinge.localRotation = targetRotation;
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
