using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomTest : MonoBehaviour
{
    public TextMeshProUGUI[] labels = new TextMeshProUGUI[6];
    int[] counts = new int[6];

    public int trials = 100;

    private void Start()
    {
        for (int i = 0; i < trials; i++)
        {
            int result = Random.Range(1, 7);
            counts[result - 1]++;
        }
        for(int i = 0; i <counts.Length; i++)
        {
            float percent = (float)counts[i] / trials * 100f;
            string result = $"{i + 1}: {counts[i]}회 {percent:F2}%";
            labels[i].text = result;
        }
    }
}
