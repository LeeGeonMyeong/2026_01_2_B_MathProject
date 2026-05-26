using UnityEngine;          
using UnityEngine.InputSystem;

public class PlayerCtr : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody playerRb;
    private Vector3 moveInput;
    private Vector3 lastMoveDirection = Vector3.forward;

    [Header("Bomb Shooting")]
    public GameObject bombPrefab;
    public Transform firePoint;
    public float forwardForce = 10f; 
    public float upwardForce = 8f;   

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
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

        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = moveInput;
        }

        // F 키를 누를 때 이제 플레이어 몸통이 돌아가지 않습니다.
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            LaunchZiggsBomb();
        }
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 moveVelocity = moveInput * moveSpeed;
            playerRb.linearVelocity = new Vector3(moveVelocity.x, playerRb.linearVelocity.y, moveVelocity.z);

            //  이동할 때만 몸통이 회전하도록 설정
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

        //  핵심 수정: 폭탄 생성 위치(spawnPos)가 플레이어 콜라이더와 겹치지 않도록 정면(transform.forward)으로 조금 더 멀리 밀어서 생성합니다.
        // firePoint가 등록되어 있다면 firePoint 위치를 최우선으로 사용합니다.
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + (transform.forward * 1.5f) + (Vector3.up * 0.5f);
        
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        CustomBomb bombScript = bomb.GetComponent<CustomBomb>();
        if (bombScript != null)
        {
            // 플레이어가 실제로 바라보고 있는 정면 방향을 기준으로 발사 속도를 부여합니다.
            Vector3 launchDirection = (transform.forward * forwardForce) + (Vector3.up * upwardForce);
            
        }
    }
}
