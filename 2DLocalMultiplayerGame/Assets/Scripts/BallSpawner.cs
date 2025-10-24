using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;           // Ball prefab'ý buraya atanacak
    public int ballCountPerSide = 10;       // Her tarafta kaç top spawn olacak

    // Sol saha spawn alaný
    public Vector2 leftAreaCenter = new Vector2(-4f, 0f);
    public Vector2 leftAreaSize = new Vector2(7f, 6f);

    // Sað saha spawn alaný
    public Vector2 rightAreaCenter = new Vector2(4f, 0f);
    public Vector2 rightAreaSize = new Vector2(7f, 6f);

    public List<GameObject> activeBalls = new List<GameObject>();

    void Start()
    {
        SpawnBalls();
    }

    void SpawnBalls()
    {
        // Sol sahada top spawn et
        SpawnBallsInArea(leftAreaCenter, leftAreaSize, ballCountPerSide);

        // Sað sahada top spawn et
        SpawnBallsInArea(rightAreaCenter, rightAreaSize, ballCountPerSide);
    }

    void SpawnBallsInArea(Vector2 center, Vector2 size, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Belirtilen alan içinde rastgele pozisyon
            float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float randomY = Random.Range(center.y - size.y / 2, center.y + size.y / 2);

            Vector3 spawnPos = new Vector3(randomX, randomY, 0);

            // Top'u spawn et
            GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
            activeBalls.Add(ball);
        }
    }

    // Topu listeden kaldýr (gol atýldýðýnda vs.)
    public void RemoveBall(GameObject ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
            Destroy(ball);
        }
    }

    // Spawn alanlarýný görmek için (sadece editörde görünür)
    void OnDrawGizmos()
    {
        // Sol alan (Mavi)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(leftAreaCenter, leftAreaSize);

        // Sað alan (Kýrmýzý)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(rightAreaCenter, rightAreaSize);
    }
}