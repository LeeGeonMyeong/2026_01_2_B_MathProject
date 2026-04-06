using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationUIManager : MonoBehaviour
{
    [Header("=== UI 연결 ===")]
    // 이미지의 위치별로 텍스트를 분리했습니다.
    public TextMeshProUGUI combatStatsText;   // 왼쪽 위 (전체 공격, 치명타 등)
    public TextMeshProUGUI enemyHpText;       // 가운데 (체력 : 270/300)
    public TextMeshProUGUI dropProbsText;     // 오른쪽 위 (현재 아이템 확률)
    public TextMeshProUGUI inventoryText;     // 오른쪽 아래 (현재 드롭된 아이템)

    [Header("=== 전투 설정 ===")]
    public float playerAtk = 30f;
    public float enemyMaxHp = 300f;
    private float currentEnemyHp;

    [Header("=== 치명타 통계 ===")]
    private int totalHits = 0;
    private int critHits = 0;
    public float targetCritRate = 0.3f; // 설정된 30%

    [Header("=== 아이템 확률 및 인벤토리 ===")]
    // 확률
    private float probNormal = 50f;
    private float probAdvanced = 30f;
    private float probRare = 15f;
    private float probLegend = 5f;

    // 이미지 오른쪽 아래처럼 획득 개수를 기록할 변수
    private int countNormal = 0;
    private int countAdvanced = 0;
    private int countRare = 0;
    private int countLegend = 0;

    void Start()
    {
        currentEnemyHp = enemyMaxHp;
        // 초기화 시 아이템 확률을 보여주기 위해 AdjustProbabilities를 한 번 호출할 수도 있습니다.
        UpdateAllUI();
    }

    // [UI 공격 버튼에 연결]
    public void OnAttackButtonClick()
    {
        if (currentEnemyHp <= 0) return;

        bool isCrit = RollCrit();
        float damage = isCrit ? playerAtk * 2 : playerAtk;
        currentEnemyHp -= damage;

        if (currentEnemyHp <= 0)
        {
            DropLootAndReset();
        }

        UpdateAllUI();
    }

    // 치명타 보정 로직 (사용자 로직 기반)
    bool RollCrit()
    {
        totalHits++;
        float currentRate = (totalHits > 1) ? (float)critHits / totalHits : 0f;

        // 보정: 확률이 너무 낮으면 강제 발생 (+5% 오차 허용)
        if (currentRate < targetCritRate && (float)(critHits + 1) / totalHits <= targetCritRate + 0.05f)
        {
            critHits++;
            return true;
        }
        // 보정: 확률이 너무 높으면 강제 일반 공격
        if (currentRate > targetCritRate + 0.1f)
        {
            return false;
        }
        // 기본 확률
        if (Random.value < targetCritRate)
        {
            critHits++;
            return true;
        }
        return false;
    }

    // 전리품 획득 및 적 초기화
    void DropLootAndReset()
    {
        float r = Random.Range(0f, 100f);

        // 누적 확률 구간 방식 (전설부터 체크)
        if (r < probLegend)
        {
            countLegend++;
            ResetItemProbabilities(); // 전설 획득 시 초기화
        }
        else if (r < probLegend + probRare)
        {
            countRare++;
            AdjustProbabilities();
        }
        else if (r < probLegend + probRare + probAdvanced)
        {
            countAdvanced++;
            AdjustProbabilities();
        }
        else
        {
            countNormal++;
            AdjustProbabilities();
        }

        currentEnemyHp = enemyMaxHp; // 새로운 적 등장
    }

    // 전설 획득 실패 시 확률 보정 (과제 조건)
    void AdjustProbabilities()
    {
        probLegend += 1.5f;
        probNormal -= 0.5f;
        probAdvanced -= 0.5f;
        probRare -= 0.5f;
    }

    // 전설 획득 시 초기화
    void ResetItemProbabilities()
    {
        probNormal = 50f;
        probAdvanced = 30f;
        probRare = 15f;
        probLegend = 5f;
    }

    // === UI 업데이트 통합 함수 ===
    void UpdateAllUI()
    {
        // 1. 왼쪽 위: 전투 통계
        float actualCritRate = (totalHits > 0) ? (float)critHits / totalHits * 100f : 0f;
        combatStatsText.text = $"전체 공격 회수 : {totalHits}\n" +
                              $"발생한 치명타 회수 : {critHits}\n" +
                              $"설정된 치명타 확률 : {targetCritRate * 100f:F2}%\n" +
                              $"실제 치명타 확률 : {actualCritRate:F2}%";

        // 2. 가운데: 적 체력
        enemyHpText.text = $"체력 : <color=red>{Mathf.Max(0, currentEnemyHp)}/{enemyMaxHp}</color>";

        // 3. 오른쪽 위: 아이템 확률
        dropProbsText.text = $"현재 아이템 확률\n" +
                             $"일반 : {probNormal:F1}%\n" +
                             $"고급 : {probAdvanced:F1}%\n" +
                             $"희귀 : {probRare:F1}%\n" +
                             $"전설 : {probLegend:F1}%";

        // 4. 오른쪽 아래: 인벤토리 (획득 개수)
        inventoryText.text = $"현재 드롭된 아이템\n\n" +
                             $"일반 : {countNormal}\n" +
                             $"고급 : {countAdvanced}\n" +
                             $"희귀 : {countRare}\n" +
                             $"전설 : {countLegend}";
    }
}