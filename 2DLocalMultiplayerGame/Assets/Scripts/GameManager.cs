using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Skor ve Can Ayarlarý")]
    public int playerLeftScore = 0;
    public int playerRightScore = 0;
    public int scoreToWin = 2;
    public int playerLeftLives = 3;
    public int playerRightLives = 3;

    [Header("Obje Referanslarý")]
    public BallSpawner ballSpawner;
    public Transform playerLeft;  // Oyuncunun kendisi (hareket eden)
    public Transform playerRight; // Oyuncunun kendisi (hareket eden)

    [Header("UI & Efekt Prefab'larý")]
    public Transform mainCanvas;
    public GameObject lifeDisplayPrefab;
    public GameObject scorePopupPrefab;

    // Bunlar SADECE SKOR için kullanýlacak
    public Transform leftPopupAnchor;
    public Transform rightPopupAnchor;

    void Start()
    {
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();
    }

    /// <summary>
    /// SKOR POPUP'I ÝÇÝN - ANCHOR KULLANIR
    /// </summary>
    public void AddPoint(Transform shootingPlayer)
    {
        string textToShow = ""; // Boþ metinle baþla

        if (shootingPlayer == playerLeft)
        {
            playerLeftScore++;
            textToShow = $"{playerLeftScore} - {playerRightScore}";

            // SKOR'u sol 'Anchor'da göster
            ShowScorePopup(leftPopupAnchor, textToShow);
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightScore++;
            textToShow = $"{playerLeftScore} - {playerRightScore}";

            // SKOR'u sað 'Anchor'da göster
            ShowScorePopup(rightPopupAnchor, textToShow);
        }

        Debug.Log($"SKOR: Sol Oyuncu: {playerLeftScore} - Sað Oyuncu: {playerRightScore}");
        CheckForWin();
    }

    /// <summary>
    /// CAN BARLARI ÝÇÝN - OYUNCUYU KULLANIR (DÜZELTÝLDÝ)
    /// </summary>
    public void PlayerLosesLife(Transform shootingPlayer)
    {
        if (shootingPlayer == playerLeft)
        {
            playerLeftLives--;

            // CAN BARINI 'Anchor'da DEÐÝL, 'playerLeft' (oyuncunun kendisi) üzerinde göster
            ShowLifeDisplay(playerLeft, playerLeftLives);
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightLives--;

            // CAN BARINI 'Anchor'da DEÐÝL, 'playerRight' (oyuncunun kendisi) üzerinde göster
            ShowLifeDisplay(playerRight, playerRightLives);
        }

        Debug.Log($"CAN KAYBEDÝLDÝ! Sol Can: {playerLeftLives} - Sað Can: {playerRightLives}");
        CheckForWin();
    }

    // Bu fonksiyon hangi 'Transform'u verirsen (ister oyuncu, ister anchor)
    // onun pozisyonunda kalpleri oluþturur.
    void ShowLifeDisplay(Transform positionTransform, int currentLives)
    {
        if (lifeDisplayPrefab == null || mainCanvas == null)
        {
            Debug.LogWarning("LifeDisplayPrefab veya MainCanvas, GameManager'a atanmamýþ!");
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
            Debug.LogError("LifeDisplayPrefab'ýn üzerinde PlayerLifeDisplay script'i bulunamadý!");
        }
    }

    // Bu fonksiyon da hangi 'Transform'u verirsen (ister oyuncu, ister anchor)
    // onun pozisyonunda skoru oluþturur.
    void ShowScorePopup(Transform positionTransform, string text)
    {
        if (scorePopupPrefab == null || mainCanvas == null)
        {
            Debug.LogWarning("ScorePopupPrefab veya MainCanvas, GameManager'a atanmamýþ!");
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
            Debug.LogError("ScorePopupPrefab'ýn üzerinde ScorePopup script'i bulunamadý!");
        }
    }

    // --- Kalan kodlar deðiþmedi ---
    void CheckForWin()
    {
        if (playerLeftScore >= scoreToWin)
            Debug.Log("SOL OYUNCU KAZANDI! (Skor)");
        else if (playerRightScore >= scoreToWin)
            Debug.Log("SAÐ OYUNCU KAZANDI! (Skor)");

        if (playerLeftLives <= 0)
            Debug.Log("SAÐ OYUNCU KAZANDI! (Sol Oyuncu Caný Bitti)");
        else if (playerRightLives <= 0)
            Debug.Log("SOL OYUNCU KAZANDI! (Sað Oyuncu Caný Bitti)");
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