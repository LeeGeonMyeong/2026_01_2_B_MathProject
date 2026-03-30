using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 30f;

    private Vector2 moveInput;

    public bool isLeftParrying = false;
    public bool isRightParrying = false;

    public Transform respawnPoint;

    private Rigidbody rb; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    public void Respawn()
    {
        transform.position = respawnPoint.position;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void OnLeftParry(InputValue value)
    {
        if (value.isPressed)
        {
            StartCoroutine(ParryWindow(true));
        }
    }

    public void OnRightParry(InputValue value)
    {
        if (value.isPressed)
        {
            StartCoroutine(ParryWindow(false));
        }
    }

    IEnumerator ParryWindow(bool isLeft)
    {
        if (isLeft)
            isLeftParrying = true;
        else
            isRightParrying = true;

        Debug.Log(isLeft ? "왼쪽 패링 ON" : "오른쪽 패링 ON");

        yield return new WaitForSeconds(0.3f); //  핵심 타이밍

        if (isLeft)
            isLeftParrying = false;
        else
            isRightParrying = false;

        Debug.Log("패링 OFF");
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        float rotation = moveInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);

        Vector3 moveDir = moveInput.y * moveSpeed * Time.deltaTime * transform.forward;
        transform.position += moveDir;
    }
}