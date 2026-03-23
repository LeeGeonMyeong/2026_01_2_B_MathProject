using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform player;

    public float viewAngle = 90f;       //시야각 (예:90도)
    public float viewDistance = 5f;     //최대 감지 거리

    private Vector3 originalScale;

    public void Start()
    {
        originalScale = transform.localScale;
    }


    public void Update()
    {
        Vector3 direction = player.position  - transform.position;

        //거리체크
        float distance = direction.magnitude;
        float angle = Vector3.Angle(transform.forward, direction);

        //둘다 만족해야 함
        if (distance < viewDistance && angle < viewAngle * 0.5f)
        {
            transform.localScale = Vector3.one * 2;
        }
        else
        {
            transform.localScale = originalScale;
        }
        
    }
}
