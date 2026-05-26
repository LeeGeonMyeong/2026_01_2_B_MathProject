using UnityEngine;

public class ReflectTest : MonoBehaviour
{
    public Vector3 velocity = new Vector3(2f, -3f, 0f);

    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    float damping = 0.9f;   //감쇠 계수

    void Update()
    {
        velocity += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

    void OllisionEnter(Collision col)
    {
        Vector3 normal = col.contacts[0].normal.normalized; //충돌 지점의 법선 벡터

        float dot = Vector3.Dot(velocity, normal);
        Vector3 reflect = velocity - 2f * dot * normal; //반사 벡터 수식 : R = V - 2(V * N)N

        velocity = reflect * damping;
    }
}
