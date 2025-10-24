// GameManager.cs
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int playerLeftScore = 0;
    public int playerRightScore = 0;
    public int scoreToWin = 2;

    // YEN�: Oyuncu Canlar�
    public int playerLeftLives = 3;
    public int playerRightLives = 3;

    public BallSpawner ballSpawner;
    public Transform playerLeft;
    public Transform playerRight;

    // Opsiyonel UI'lar
    // public Text playerLeftScoreText;
    // public Text playerRightScoreText;
    // public Text playerLeftLivesText;
    // public Text playerRightLivesText;

    void Start()
    {
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();

        // Opsiyonel: Can UI'lar�n� g�ncelle
        // if (playerLeftLivesText != null) playerLeftLivesText.text = "Can: " + playerLeftLives;
        // if (playerRightLivesText != null) playerRightLivesText.text = "Can: " + playerRightLives;
    }

    public void AddPoint(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft) playerLeftScore++;
        else if (shootingPlayer == playerRight) playerRightScore++;

        Debug.Log($"SKOR: Sol Oyuncu: {playerLeftScore} - Sa� Oyuncu: {playerRightScore}");
        // Opsiyonel: UI G�ncelleme
        // if (playerLeftScoreText != null) playerLeftScoreText.text = playerLeftScore.ToString();
        // if (playerRightScoreText != null) playerRightScoreText.text = playerRightScore.ToString();

        CheckForWin();
    }

    // YEN�: Oyuncu Can Kaybetti�inde
    public void PlayerLosesLife(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft)
        {
            playerLeftLives--;
            // Opsiyonel: UI G�ncelleme
            // if (playerLeftLivesText != null) playerLeftLivesText.text = "Can: " + playerLeftLives;
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightLives--;
            // Opsiyonel: UI G�ncelleme
            // if (playerRightLivesText != null) playerRightLivesText.text = "Can: " + playerRightLives;
        }

        Debug.Log($"CAN KAYBED�LD�! Sol Can: {playerLeftLives} - Sa� Can: {playerRightLives}");
        CheckForWin(); // Can� biten var m� diye kontrol et
    }

    void CheckForWin()
    {
        if (playerLeftScore >= scoreToWin)
            Debug.Log("SOL OYUNCU KAZANDI! (Skor)");
        else if (playerRightScore >= scoreToWin)
            Debug.Log("SA� OYUNCU KAZANDI! (Skor)");

        // YEN�: Can kontrol�
        if (playerLeftLives <= 0)
            Debug.Log("SA� OYUNCU KAZANDI! (Sol Oyuncu Can� Bitti)");
        else if (playerRightLives <= 0)
            Debug.Log("SOL OYUNCU KAZANDI! (Sa� Oyuncu Can� Bitti)");
    }

    // G�NCELLEND�: Hangi tip topun spawn olaca��n� bilmesi gerek
    public void RespawnBall(Transform shootingPlayer, ElementType ballType)
    {
        if (ballSpawner == null) return;

        // BallSpawner'dan do�ru tipte topu spawn etmesini iste
        if (shootingPlayer == playerLeft)
        {
            ballSpawner.RespawnSpecificBall(ballType, ballSpawner.leftAreaCenter, ballSpawner.leftAreaSize);
        }
        else if (shootingPlayer == playerRight)
        {
            ballSpawner.RespawnSpecificBall(ballType, ballSpawner.rightAreaCenter, ballSpawner.rightAreaSize);
        }
    }
}