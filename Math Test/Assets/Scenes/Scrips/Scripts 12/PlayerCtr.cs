using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtr : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody playerRb;
    private Vector3 moveInput;

    [Header("Ziggs Bomb (F Key)")]
    public GameObject bombPrefab;
    public Transform firePoint;
    public float forwardForce = 10f;
    public float upwardForce = 8f;

    [Header("Mine Settings (G Key)")]
    public GameObject minePrefab;       // 새로 만들 지뢰 프리팹
    public float mineSpawnDistance = 3f;// 플레이어 정면 얼마나 먼 곳에 생성할지

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 이동 입력 처리 (Both 설정 기준)
        float h = 0;
        float v = 0;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) v = 1f;
            if (Keyboard.current.sKey.isPressed) v = -1f;
            if (Keyboard.current.aKey.isPressed) h = -1f;
            if (Keyboard.current.dKey.isPressed) h = 1f;
        }
        moveInput = new Vector3(h, 0f, v).normalized;

        // F 키 : 기존 직스 폭탄 발사
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            LaunchZiggsBomb();
        }

        // 💡 G 키 : 플레이어 정면 일정 거리에 지뢰 설치
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            SpawnMine();
        }
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 moveVelocity = moveInput * moveSpeed;
            playerRb.linearVelocity = new Vector3(moveVelocity.x, playerRb.linearVelocity.y, moveVelocity.z);

            Quaternion newRotation = Quaternion.LookRotation(moveInput);
            playerRb.MoveRotation(Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 15f));
        }
        else
        {
            playerRb.linearVelocity = new Vector3(0f, playerRb.linearVelocity.y, 0f);
        }
    }

    void LaunchZiggsBomb()
    {
        if (bombPrefab == null) return;
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + (transform.forward * 1.5f) + (Vector3.up * 0.5f);
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();
        if (bombRb != null)
        {
            Vector3 launchDirection = (transform.forward * forwardForce) + (Vector3.up * upwardForce);
            bombRb.linearVelocity = launchDirection;
        }
    }

    // 💡 지뢰 생성 함수
    void SpawnMine()
    {
        if (minePrefab == null) return;

        // 플레이어가 보고 있는 방향(transform.forward)으로 설정한 거리만큼 떨어진 바닥 높이에 생성
        Vector3 spawnPos = transform.position + (transform.forward * mineSpawnDistance);
        spawnPos.y = transform.position.y; // 플레이어의 발바닥 높이 정도로 정렬

        Instantiate(minePrefab, spawnPos, Quaternion.identity);
    }

    // 💡 지뢰 폭발 시 플레이어를 밀어내기 위해 외부에 노출하는 함수
    public void AddExplosionImpulse(Vector3 impulse)
    {
        if (playerRb != null)
        {
            // 순간적으로 폭발 힘을 플레이어 Rigidbody에 누적
            playerRb.AddForce(impulse, ForceMode.Impulse);
        }
    }
}