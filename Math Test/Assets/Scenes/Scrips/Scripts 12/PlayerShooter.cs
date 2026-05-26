using UnityEngine;
// 1. 신형 인풋 시스템 네임스페이스 추가
using UnityEngine.InputSystem; 

public class PlayerShooter : MonoBehaviour
{
    public GameObject bombPrefab; 
    public Transform firePoint;   
    public float launchSpeed = 15f; 

    void Update()
    {
        // 2. 구형 Input.GetKeyDown 대신 Keyboard.current 사용
        // 여기서는 'F' 키가 이번 프레임에 눌렸는지 체크합니다.
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            LaunchBomb();
        }
    }

    void LaunchBomb()
    {
        if (bombPrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward;
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

       
    }
}