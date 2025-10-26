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
    public Transform commonScoreAnchor; // YENÝ: Ortak skorun çýkacaðý nokta

    // --- YENÝ SES ALANI ---
    [Header("Ses Efektleri")]
    [Tooltip("Sayý alma, can kaybetme gibi efektleri çalacak olan AudioSource")]
    public AudioSource sfxSource;   // Inspector'dan ikinci AudioSource'u buraya sürükle
    [Tooltip("Oyuncu sayý kazandýðýnda çalacak ses")]
    public AudioClip successSound;  // Baþarý ses dosyaný buraya sürükle
    [Tooltip("Oyuncu can kaybettiðinde çalacak ses")]
    public AudioClip failureSound;  // Üzülme/can kaybetme ses dosyaný buraya sürükle
    // (Arka plan müziðini çalan AudioSource'u buraya eklemeye gerek yok, 
    // o zaten 'Play On Awake' ve 'Loop' ile kendi kendine çalýþacak)
    // --- SES ALANI BÝTTÝ ---


    void Start()
    {
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();

        // sfxSource atanmamýþsa, bu objenin üzerindeki (müzik hariç) ikinci AudioSource'u bulmayý dene
        if (sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1) // 1'den fazla varsa (ilki müzik varsayýlýr)
            {
                sfxSource = sources[1]; // Ýkinciyi SFX için kullan
                Debug.Log("SFX AudioSource otomatik olarak atandý.");
            }
            else if (sources.Length > 0) // Sadece 1 tane varsa onu ata ama uyar
            {
                sfxSource = sources[0];
                Debug.LogWarning("GameManager'da sadece 1 AudioSource bulundu. SFX ve Müzik çakýþabilir!");
            }
        }
    }

    /// <summary>
    /// SKOR POPUP'I ÝÇÝN - ARTIK ORTAK ANCHOR KULLANIR
    /// </summary>
    public void AddPoint(Transform shootingPlayer)
    {
        // 1. Skoru artýr
        if (shootingPlayer == playerLeft)
        {
            playerLeftScore++;
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightScore++;
        }

        // --- YENÝ SES KODU ---
        // Baþarý sesini çal (PlayOneShot, müziði durdurmadan çalar)
        if (sfxSource != null && successSound != null)
        {
            sfxSource.PlayOneShot(successSound);
        }
        // --- SES KODU BÝTTÝ ---

        // 2. Gösterilecek metni oluþtur
        string textToShow = $"{playerLeftScore} - {playerRightScore}";

        // 3. Skoru YENÝ ortak 'Anchor'da göster
        if (commonScoreAnchor != null)
        {
            ShowScorePopup(commonScoreAnchor, textToShow);
        }
        else
        {
            Debug.LogWarning("CommonScoreAnchor, GameManager'a atanmamýþ!");
        }

        Debug.Log($"SKOR: Sol Oyuncu: {playerLeftScore} - Sað Oyuncu: {playerRightScore}");
        CheckForWin();
    }

    /// <summary>
    /// CAN BARLARI ÝÇÝN - OYUNCUYU KULLANIR (Bu fonksiyon deðiþmedi)
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

        // --- YENÝ SES KODU ---
        // Baþarýsýzlýk/can kaybetme sesini çal
        if (sfxSource != null && failureSound != null)
        {
            sfxSource.PlayOneShot(failureSound);
        }
        // --- SES KODU BÝTTÝ ---

        Debug.Log($"CAN KAYBEDÝLDÝ! Sol Can: {playerLeftLives} - Sað Can: {playerRightLives}");
        CheckForWin();
    }

    // Bu fonksiyon deðiþmedi
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

    // Bu fonksiyon da deðiþmedi
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
            // ScorePopup script'i hangi transform'u verirsen onu takip eder.
            // Bu yüzden "commonScoreAnchor" vermemiz sorunsuz çalýþýr.
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