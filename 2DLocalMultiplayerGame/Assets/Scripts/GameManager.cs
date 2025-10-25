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

    // --- DE����KL�K BURADA ---
    // Eski anchor'lar yerine tek bir ortak anchor kullanaca��z.
    public Transform commonScoreAnchor; // YEN�: Ortak skorun ��kaca�� nokta

    // public Transform leftPopupAnchor; // ESK�: Art�k kullan�lm�yor
    // public Transform rightPopupAnchor; // ESK�: Art�k kullan�lm�yor
    // -------------------------

    void Start()
    {
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();
    }

    /// <summary>
    /// SKOR POPUP'I ���N - ARTIK ORTAK ANCHOR KULLANIR
    /// </summary>
    public void AddPoint(Transform shootingPlayer)
    {
        // 1. Skoru art�r
        if (shootingPlayer == playerLeft)
        {
            playerLeftScore++;
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightScore++;
        }

        // 2. G�sterilecek metni olu�tur
        string textToShow = $"{playerLeftScore} - {playerRightScore}";

        // 3. Skoru YEN� ortak 'Anchor'da g�ster
        if (commonScoreAnchor != null)
        {
            ShowScorePopup(commonScoreAnchor, textToShow);
        }
        else
        {
            Debug.LogWarning("CommonScoreAnchor, GameManager'a atanmam��!");
        }

        Debug.Log($"SKOR: Sol Oyuncu: {playerLeftScore} - Sa� Oyuncu: {playerRightScore}");
        CheckForWin();
    }

    /// <summary>
    /// CAN BARLARI ���N - OYUNCUYU KULLANIR (Bu fonksiyon de�i�medi)
    /// </summary>
    public void PlayerLosesLife(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft)
        {
            playerLeftLives--;
            ShowLifeDisplay(playerLeft, playerLeftLives);
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightLives--;
            ShowLifeDisplay(playerRight, playerRightLives);
        }

        Debug.Log($"CAN KAYBED�LD�! Sol Can: {playerLeftLives} - Sa� Can: {playerRightLives}");
        CheckForWin();
    }

    // Bu fonksiyon de�i�medi
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

    // Bu fonksiyon da de�i�medi
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
            // ScorePopup script'i hangi transform'u verirsen onu takip eder.
            // Bu y�zden "commonScoreAnchor" vermemiz sorunsuz �al���r.
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