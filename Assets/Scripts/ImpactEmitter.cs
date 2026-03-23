using UnityEngine;

public class ImpactEmitter : MonoBehaviour
{
    public SurfaceEffectData resourceEffect;
    public SurfaceEffectData creatureEffect;
    public GameObject simpleResource;
    public GameObject simpleCreature;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        if (collision.gameObject.CompareTag("Recurso"))
        {
            if (resourceEffect != null) GameEvents.TriggerSurfaceImpact(contact.point, contact.normal, resourceEffect);
            else if (simpleResource != null) GameEvents.TriggerSimpleImpact(contact.point, contact.normal, simpleResource);
        }
        else if (collision.gameObject.CompareTag("Criatura"))
        {
            if (creatureEffect != null) GameEvents.TriggerSurfaceImpact(contact.point, contact.normal, creatureEffect);
            else if (simpleCreature != null) GameEvents.TriggerSimpleImpact(contact.point, contact.normal, simpleCreature);
        }
    }
}
