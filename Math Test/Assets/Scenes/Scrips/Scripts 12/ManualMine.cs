using UnityEngine;

public class ManualMine : MonoBehaviour
{
    [Header("Mine Core Settings")]
    public float duration = 2.5f;     // 폭발 지연 시간
    public float radius = 6f;         // 폭발 반경
    public float maxForce = 25f;      // 최대 폭발력 (수동 연산이므로 수치 조절 필요)
    public float upwardsModifier = 0.5f; // 위로 띄워주는 보정 값

    private bool hasExploded = false;

    void Start()
    {
        // 생성된 후 지정한 시간(duration) 뒤에 폭발
        Invoke("Explode", duration);
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 minePos = transform.position;

        // 폭발 반경 내의 모든 콜라이더 수집
        Collider[] colliders = Physics.OverlapSphere(minePos, radius);

        foreach (var col in colliders)
        {
            if (col.gameObject == this.gameObject) continue; // 자신 제외

            // 1. 만약 감지된 콜라이더가 플레이어(PlayerCtr)라면?
            PlayerController player = col.GetComponent<PlayerController>();
            if (player == null) player = col.GetComponentInParent<PlayerController>();
            // 클래스명이 'PlayerCtr'인지 확인해보고 환경에 맞춰 유연하게 연동 가능하도록 예외처리
            var playerCtr = col.GetComponent<PlayerCtr>() ?? col.GetComponentInParent<PlayerCtr>();

            if (playerCtr != null)
            {
                // 플레이어와 지뢰 사이의 수동 물리 계산
                Vector3 toPlayer = col.transform.position - minePos;
                float distance = toPlayer.magnitude;

                if (distance <= radius)
                {
                    Vector3 dir = toPlayer.normalized;
                    // 거리가 가까울수록 강하고, 멀수록 약해지는 감쇄공식
                    float attenuation = 1f - Mathf.Clamp01(distance / radius);

                    // 약간 위로 솟구치게 방향 보정
                    dir += Vector3.up * upwardsModifier;
                    dir = dir.normalized;

                    Vector3 forceVector = dir * maxForce * attenuation;

                    // 플레이어 스크립트에 힘 주입!
                    playerCtr.AddExplosionImpulse(forceVector);
                }
                continue; // 플레이어 처리 끝났으니 패스
            }

            // 2. 플레이어가 아닌 일반 적이나 물리 오브젝트들 처리
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 toTarget = rb.position - minePos;
                float distance = toTarget.magnitude;

                if (distance <= radius)
                {
                    Vector3 dir = toTarget.normalized;
                    float attenuation = 1f - Mathf.Clamp01(distance / radius);

                    dir += Vector3.up * upwardsModifier;
                    dir = dir.normalized;

                    Vector3 impulse = dir * maxForce * attenuation;
                    rb.AddForce(impulse, ForceMode.Impulse);
                }
            }
        }

        // 폭발 효과 연출 후 지뢰 오브젝트 제거
        Destroy(gameObject);
    }

    // 범위 가시화 기즈모
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}