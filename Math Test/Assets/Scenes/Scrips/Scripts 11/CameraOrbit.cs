using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    private float yaw = 0f;
    [HideInInspector] public float moveInput = 0f; // 외부에서 접근하므로 public 유지

    public float rotateSpeed = 100f;
    public Vector3 offset = new Vector3(0f, 4f, -7f);

    void Update()
    {
        yaw += moveInput * rotateSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 rotatedOffset = rotation * offset;
        transform.position = target.position + rotatedOffset;
        transform.LookAt(target);
    }
} // 맨 마지막에 이 괄호가 잘 닫혀있는지 확인!