using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform center;   // 공전 중심 (태양 or 지구)
    public float radius = 5f;  // 중심과 거리
    public float speed = 1f;   // 공전 속도

    private float angle = 0f;  // 현재 각도

    void Start()
    {
        // Inspector에서 center가 안 들어왔으면 자동 지정
        if (center == null)
        {
            // Moon이면 Earth, 아니면 Sun 찾기
            GameObject earth = GameObject.Find("Earth");
            GameObject sun = GameObject.Find("Sun");

            if (gameObject.name == "Moon" && earth != null)
                center = earth.transform;
            else if (sun != null)
                center = sun.transform;
            else
                Debug.LogWarning(gameObject.name + ": center가 지정되지 않았고 Sun/Earth도 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (center == null) return; // 안전장치

        // 각도 증가
        angle += speed * Time.deltaTime;

        // 원형 궤도 위치 계산
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // 중심 기준 위치 적용
        transform.position = center.position + new Vector3(x, 0, z);

        // Optional: 자전
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (center != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center.position, radius);
        }
    }
}