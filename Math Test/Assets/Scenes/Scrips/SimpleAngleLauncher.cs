using UnityEngine;
using TMPro;

public class SimpleAngleLauncher : MonoBehaviour
{
    public TMP_InputField angleInputField;
    public GameObject spherePrefab;
    public Transform firePoints;
    public float force = 15f;


    public void Lauch() //Canvas에 버튼 만들고 OnClick 이벤트에 연결
    {
        float angle = float.Parse(angleInputField.text);
        float rad = angle * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));

        GameObject sphere = Instantiate(spherePrefab, firePoints.position, Quaternion.identity);
        Rigidbody rb = sphere.GetComponent<Rigidbody>();

        rb.AddForce((dir + Vector3.up * 0.3f).normalized * force, ForceMode.Impulse);
    }
}
