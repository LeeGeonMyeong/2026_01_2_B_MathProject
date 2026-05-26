using UnityEngine;

public class CustomBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float force = 400f;
    public float radius = 5f;

    [Header("Trigger Conditions")]
    private int bounceCount = 0;
    public int maxBounces = 3;
    private bool hasExploded = false;

    // 💡 이제 Update에서 직접 이동시키지 않고 유니티 물리엔진에 맡깁니다.

    void OnCollisionEnter(Collision col)
    {
        if (hasExploded) return;

        // 조건 1: 적에게 직접 닿으면 즉시 폭발
        if (col.gameObject.CompareTag("Enemy"))
        {
            RunExplode();
            return;
        }

        // 조건 2: 땅에 닿았을 때 카운트
        if (col.gameObject.CompareTag("Ground"))
        {
            bounceCount++;
            Debug.Log($"폭탄 바운스 횟수: {bounceCount} / {maxBounces}");

            if (bounceCount >= maxBounces)
            {
                RunExplode();
            }
        }
    }

    void RunExplode()
    {
        hasExploded = true;
        Vector3 explosionPos = transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (var col in hitColliders)
        {
            if (col.gameObject == this.gameObject) continue;

            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(force, explosionPos, radius);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}