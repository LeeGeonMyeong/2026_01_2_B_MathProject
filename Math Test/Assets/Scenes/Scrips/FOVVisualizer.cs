using UnityEngine;

public class FOVVisualizer : MonoBehaviour
{
    public float viewAngle = 60f;

    public float viewDistance = 5f;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 forward = transform.forward * viewDistance;

        //왼쪽 시야 경계
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        //오른쪽 시야 경계
        Vector3 rightBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);



        //캐릭터 앞쪽 방향
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, forward);
    }
}
