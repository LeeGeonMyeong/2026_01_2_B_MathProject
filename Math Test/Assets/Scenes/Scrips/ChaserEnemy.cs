using UnityEngine;
using UnityEngine.SceneManagement;

public class ChaserEnemy : MonoBehaviour
{
    public Transform player;

    public float rotationSpeed = 50f;
    public float detectionRange = 8f;
    public float dashSpeed = 15f;
    public float stopDistance = 1.2f;

    public float viewAngle = 60f; // 시야각

    public bool isDashing = false;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void CatchPlayer()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.Respawn();
    }

    private void Update()
    {
        if (!isDashing) //  회전 모드
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            //  플레이어 방향
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            //  내적 (시야각 판정)
            float dot = Vector3.Dot(transform.forward, dirToPlayer);

            // cos(시야각/2)
            float cosValue = Mathf.Cos(viewAngle * Mathf.Deg2Rad / 2f);

            float distance = Vector3.Distance(transform.position, player.position);

            //  "앞 60도 + 거리 안"
            if (dot > cosValue && distance <= detectionRange)
            {
                isDashing = true;
            }
        }
        else //  Dash 모드
        {
            Vector3 dir = (player.position - transform.position).normalized;

            rb.MovePosition(transform.position + dir * dashSpeed * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, player.position);

            //  플레이어 근접 시
            if (distance <= stopDistance)
            {
                CheckParry();   // 👉 먼저 패링 판정
                isDashing = false;
            }
        }
    }

    void CheckParry()
    {
        PlayerController pc = player.GetComponent<PlayerController>();

        Vector3 playerToEnemy = (transform.position - player.position).normalized;
        Vector3 playerForward = player.forward;

        Vector3 cross = Vector3.Cross(playerForward, playerToEnemy);

        bool isLeftAttack = cross.y > 0;

        if (isLeftAttack && pc.isLeftParrying)
        {
            Debug.Log("왼쪽 패링 성공!");
            Destroy(gameObject);
        }
        else if (!isLeftAttack && pc.isRightParrying)
        {
            Debug.Log("오른쪽 패링 성공!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("패링 실패 → 플레이어 사망");
            pc.Respawn();
        }
    }
}
