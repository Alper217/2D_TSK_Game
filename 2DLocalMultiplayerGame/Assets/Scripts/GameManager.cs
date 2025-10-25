using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Skor ve Can Ayarlar�")]
    public int playerLeftScore = 0;
    public int playerRightScore = 0;
    public int scoreToWin = 2;
    public int playerLeftLives = 3;
    public int playerRightLives = 3;

    [Header("Obje Referanslar�")]
    public BallSpawner ballSpawner;
    public Transform playerLeft;  // Oyuncunun kendisi (hareket eden)
    public Transform playerRight; // Oyuncunun kendisi (hareket eden)

    [Header("UI & Efekt Prefab'lar�")]
    public Transform mainCanvas;
    public GameObject lifeDisplayPrefab;
    public GameObject scorePopupPrefab;

    // Bunlar SADECE SKOR i�in kullan�lacak
    public Transform leftPopupAnchor;
    public Transform rightPopupAnchor;

    void Start()
    {
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();
    }

    /// <summary>
    /// SKOR POPUP'I ���N - ANCHOR KULLANIR
    /// </summary>
    public void AddPoint(Transform shootingPlayer)
    {
        string textToShow = ""; // Bo� metinle ba�la

        if (shootingPlayer == playerLeft)
        {
            playerLeftScore++;
            textToShow = $"{playerLeftScore} - {playerRightScore}";

            // SKOR'u sol 'Anchor'da g�ster
            ShowScorePopup(leftPopupAnchor, textToShow);
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightScore++;
            textToShow = $"{playerLeftScore} - {playerRightScore}";

            // SKOR'u sa� 'Anchor'da g�ster
            ShowScorePopup(rightPopupAnchor, textToShow);
        }

        Debug.Log($"SKOR: Sol Oyuncu: {playerLeftScore} - Sa� Oyuncu: {playerRightScore}");
        CheckForWin();
    }

    /// <summary>
    /// CAN BARLARI ���N - OYUNCUYU KULLANIR (D�ZELT�LD�)
    /// </summary>
    public void PlayerLosesLife(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft)
        {
            playerLeftLives--;

            // CAN BARINI 'Anchor'da DE��L, 'playerLeft' (oyuncunun kendisi) �zerinde g�ster
            ShowLifeDisplay(playerLeft, playerLeftLives);
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightLives--;

            // CAN BARINI 'Anchor'da DE��L, 'playerRight' (oyuncunun kendisi) �zerinde g�ster
            ShowLifeDisplay(playerRight, playerRightLives);
        }

        Debug.Log($"CAN KAYBED�LD�! Sol Can: {playerLeftLives} - Sa� Can: {playerRightLives}");
        CheckForWin();
    }

    // Bu fonksiyon hangi 'Transform'u verirsen (ister oyuncu, ister anchor)
    // onun pozisyonunda kalpleri olu�turur.
    void ShowLifeDisplay(Transform positionTransform, int currentLives)
    {
        if (lifeDisplayPrefab == null || mainCanvas == null)
        {
            Debug.LogWarning("LifeDisplayPrefab veya MainCanvas, GameManager'a atanmam��!");
            return;
        }
        GameObject displayObject = Instantiate(lifeDisplayPrefab, mainCanvas);
        PlayerLifeDisplay displayScript = displayObject.GetComponent<PlayerLifeDisplay>();
        if (displayScript != null)
        {
            displayScript.Initialize(currentLives, positionTransform);
        }
        else
        {
            Debug.LogError("LifeDisplayPrefab'�n �zerinde PlayerLifeDisplay script'i bulunamad�!");
        }
    }

    // Bu fonksiyon da hangi 'Transform'u verirsen (ister oyuncu, ister anchor)
    // onun pozisyonunda skoru olu�turur.
    void ShowScorePopup(Transform positionTransform, string text)
    {
        if (scorePopupPrefab == null || mainCanvas == null)
        {
            Debug.LogWarning("ScorePopupPrefab veya MainCanvas, GameManager'a atanmam��!");
            return;
        }
        GameObject popupObject = Instantiate(scorePopupPrefab, mainCanvas);
        ScorePopup popupScript = popupObject.GetComponent<ScorePopup>();
        if (popupScript != null)
        {
            popupScript.Initialize(positionTransform, text);
        }
        else
        {
            Debug.LogError("ScorePopupPrefab'�n �zerinde ScorePopup script'i bulunamad�!");
        }
    }

    // --- Kalan kodlar de�i�medi ---
    void CheckForWin()
    {
        if (playerLeftScore >= scoreToWin)
            Debug.Log("SOL OYUNCU KAZANDI! (Skor)");
        else if (playerRightScore >= scoreToWin)
            Debug.Log("SA� OYUNCU KAZANDI! (Skor)");

        if (playerLeftLives <= 0)
            Debug.Log("SA� OYUNCU KAZANDI! (Sol Oyuncu Can� Bitti)");
        else if (playerRightLives <= 0)
            Debug.Log("SOL OYUNCU KAZANDI! (Sa� Oyuncu Can� Bitti)");
    }

    public void RespawnBall(Transform shootingPlayer, ElementType ballType)
    {
        if (ballSpawner == null) return;

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