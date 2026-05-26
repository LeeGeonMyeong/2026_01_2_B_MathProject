using UnityEngine;

public class ManaualExplode : MonoBehaviour
{
    public float delay = 1.5f;

    public float radius = 5f;

    public float force = 300f;

    public float upwardsModifier = 1f;

    void Start()
    {
        Invoke("Explode", delay);
    }

    void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (var col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb == null) continue;
            Vector3 toTarget = rb.position - explosionPos;
            float distance = toTarget.magnitude;
            Vector3 dir = toTarget.normalized;
            float attenuation = 1f - Mathf.Clamp01(distance / radius);
            dir += Vector3.up * upwardsModifier;
            dir = dir.normalized;
            Vector3 inpulse = dir * force * attenuation;
            rb.AddForce(inpulse, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

    void OnDrawGizmosSeclected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
