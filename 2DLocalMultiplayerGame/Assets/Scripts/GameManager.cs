// GameManager.cs
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int playerLeftScore = 0;
    public int playerRightScore = 0;
    public int scoreToWin = 2;

    // YENÝ: Oyuncu Canlarý
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

        // Opsiyonel: Can UI'larýný güncelle
        // if (playerLeftLivesText != null) playerLeftLivesText.text = "Can: " + playerLeftLives;
        // if (playerRightLivesText != null) playerRightLivesText.text = "Can: " + playerRightLives;
    }

    public void AddPoint(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft) playerLeftScore++;
        else if (shootingPlayer == playerRight) playerRightScore++;

        Debug.Log($"SKOR: Sol Oyuncu: {playerLeftScore} - Sað Oyuncu: {playerRightScore}");
        // Opsiyonel: UI Güncelleme
        // if (playerLeftScoreText != null) playerLeftScoreText.text = playerLeftScore.ToString();
        // if (playerRightScoreText != null) playerRightScoreText.text = playerRightScore.ToString();

        CheckForWin();
    }

    // YENÝ: Oyuncu Can Kaybettiðinde
    public void PlayerLosesLife(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft)
        {
            playerLeftLives--;
            // Opsiyonel: UI Güncelleme
            // if (playerLeftLivesText != null) playerLeftLivesText.text = "Can: " + playerLeftLives;
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightLives--;
            // Opsiyonel: UI Güncelleme
            // if (playerRightLivesText != null) playerRightLivesText.text = "Can: " + playerRightLives;
        }

        Debug.Log($"CAN KAYBEDÝLDÝ! Sol Can: {playerLeftLives} - Sað Can: {playerRightLives}");
        CheckForWin(); // Caný biten var mý diye kontrol et
    }

    void CheckForWin()
    {
        if (playerLeftScore >= scoreToWin)
            Debug.Log("SOL OYUNCU KAZANDI! (Skor)");
        else if (playerRightScore >= scoreToWin)
            Debug.Log("SAÐ OYUNCU KAZANDI! (Skor)");

        // YENÝ: Can kontrolü
        if (playerLeftLives <= 0)
            Debug.Log("SAÐ OYUNCU KAZANDI! (Sol Oyuncu Caný Bitti)");
        else if (playerRightLives <= 0)
            Debug.Log("SOL OYUNCU KAZANDI! (Sað Oyuncu Caný Bitti)");
    }

    // GÜNCELLENDÝ: Hangi tip topun spawn olacaðýný bilmesi gerek
    public void RespawnBall(Transform shootingPlayer, ElementType ballType)
    {
        if (ballSpawner == null) return;

        // BallSpawner'dan doðru tipte topu spawn etmesini iste
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