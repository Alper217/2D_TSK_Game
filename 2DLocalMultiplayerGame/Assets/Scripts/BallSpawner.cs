// BallSpawner.cs
using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    // GÜNCELLENDÝ: 1 prefab yerine 4 prefab
    public GameObject[] ballPrefabs; // 4 prefab (Pink, Green, Blue, Normal)
    public int countPerPrefab = 2; // Her prefab'dan kaç tane spawn edilecek

    public Vector2 leftAreaCenter = new Vector2(-4f, 0f);
    public Vector2 leftAreaSize = new Vector2(7f, 6f);
    public Vector2 rightAreaCenter = new Vector2(4f, 0f);
    public Vector2 rightAreaSize = new Vector2(7f, 6f);

    public List<GameObject> activeBalls = new List<GameObject>();

    void Start()
    {
        SpawnInitialBalls();
    }
    void SpawnInitialBalls()
    {
        // Sol sahada tüm prefab'lardan spawn et
        SpawnAllPrefabs(leftAreaCenter, leftAreaSize);
        // Sað sahada tüm prefab'lardan spawn et
        SpawnAllPrefabs(rightAreaCenter, rightAreaSize);
    }

    // YENÝ: Verilen alanda tüm prefab'lardan 'countPerPrefab' kadar spawn eder
    void SpawnAllPrefabs(Vector2 center, Vector2 size)
    {
        foreach (GameObject prefab in ballPrefabs)
        {
            for (int i = 0; i < countPerPrefab; i++)
            {
                SpawnSingleBall(prefab, center, size);
            }
        }
    }

    // YENÝ: Verilen alanda TEK bir top spawn eder
    void SpawnSingleBall(GameObject prefab, Vector2 center, Vector2 size)
    {
        if (prefab == null) return;

        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomY = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
        Vector3 spawnPos = new Vector3(randomX, randomY, 0);

        GameObject ball = Instantiate(prefab, spawnPos, Quaternion.identity);
        activeBalls.Add(ball); // Listeye ekle
    }

    // YENÝ: GameManager'ýn belirli bir tipi yeniden doðurmak için çaðýrdýðý fonksiyon
    public void RespawnSpecificBall(ElementType ballType, Vector2 center, Vector2 size)
    {
        GameObject prefabToSpawn = GetPrefabForType(ballType);
        if (prefabToSpawn != null)
        {
            SpawnSingleBall(prefabToSpawn, center, size);
        }
    }

    // YENÝ: Element tipine göre doðru prefab'ý bulur
    GameObject GetPrefabForType(ElementType ballType)
    {
        foreach (GameObject prefab in ballPrefabs)
        {
            Ball ballScript = prefab.GetComponent<Ball>();
            // Prefab'ýn üzerindeki Ball script'inin tipini kontrol et
            if (ballScript != null && ballScript.ballType == ballType)
            {
                return prefab;
            }
        }
        Debug.LogWarning("Bu tip için prefab bulunamadý: " + ballType);
        return null;
    }

    public void RemoveBall(GameObject ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
        }
        Destroy(ball);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(leftAreaCenter, leftAreaSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(rightAreaCenter, rightAreaSize);
    }
}