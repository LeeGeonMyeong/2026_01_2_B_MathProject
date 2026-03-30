using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4f, -5f);

    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        Vector3 desiredPosition =
            target.position +
            Quaternion.Euler(0f, target.eulerAngles.y, 0f) * offset;

        // 부드럽게 따라가기
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target);
    }
}