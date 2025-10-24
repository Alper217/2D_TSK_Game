// BallSpawner.cs
using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    // G�NCELLEND�: 1 prefab yerine 4 prefab
    public GameObject[] ballPrefabs; // 4 prefab (Pink, Green, Blue, Normal)
    public int countPerPrefab = 2; // Her prefab'dan ka� tane spawn edilecek

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
        // Sol sahada t�m prefab'lardan spawn et
        SpawnAllPrefabs(leftAreaCenter, leftAreaSize);
        // Sa� sahada t�m prefab'lardan spawn et
        SpawnAllPrefabs(rightAreaCenter, rightAreaSize);
    }

    // YEN�: Verilen alanda t�m prefab'lardan 'countPerPrefab' kadar spawn eder
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

    // YEN�: Verilen alanda TEK bir top spawn eder
    void SpawnSingleBall(GameObject prefab, Vector2 center, Vector2 size)
    {
        if (prefab == null) return;

        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomY = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
        Vector3 spawnPos = new Vector3(randomX, randomY, 0);

        GameObject ball = Instantiate(prefab, spawnPos, Quaternion.identity);
        activeBalls.Add(ball); // Listeye ekle
    }

    // YEN�: GameManager'�n belirli bir tipi yeniden do�urmak i�in �a��rd��� fonksiyon
    public void RespawnSpecificBall(ElementType ballType, Vector2 center, Vector2 size)
    {
        GameObject prefabToSpawn = GetPrefabForType(ballType);
        if (prefabToSpawn != null)
        {
            SpawnSingleBall(prefabToSpawn, center, size);
        }
    }

    // YEN�: Element tipine g�re do�ru prefab'� bulur
    GameObject GetPrefabForType(ElementType ballType)
    {
        foreach (GameObject prefab in ballPrefabs)
        {
            Ball ballScript = prefab.GetComponent<Ball>();
            // Prefab'�n �zerindeki Ball script'inin tipini kontrol et
            if (ballScript != null && ballScript.ballType == ballType)
            {
                return prefab;
            }
        }
        Debug.LogWarning("Bu tip i�in prefab bulunamad�: " + ballType);
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