using UnityEngine;

public class SimplePerlinNoise : MonoBehaviour
{
    [SerializeField]
    public int seed = 0;

    Vector2[] gradients = 
    {
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(1, 1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, -1).normalized
    };

    public float Noise(float x, float z)
    {
        int x0 = Mathf.FloorToInt(x);
        int z0 = Mathf.FloorToInt(z);

        int x1 = x0 + 1;
        int z1 = z0 + 1;

        float u = x - x0;
        float v = z - z0;

        Vector2 g00 = GetGradient(x0, z0);
        Vector2 g10 = GetGradient(x1, z0);
        Vector2 g01 = GetGradient(x0, z1);
        Vector2 g11 = GetGradient(x1, z1);

        Vector2 d00 = new Vector2(u, v);
        Vector2 d10 = new Vector2(u - 1f, v);
        Vector2 d01 = new Vector2(u, v - 1f);
        Vector2 d11 = new Vector2(u - 1f, v - 1f);

        float s00 = Dot(g00, d00);
        float s10 = Dot(g10, d10);
        float s01 = Dot(g01, d01);
        float s11 = Dot(g11, d11);

        float fu = Fade(u);
        float fv = Fade(v);

        float nx0 = Mathf.Lerp(s00, s10, fu);
        float nx1 = Mathf.Lerp(s01, s11, fu);

        float value = Mathf.Lerp(nx0, nx1, fv);

        return value * 0.5f + 0.5f;
    }

    Vector2 GetGradient(int x, int z)
    {
        int hash = x * 17 + z * 31 + seed;
        hash = Mathf.Abs(hash);

        int index = hash % gradients.Length;

        return gradients[index];
    }

    float Dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    float fV = 0f; // 기존 오타 방지용 임시 변수 제거 가능
    float Fade(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }
}