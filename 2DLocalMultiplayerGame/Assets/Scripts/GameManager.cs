// GameManager.cs
using UnityEngine;
using UnityEngine.UI; // Puan için UI kullanacaksanýz ekleyin

public class GameManager : MonoBehaviour
{
    public int playerLeftScore = 0;
    public int playerRightScore = 0;
    public int scoreToWin = 2;

    public BallSpawner ballSpawner;
    public Transform playerLeft;
    public Transform playerRight;

    // Opsiyonel: Puanlarý göstermek için
    // public Text playerLeftScoreText;
    // public Text playerRightScoreText;

    void Start()
    {
        // Baþlangýçta BallSpawner'ý bul
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();
    }

    public void AddPoint(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft)
        {
            playerLeftScore++;
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightScore++;
        }

        Debug.Log($"SKOR: Sol Oyuncu: {playerLeftScore} - Sað Oyuncu: {playerRightScore}");

        // Opsiyonel: UI Güncelleme
        // if (playerLeftScoreText != null) playerLeftScoreText.text = playerLeftScore.ToString();
        // if (playerRightScoreText != null) playerRightScoreText.text = playerRightScore.ToString();

        CheckForWin();
    }

    void CheckForWin()
    {
        if (playerLeftScore >= scoreToWin)
        {
            Debug.Log("SOL OYUNCU KAZANDI!");
            // Oyunu bitirme mantýðý (örn: Time.timeScale = 0;
        }
        else if (playerRightScore >= scoreToWin)
        {
            Debug.Log("SAÐ OYUNCU KAZANDI!");
            // Oyunu bitirme mantýðý
        }
    }

    // Topu vuran oyuncunun sahasýnda yeniden doður
    public void RespawnBall(Transform shootingPlayer)
    {
        if (ballSpawner == null) return;

        if (shootingPlayer == playerLeft)
        {
            ballSpawner.SpawnBallsInArea(ballSpawner.leftAreaCenter, ballSpawner.leftAreaSize, 1);
        }
        else if (shootingPlayer == playerRight)
        {
            ballSpawner.SpawnBallsInArea(ballSpawner.rightAreaCenter, ballSpawner.rightAreaSize, 1);
        }
    }
}