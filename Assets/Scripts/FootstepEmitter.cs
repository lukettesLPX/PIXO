using UnityEngine;

public class FootstepEmitter : MonoBehaviour
{
    public SurfaceEffectData footstepEffect;
    public GameObject simpleEffect;
    public bool isAutomatic = true;
    public float stepDistance = 1.5f;

    private Vector3 lastPosition;
    private float distanceMoved;
    private CharacterController controller;

    private void Start()
    {
        lastPosition = transform.position;
        controller = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        if (isAutomatic)
        {
            if (controller != null && !controller.isGrounded) return;

            float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(lastPosition.x, 0, lastPosition.z));
            distanceMoved += dist;
            lastPosition = transform.position;

            if (distanceMoved >= stepDistance)
            {
                TriggerFootstep();
                distanceMoved = 0f;
            }
        }
    }

    public void TriggerFootstep()
    {
        if (footstepEffect != null) GameEvents.TriggerSurfaceImpact(transform.position, Vector3.up, footstepEffect);
        else if (simpleEffect != null) GameEvents.TriggerSimpleImpact(transform.position, Vector3.up, simpleEffect);
    }
}
