using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public bool isPlayer1Ball; // 인스펙터에서 1P 공이면 체크, 2P 공이면 체크 해제

    private void OnCollisionEnter(Collision collision)
    {
        // 공들이 멈춰있는 상태거나, 현재 내 턴이 아니면 충돌 계산 제외
        if (!GameManager.Instance.isBallsMoving) return;

        if (GameManager.Instance.currentTurn == 1 && !isPlayer1Ball) return;
        if (GameManager.Instance.currentTurn == 2 && isPlayer1Ball) return;

        // 상대방 플레이어 공과 부딪혔는지 체크
        BallCollision otherBall = collision.gameObject.GetComponent<BallCollision>();
        if (otherBall != null)
        {
            // 내 턴인데 상대방 공을 때렸다!
            if (otherBall.isPlayer1Ball != this.isPlayer1Ball)
            {
                GameManager.Instance.hitOpponent = true;
            }
            return;
        }

        // Target(빨간공 등)과 부딪혔는지 체크 (태그가 "Target"이어야 합니다)
        if (collision.gameObject.CompareTag("Target"))
        {
            GameManager.Instance.hitTargets.Add(collision.gameObject);
        }
    }
}