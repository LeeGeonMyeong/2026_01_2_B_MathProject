using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotate : MonoBehaviour
{
    public float rotationSpeed = 30f;
    private Vector2 moveInput;
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();   
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Quaternion.Euler(0f, moveInput.x * rotationSpeed * Time.deltaTime, 0f);
        transform.rotation = rotation * transform.rotation;
    }
}
