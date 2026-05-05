using UnityEngine;
using UnityEngine.InputSystem;

public class LerpMover : MonoBehaviour
{

    public Transform startPos;
    public Transform endPos;



    [SerializeField] private float duration = 2f;

    [SerializeField] private float t = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time / duration, 1f);
        transform.position = Vector3.Lerp(startPos.position, endPos.position, t);

    }
}