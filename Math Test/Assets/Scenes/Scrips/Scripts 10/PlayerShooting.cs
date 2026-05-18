using UnityEngine;
using UnityEngine.InputSystem; // 새 인풋 시스템을 사용하기 위해 필수!

public class PlayerShooting : MonoBehaviour
{
    [Header("Prefab & Target")]
    public GameObject bombPrefab;
    public Transform enemyTarget;

    [Header("Bezier Settings")]
    public float p1Radius = 3f;
    public float p2Radius = 5f;
    public float p1Height = 4f;
    public float p2Height = 6f;
    public float flightDuration = 1.5f;

    // 1. 새 인풋 시스템의 Send Messages 방식에 의해 호출되는 정상적인 함수
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            ShootBombs();
        }
    }

    // 2. 에러가 나던 Update 메서드는 지우거나 내부를 비워줍니다.
    void Update()
    {
        // 구형 Input 코드가 있던 자리를 비워두어 에러를 방지합니다.
    }

    void ShootBombs()
    {
        if (bombPrefab == null || enemyTarget == null) return;

        int bombCount = 10;
        for (int i = 0; i < bombCount; i++)
        {
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            Bezier projectile = bomb.GetComponent<Bezier>();

            if (projectile != null)
            {
                projectile.StartShooting(
                    transform.position,
                    enemyTarget.position,
                    p1Radius,
                    p2Radius,
                    p1Height,
                    p2Height,
                    flightDuration
                );
            }
        }
    }
}