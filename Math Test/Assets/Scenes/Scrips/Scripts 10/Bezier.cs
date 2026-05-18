using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    private Vector3 p0; // 시작점
    private Vector3 p1; // 제어점 1
    private Vector3 p2; // 제어점 2
    private Vector3 p3; // 도착점 (적 위치)

    private List<Vector3> points;
    private float time = 0f;
    private float speed = 1f; // 발사 속도 조절용
    private bool isShooting = false;

    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.enabled = false; // 처음엔 트레일을 끕니다.
    }

    
    public void StartShooting(Vector3 startPos, Vector3 targetPos, float p1Radius, float p2Radius, float p1Height, float p2Height, float duration)
    {
        p0 = startPos;
        p3 = targetPos;
        speed = 1f / duration; // 지정된 시간(초) 동안 날아가도록 설정

        // 제어점 랜덤 생성
        Vector2 rand1 = Random.insideUnitCircle * p1Radius;
        p1 = p0 + new Vector3(rand1.x, 0f, rand1.y);
        p1.y += p1Height;

        Vector2 rand2 = Random.insideUnitCircle * p2Radius;
        p2 = p3 + new Vector3(rand2.x, 0f, rand2.y);
        p2.y += p2Height;

        // 리스트 구성
        points = new List<Vector3> { p0, p1, p2, p3 };

        transform.position = p0;
        time = 0f;

        if (trail != null)
        {
            trail.Clear();    // 이전 잔상 제거
            trail.enabled = true; // 트레일 켜기
        }

        isShooting = true;
    }

    void Update()
    {
        if (!isShooting) return;

        time += Time.deltaTime * speed;

        if (time >= 1f)
        {
            transform.position = DeCasteljau(new List<Vector3>(points), 1f);
            isShooting = false;
            OnTargetHit(); // 목표 도달 시 처리
            return;
        }

        // 원본 points가 변형되지 않도록 복사본을 전달합니다.
        transform.position = DeCasteljau(new List<Vector3>(points), time);
    }

    // 기존 DeCasteljau 코드의 무한 루프 버그를 수정했습니다.
    Vector3 DeCasteljau(List<Vector3> p, float t)
    {
        while (p.Count > 1)
        {
            int last = p.Count - 1;
            var next = new List<Vector3>(last);

            for (int i = 0; i < last; i++)
            {
                next.Add(Vector3.Lerp(p[i], p[i + 1], t));
            }
            p = next; // 다음 단계의 리스트로 교체하여 개수를 줄여나갑니다.
        }

        return p[0];
    }

    void OnTargetHit()
    {
        //  적에게 데미지를 주거나 이펙트를 생성하는 로직이 들어갈 자리입니다.
        Destroy(gameObject, 0.5f); // 트레일이 완전히 사라질 수 있도록 약간의 여유를 두고 파괴
    }
}