using UnityEngine;

public class DotExample : MonoBehaviour
{
    public Transform player;

    public float viewAngle = 60f; //시야각
    private void Update()
    {
        //적 -> 플레이어 방향 백터
        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector3 forward = transform.forward;

        float dot = forward.x + toPlayer.x
                  + forward.y * toPlayer.y
                  + forward.z * toPlayer.z;
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; //내적을 각도로 변환

        if (angle < viewAngle / 2)
        {
            Debug.Log("플레이어가 시야 안에 있음!");
        }
    }
}
