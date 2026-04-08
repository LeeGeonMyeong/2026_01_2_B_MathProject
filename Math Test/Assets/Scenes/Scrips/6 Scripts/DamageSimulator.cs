using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class DamageSimulator : MonoBehaviour
{
    public TextMeshProUGUI statusDisplay;
    public TextMeshProUGUI logDisplay;
    public TextMeshProUGUI resultDisplay;
    public TextMeshProUGUI rangeDisplay;

    private int level = 1;
    private float totalDamage = 0, baseDamage = 20f;
    private int attackCount = 0;

    private string weaponName;
    private float stdDevMult, critRate, critMult;

    private int weakPointCount = 0;     //약점 공격 횟수 (+2 초과)
    private int missCount = 0;          //명중 실패 횟수 (-2 미만)
    private int totalCritCount = 0;     //전체 크리티컬 횟수
    private float maxDamage = 0f;       //최대 데미지 기록
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetWeapon(0); //시작 시 단검 장착
    }

    private void ResetData()
    {
        totalDamage = 0;
        attackCount = 0;
        level = 0;
        baseDamage = 20f;
        maxDamage = 0f;
        totalCritCount = 0;
        missCount = 0;
        weakPointCount = 0;
    }
    // Update is called once per frame
    public void SetWeapon(int id)
    {
        ResetData();
        if (id == 0)
        {
            SetStats("단검", 0.1f, 0.4f, 1.5f);
        }
        else if (id == 1)
        {
            SetStats("마검", 0.2f, 0.3f, 2.0f);
        }
        else
        {
            SetStats("엑스칼리버", 0.3f, 0.2f, 3.0f);
        }

        logDisplay.text = string.Format("{0} 장착!", weaponName);
        UpdateUI();
    }

    private void SetStats(string _name, float _stdDev, float _critRate, float _critMult)
    {
        weaponName = _name;
        stdDevMult = _stdDev;
        critRate = _critRate;
        critMult = _critMult;
    }

    public void LevelUp()
    {
        totalDamage = 0;
        attackCount = 0;
        level++;
        baseDamage = level * 20f;
        logDisplay.text = string.Format("레벨업! 현재 레벨: {0}", level);
        UpdateUI();
    }

    public void OnAttack()
    {
        // 1. 정규분포 데미지 계산
        float sd = baseDamage * stdDevMult;
        float normalDamage = GetNormalStdDevDamage(baseDamage, sd);
        float finalDamage = 0f;
        // 경계값 설정
        float upperBound = baseDamage + (2 * sd);
        float lowerBound = baseDamage - (2 * sd);

        // 명중실패 판정
        if (normalDamage < lowerBound)
        {
            logDisplay.text = "<color=#808080>[빗나감]</color> 공격이 완전히 빗나갔습니다.";
            UpdateUI();
            return; //
        }

        // 약점 공격 판정 (정규분포 2 초과 시 2배 데미지
        bool isWeakPoint = normalDamage > upperBound;
        if (isWeakPoint)
        {
            normalDamage *= 2f;
        }

        // 4. 치명타 판정
        bool isCrit = Random.value < critRate;
        if (isCrit)
        {
            totalCritCount++; // 크리티컬 횟수 증가
            finalDamage = normalDamage * critMult;
        }
        else finalDamage = normalDamage;


        //로그 및 UI 업데이트
        string critMark = isCrit ? "<color=red>[치명타!]<color> " : "";
        string weakMark = isWeakPoint ? "<color=yellow>[약점 포착!]</color>" : "";
        logDisplay.text = string.Format("{0}: {1:F1}", critMark, finalDamage);
        UpdateUI();
    }

    public void OnAttackThousand()
    {
        // 1000번 반복하기 전에 이전 로그를 지워줍니다.
        logDisplay.text = "1000회 공격 시뮬레이션 중...";

        for (int i = 0; i < 1000; i++)
        {
            // 1. 정규분포 데미지 계산
            float sd = baseDamage * stdDevMult;
            float normalDamage = GetNormalStdDevDamage(baseDamage, sd);
            float finalDamage = 0f;

            // 2. 명중 실패 판정 (-2 미만)
            if (normalDamage < baseDamage - (2 * sd))
            {
                missCount++;
                finalDamage = 0f;
            }
            else
            {
                // 3. 약점 공격 판정 (+2 초과)
                if (normalDamage > baseDamage + (2 * sd))
                {
                    weakPointCount++;
                    normalDamage *= 2f; // 데미지 2배
                }

                // 4. 치명타 판정
                bool isCrit = Random.value < critRate;
                if (isCrit)
                {
                    totalCritCount++;
                    finalDamage = normalDamage * critMult;
                }
                else
                {
                    finalDamage = normalDamage;
                }
            }

            // 5. 결과 누적
            attackCount++;
            totalDamage += finalDamage;
            if (finalDamage > maxDamage) maxDamage = finalDamage;
        }

        // 모든 반복이 끝난 후 UI를 딱 한 번만 갱신 (성능 최적화)
        logDisplay.text = "1000회 연속 공격 완료!";
        UpdateUI();
    }

    private void UpdateUI()
    {
        statusDisplay.text = string.Format("Level: {0} / 무기: {1}\n기본 데미지: {2} / 치명타: {3}% (x{4}",
                level, weaponName, baseDamage, critRate * 100, critMult);

        rangeDisplay.text = string.Format("예상 일반 데메지 범위 : [{0:F1} ~ {1:F1}]",
            baseDamage - (3 * baseDamage * stdDevMult),
            baseDamage + (3 * baseDamage * stdDevMult));

        

        float dpa = attackCount > 0 ? totalDamage / attackCount : 0;
        resultDisplay.text = string.Format(
        "누적 데미지: {0:F1}\n공격 횟수: {1}\n평균 DPA: {2:F2}\n약점 공격: {3}회\n명중 실패: {4}회\n전체 크리티컬: {5}회\n최대 데미지: {6:F1}",
        totalDamage,      // {0}
        attackCount,      // {1}
        dpa,              // {2}
        weakPointCount,   // {3}
        missCount,        // {4}
        totalCritCount,   // {5}
        maxDamage         // {6}
        );
    }


    private float GetNormalStdDevDamage(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }
}