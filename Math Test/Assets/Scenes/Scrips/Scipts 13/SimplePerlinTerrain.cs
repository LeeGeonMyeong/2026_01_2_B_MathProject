using UnityEngine;

public class SimplePerlinTerrain : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 30;
    public int depth = 30;
    public float scale = 0.1f;
    public float heightMultiplier = 8f;
    
    [Header("Water Settings")]
    public int waterLevel = 3; // 이 높이 이하의 빈 공간에는 물이 찹니다.

    [Header("Prefabs")]
    public GameObject grassPrefab; // 제일 상단 타일
    public GameObject dirtPrefab;  // 기존 맵 (흙)
    public GameObject waterPrefab; // 물 타일

    SimplePerlinNoise simpleNoise;
    
    // 매 실행마다 새로운 맵이 나오게 하기 위한 오프셋 변수
    private float offsetX;
    private float offsetZ;

    void Start()
    {
        simpleNoise = GetComponent<SimplePerlinNoise>();
        
        // 매 실행마다 새로운 지형이 나오도록 무작위 오프셋 결정
        offsetX = Random.Range(0f, 1000f);
        offsetZ = Random.Range(0f, 1000f);

        Generate();
    }

    public void Generate()
    {
        // 1단계: 흙(Dirt)과 잔디(Grass) 배치
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 무작위성을 부여하기 위해 좌표에 오프셋(Offset) 추가
                float xCoord = (x * scale) + offsetX;
                float zCoord = (z * scale) + offsetZ;

                float noise = simpleNoise.Noise(xCoord, zCoord);
                int height = Mathf.RoundToInt(noise * heightMultiplier);

                CreateColumn(x, z, height);
            }
        }

        // 2단계: 배치가 끝난 후 사후 처리 (물 채우기 로직)
        FillWater();
    }

    // 기존의 CreateCube를 확장하여 높이에 따라 블록을 다르게 배치합니다.
    void CreateColumn(int x, int z, int height)
    {
        for (int y = 0; y <= height; y++)
        {
            Vector3 position = new Vector3(x, y, z);
            GameObject prefabToSpawn = dirtPrefab;

            // 조건 1: 높이 판별 후 최 상단일 경우 (y == height) Grass 배치
            if (y == height)
            {
                prefabToSpawn = grassPrefab;
            }
            else
            {
                prefabToSpawn = dirtPrefab;
            }

            Instantiate(prefabToSpawn, position, Quaternion.identity, transform);
        }
    }

    // 조건 2: 전체 맵을 검사하여 특정 높이(waterLevel) 이하에 타일이 없다면 물을 채웁니다.
    void FillWater()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y <= waterLevel; y++)
                {
                    Vector3 targetPos = new Vector3(x, y, z);

                    // 해당 위치에 이미 블록(Dirt나 Grass)이 존재하는지 레이캐스트나 Physics로 체크합니다.
                    // 여기서는 간단하게 오버랩 박스를 사용하여 해당 좌표가 비어있는지 확인합니다.
                    if (!Physics.CheckBox(targetPos, new Vector3(0.4f, 0.4f, 0.4f)))
                    {
                        // 비어있다면 물 타일을 생성합니다.
                        Instantiate(waterPrefab, targetPos, Quaternion.identity, transform);
                    }
                }
            }
        }
    }
}