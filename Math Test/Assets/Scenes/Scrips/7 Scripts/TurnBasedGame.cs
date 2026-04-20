using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Text UI를 사용하기 위해 추가

public class TurnBasedGame : MonoBehaviour
{
    [SerializeField] float critChance = 0.2f;
    [SerializeField] float meanDamage = 20f;
    [SerializeField] float stdDevDamage = 5f;
    [SerializeField] float enemyHP = 100f;
    [SerializeField] float poissonLambda = 2f;
    [SerializeField] float hitRate = 0.6f;
    [SerializeField] float critDamageRate = 2f;
    [SerializeField] int maxHitsPerTurn = 5;

    // --- 추가된 필드 ---
    [SerializeField] float baseRareItemChance = 0.05f; // 기본 레어 아이템 획득 확률
    [SerializeField] TMP_Text resultText; // 결과를 출력할 Text UI

    int turn = 0;
    bool rareItemObtained = false;
    float currentRareItemChance; // 현재 턴의 레어 아이템 획득 확률

    string[] rewards = { "Gold", "Weapon", "Armor", "Potion" };

    // 화면에 출력할 데이터를 담는 변수들
    int totalEnemies = 0;
    int totalKilledEnemies = 0;
    float totalHits = 0;
    float totalCritHits = 0;
    float maxDamage = 0f;
    float minDamage = float.MaxValue;
    int potionCount = 0;
    int goldCount = 0;
    int normalWeaponCount = 0;
    int rareWeaponCount = 0;
    int normalArmorCount = 0;
    int rareArmorCount = 0;

    public void StartSimulation()
    {
        rareItemObtained = false;
        turn = 0;
        currentRareItemChance = baseRareItemChance; // 시작 확률 초기화

        // 데이터 초기화
        totalEnemies = 0;
        totalKilledEnemies = 0;
        totalHits = 0;
        totalCritHits = 0;
        maxDamage = 0f;
        minDamage = float.MaxValue;
        potionCount = 0;
        goldCount = 0;
        normalWeaponCount = 0;
        rareWeaponCount = 0;
        normalArmorCount = 0;
        rareArmorCount = 0;

        while (!rareItemObtained)
        {
            SimulateTurn();
            turn++;
            currentRareItemChance += 0.05f; // 턴마다 확률 5% 증가
        }

        Debug.Log($"레어 아이템 {turn} 턴에 획득 (확률: {currentRareItemChance * 100:F1}%)");
        UpdateResultText(); // 화면에 결과 출력
    }

    void SimulateTurn()
    {
        Debug.Log($"--- Turn {turn + 1} ---");

        // 푸아송 샘플링: 적 등장 수
        int enemyCount = SamplePoisson(poissonLambda);
        totalEnemies += enemyCount;
        Debug.Log($"적 등장 : {enemyCount}");

        for (int i = 0; i < enemyCount; i++)
        {
            // 이항 샘플링: 명중 횟수
            int hits = SampleBinomial(maxHitsPerTurn, hitRate);
            float totalDamage = 0f;
            totalHits += hits;

            for (int j = 0; j < hits; j++)
            {
                float damage = SampleNormal(meanDamage, stdDevDamage);

                // 최대/최소 데미지 갱신
                maxDamage = Mathf.Max(maxDamage, damage);
                minDamage = Mathf.Min(minDamage, damage);

                // 베르누이 분포 샘플링: 확률 기반 치명타 발생
                if (Random.value < critChance)
                {
                    totalCritHits++;
                    damage *= critDamageRate;
                    Debug.Log($" 크리티컬 hit! {damage:F1}");
                }
                else
                    Debug.Log($" 일반 hit! {damage:F1}");

                totalDamage += damage;
            }

            if (totalDamage >= enemyHP)
            {
                totalKilledEnemies++;
                Debug.Log($"적 {i + 1} 처치! (데미지: {totalDamage:F1})");

                // 균등 분포 샘플링: 보상 결정
                string reward = rewards[UnityEngine.Random.Range(0, rewards.Length)];
                Debug.Log($"보상: {reward}");

                if (reward == "Potion") potionCount++;
                else if (reward == "Gold") goldCount++;
                else if (reward == "Weapon")
                {
                    if (Random.value < currentRareItemChance)
                    {
                        rareItemObtained = true;
                        rareWeaponCount++;
                        Debug.Log($"레어 무기 획득! (확률: {currentRareItemChance * 100:F1}%)");
                    }
                    else
                    {
                        normalWeaponCount++;
                    }
                }
                else if (reward == "Armor")
                {
                    if (Random.value < currentRareItemChance)
                    {
                        rareItemObtained = true;
                        rareArmorCount++;
                        Debug.Log($"레어 방어구 획득! (확률: {currentRareItemChance * 100:F1}%)");
                    }
                    else
                    {
                        normalArmorCount++;
                    }
                }
            }
        }
    }

    // 화면의 Text UI에 결과를 출력하는 함수
    void UpdateResultText()
    {
        if (resultText == null) return;

        // 문자열 형식으로 결과 작성
        string resultStr = "전투 결과\n\n";
        resultStr += $"총 진행 턴 수 : {turn}\n";
        resultStr += $"발생한 적 : {totalEnemies}\n";
        resultStr += $"처치한 적 : {totalKilledEnemies}\n";
        resultStr += $"공격 명중 결과 : {(totalHits / (turn * poissonLambda * maxHitsPerTurn)) * 100:F2}%\n"; // 대략적인 명중률 계산
        resultStr += $"발생한 치명타율 결과 : {(totalCritHits / totalHits) * 100:F2}%\n";
        resultStr += $"최대 데미지 : {maxDamage:F2}\n";
        resultStr += $"최소 데미지 : {minDamage:F2}\n\n";

        resultStr += "획득한 아이템\n\n";
        resultStr += $"포션 : {potionCount}개\n";
        resultStr += $"골드 : {goldCount}개\n";
        resultStr += $"무기 - 일반 : {normalWeaponCount}개\n";
        resultStr += $"무기 - 레어 : {rareWeaponCount}개\n";
        resultStr += $"방어구 - 일반 : {normalArmorCount}개\n";
        resultStr += $"방어구 - 레어 : {rareArmorCount}개\n";

        resultText.text = resultStr;
    }

    // --- 분포 샘플 함수들 ---
    int SamplePoisson(float lambda)
    {
        int k = 0;
        float p = 1f;
        float L = Mathf.Exp(-lambda);
        while (p > L)
        {
            k++;
            p *= Random.value;
        }
        return k - 1;
    }

    int SampleBinomial(int n, float p)
    {
        int success = 0;
        for (int i = 0; i < n; i++)
            if (Random.value < p) success++;
        return success;
    }

    float SampleNormal(float mean, float stdDev)
    {
        float u1 = Random.value;
        float u2 = Random.value;
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);
        return mean + stdDev * z;
    }
}