using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRayCastTest : MonoBehaviour
{
    public float rayDistance = 100f;
    float moveInput;
    public CameraOrbit cam;

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
        cam.moveInput = moveInput;
    }

    public void OnClick(InputValue value)
    {
        if (!value.isPressed)
            return;

        // [조건 3] 공이 움직이는 동안에는 추가 입력을 막는다.
        if (GameManager.Instance.isBallsMoving)
            return;

        // 점수가 5점 도달해서 게임이 끝났다면 입력 막기
        if (GameManager.Instance.p1Score >= 5 || GameManager.Instance.p2Score >= 5)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;

            if (rb != null)
            {
                // [조건 2] 1P 턴에는 1P 공만, 2P 턴에는 2P 공만 칠 수 있다.
                BallCollision ballInfo = rb.GetComponent<BallCollision>();
                if (ballInfo == null) return; // 주공이 아니면 무시 (Target공 클릭 방지)

                if (GameManager.Instance.currentTurn == 1 && !ballInfo.isPlayer1Ball) return;
                if (GameManager.Instance.currentTurn == 2 && ballInfo.isPlayer1Ball) return;

                // 물리 타격 로직 (기존 유지)
                Vector3 hitPoint = hit.point;
                Vector3 center = rb.gameObject.transform.position;
                Vector3 forceDirection = center - hitPoint;
                forceDirection.y = 0f; // Y축 힘 제거 (마쎄이 방지 및 튀기 방지)
                forceDirection.Normalize();

                rb.AddForce(forceDirection * 10f, ForceMode.Impulse);

                // 공이 즉시 움직이기 시작하므로 플래그를 미리 켜서 연속 클릭 방지
                GameManager.Instance.isBallsMoving = true;
            }
        }
    }
}