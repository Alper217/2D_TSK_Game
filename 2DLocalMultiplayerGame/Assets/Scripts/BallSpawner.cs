// BallSpawner.cs
using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    // D�ZELTME: Oyun ba��nda sadece 1 top spawn olmal�
    public int initialBallCountPerSide = 1;

    public Vector2 leftAreaCenter = new Vector2(-4f, 0f);
    public Vector2 leftAreaSize = new Vector2(7f, 6f);
    public Vector2 rightAreaCenter = new Vector2(4f, 0f);
    public Vector2 rightAreaSize = new Vector2(7f, 6f);

    public List<GameObject> activeBalls = new List<GameObject>();

    void Start()
    {
        SpawnInitialBalls();
    }

    void SpawnInitialBalls() // Fonksiyon ad� daha a��klay�c�
    {
        // Sol sahada ba�lang�� topu
        SpawnBallsInArea(leftAreaCenter, leftAreaSize, initialBallCountPerSide);

        // Sa� sahada ba�lang�� topu
        SpawnBallsInArea(rightAreaCenter, rightAreaSize, initialBallCountPerSide);
    }

    // BU FONKS�YON PUBLIC OLMALI (GameManager eri�ebilsin)
    public void SpawnBallsInArea(Vector2 center, Vector2 size, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float randomY = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
            Vector3 spawnPos = new Vector3(randomX, randomY, 0);

            GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
            activeBalls.Add(ball); // Listeye ekle
        }
    }

    public void RemoveBall(GameObject ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
        }
        // Topu her zaman yok et
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