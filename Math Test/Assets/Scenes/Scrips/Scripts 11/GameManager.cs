using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Balls")]
    public Rigidbody player1Ball;
    public Rigidbody player2Ball;

    [Header("Target Balls")]
    public List<Rigidbody> targetBalls = new List<Rigidbody>(); // 안전하게 미리 생성
    private List<Rigidbody> allBalls = new List<Rigidbody>();

    [Header("UI Elements (Text or TextMeshPro)")]
    public GameObject turnText;
    public GameObject scoreText;
    public GameObject winText;

    // 게임 상태 변수
    public int currentTurn = 1; // 1 = 1P, 2 = 2P
    public int p1Score = 0;
    public int p2Score = 0;
    
    public bool isBallsMoving = false;

    // 이번 턴 충돌 기록 체크용 (공에 부딪힐 때마다 갱신됨)
    [HideInInspector] public bool hitOpponent = false;
    [HideInInspector] public HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 모든 공을 하나의 리스트로 관리하여 정지 여부 확인
        allBalls.Add(player1Ball);
        allBalls.Add(player2Ball);
        allBalls.AddRange(targetBalls);

        if (winText != null) winText.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (p1Score >= 5 || p2Score >= 5) return; // 게임 종료 시 체크 중단

        CheckBallsMoving();
    }

    // 4. 모든 공의 속도가 일정 값 이하인지 체크
    void CheckBallsMoving()
    {
        bool anyBallMoving = false;
        float stopThreshold = 0.05f; // 이 속도 이하면 멈춘 것으로 간주

        foreach (Rigidbody rb in allBalls)
        {
            if (rb != null && rb.linearVelocity.magnitude > stopThreshold)
            {
                anyBallMoving = true;
                break;
            }
        }

        // 공들이 움직이다가 방금 막 모두 멈춘 시점 감지
        if (isBallsMoving && !anyBallMoving)
        {
            isBallsMoving = false;
            EvaluateTurnResult(); // 턴 결과 정산
        }

        isBallsMoving = anyBallMoving;
    }

    // 5, 6번 조건: 점수 계산 및 턴 교체
    void EvaluateTurnResult()
    {
        if (currentTurn == 1)
        {
            if (hitOpponent) // 상대 공을 맞추면 감점 (-1)
            {
                p1Score = Mathf.Max(0, p1Score - 1);
            }
            // 상대 공을 안 건드리고 Target 공을 '모두' 맞췄을 때 득점 (+1)
            else if (hitTargets.Count == targetBalls.Count) 
            {
                p1Score++;
            }
        }
        else // 2P 턴일 때
        {
            if (hitOpponent)
            {
                p2Score = Mathf.Max(0, p2Score - 1);
            }
            else if (hitTargets.Count == targetBalls.Count)
            {
                p2Score++;
            }
        }

        UpdateUI();

        // 7. 5점에 도달하면 게임 종료 및 승리 텍스트 표시
        if (p1Score >= 5)
        {
            if (winText != null) winText.SetActive(true);

            var tText = winText.GetComponent<Text>();
            if (tText != null) tText.text = "PLAYER 1 WIN!";
            else 
            {
                var tmpText = winText.GetComponent<TMPro.TMP_Text>();
                if (tmpText != null) tmpText.text = "PLAYER 1 WIN!";
            }
            return;
        }
        
        if (p2Score >= 5)
        {
            if (winText != null) winText.SetActive(true);

            var tText = winText.GetComponent<Text>();
            if (tText != null) tText.text = "PLAYER 2 WIN!";
            else 
            {
                var tmpText = winText.GetComponent<TMPro.TMP_Text>();
                if (tmpText != null) tmpText.text = "PLAYER 2 WIN!";
            }
            return;
        }

        // 턴 전환 및 데이터 초기화
        currentTurn = (currentTurn == 1) ? 2 : 1;
        hitOpponent = false;
        hitTargets.Clear();
        
        UpdateUI();
    }

    void UpdateUI()
    {
        // Turn Text 업데이트 (일반 Text 및 TextMeshPro 둘 다 지원)
        if (turnText != null)
        {
            var tText = turnText.GetComponent<Text>();
            if (tText != null) tText.text = $"CURRENT TURN: {currentTurn}P";
            else 
            {
                var tmpText = turnText.GetComponent<TMPro.TMP_Text>();
                if (tmpText != null) tmpText.text = $"CURRENT TURN: {currentTurn}P";
            }
        }

        // Score Text 업데이트
        if (scoreText != null)
        {
            var sText = scoreText.GetComponent<Text>();
            if (sText != null) sText.text = $"1P Score: {p1Score}  |  2P Score: {p2Score}";
            else 
            {
                var tmpScore = scoreText.GetComponent<TMPro.TMP_Text>();
                if (tmpScore != null) tmpScore.text = $"1P Score: {p1Score}  |  2P Score: {p2Score}";
            }
        }
    }
} // 이 부분 괄호가 누락되어 에러가 났던 것입니다!