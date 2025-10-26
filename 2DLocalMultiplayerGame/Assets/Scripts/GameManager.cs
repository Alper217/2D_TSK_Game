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
    public Transform commonScoreAnchor; // YEN�: Ortak skorun ��kaca�� nokta

    // --- YEN� SES ALANI ---
    [Header("Ses Efektleri")]
    [Tooltip("Say� alma, can kaybetme gibi efektleri �alacak olan AudioSource")]
    public AudioSource sfxSource;   // Inspector'dan ikinci AudioSource'u buraya s�r�kle
    [Tooltip("Oyuncu say� kazand���nda �alacak ses")]
    public AudioClip successSound;  // Ba�ar� ses dosyan� buraya s�r�kle
    [Tooltip("Oyuncu can kaybetti�inde �alacak ses")]
    public AudioClip failureSound;  // �z�lme/can kaybetme ses dosyan� buraya s�r�kle
    // (Arka plan m�zi�ini �alan AudioSource'u buraya eklemeye gerek yok, 
    // o zaten 'Play On Awake' ve 'Loop' ile kendi kendine �al��acak)
    // --- SES ALANI B�TT� ---


    void Start()
    {
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();

        // sfxSource atanmam��sa, bu objenin �zerindeki (m�zik hari�) ikinci AudioSource'u bulmay� dene
        if (sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1) // 1'den fazla varsa (ilki m�zik varsay�l�r)
            {
                sfxSource = sources[1]; // �kinciyi SFX i�in kullan
                Debug.Log("SFX AudioSource otomatik olarak atand�.");
            }
            else if (sources.Length > 0) // Sadece 1 tane varsa onu ata ama uyar
            {
                sfxSource = sources[0];
                Debug.LogWarning("GameManager'da sadece 1 AudioSource bulundu. SFX ve M�zik �ak��abilir!");
            }
        }
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

        // --- YEN� SES KODU ---
        // Ba�ar� sesini �al (PlayOneShot, m�zi�i durdurmadan �alar)
        if (sfxSource != null && successSound != null)
        {
            sfxSource.PlayOneShot(successSound);
        }
        // --- SES KODU B�TT� ---

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

        // --- YEN� SES KODU ---
        // Ba�ar�s�zl�k/can kaybetme sesini �al
        if (sfxSource != null && failureSound != null)
        {
            sfxSource.PlayOneShot(failureSound);
        }
        // --- SES KODU B�TT� ---

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