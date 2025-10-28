// GameManager.cs (G�ncellenmi� Hali)
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine i�in bu sat�r gerekli!

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

    [Tooltip("Sahnedeki TribuneManager script'ini buraya s�r�kleyin")]
    public TribuneManager tribuneManager;

    [Header("UI & Efekt Prefab'lar�")]
    public Transform mainCanvas;
    public GameObject lifeDisplayPrefab;
    public GameObject scorePopupPrefab;
    public Transform commonScoreAnchor;

    [Header("Ses Efektleri")]
    public AudioSource sfxSource;
    public AudioClip successSound;
    public AudioClip failureSound;

    // --- YEN� EKLENEN MEKS�KA DALGASI OTOMASYONU ---
    [Header("Meksika Dalgas� Otomasyonu")]
    [Tooltip("Skor al�nmazsa ka� saniye sonra dalga ba�las�n? (�rn: 12)")]
    public float waveTriggerTime = 12.0f;
    [Tooltip("Dalga ba�lad�ktan sonra bir sonrakinin tetiklenmesi i�in ka� saniye ge�meli? (�rn: 20)")]
    public float waveCooldownTime = 20.0f;

    private float timeSinceLastScore = 0.0f;
    private bool isWaveOnCooldown = false;
    // --- YEN� EKLENEN KOD B�TT� ---


    void Start()
    {
        if (ballSpawner == null)
            ballSpawner = FindObjectOfType<BallSpawner>();

        if (tribuneManager == null)
        {
            tribuneManager = FindObjectOfType<TribuneManager>();
        }

        // sfxSource bulma kodu...
        if (sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1) { sfxSource = sources[1]; }
            else if (sources.Length > 0) { sfxSource = sources[0]; }
        }
    }

    // --- YEN� EKLENEN UPDATE FONKS�YONU ---
    void Update()
    {
        // 1. Manuel Test ('M' tu�u)
        // 'M' tu�una bas�l�rsa VE bekleme modunda de�ilse dalgay� ba�lat
        if (Input.GetKeyDown(KeyCode.M) && !isWaveOnCooldown)
        {
            StartAutomaticWave();
        }

        // 2. Otomatik Zamanlay�c�
        // E�er dalga bekleme modunda de�ilse, skor zamanlay�c�s�n� art�r
        if (!isWaveOnCooldown)
        {
            timeSinceLastScore += Time.deltaTime;

            // Zamanlay�c�, tetikleme s�resini ge�ti mi?
            if (timeSinceLastScore >= waveTriggerTime)
            {
                StartAutomaticWave();
            }
        }
    }
    // --- YEN� EKLENEN KOD B�TT� ---


    /// <summary>
    /// SKOR POPUP'I ���N
    /// </summary>
    public void AddPoint(Transform shootingPlayer)
    {
        // 1. Skoru art�r
        if (shootingPlayer == playerLeft)
        {
            playerLeftScore++;
            if (tribuneManager != null)
                tribuneManager.TriggerCelebration(Team.PlayerLeft);
        }
        else if (shootingPlayer == playerRight)
        {
            playerRightScore++;
            if (tribuneManager != null)
                tribuneManager.TriggerCelebration(Team.PlayerRight);
        }

        // Ses kodu...
        if (sfxSource != null && successSound != null)
        {
            sfxSource.PlayOneShot(successSound);
        }

        // --- YEN� EKLENEN KOD ---
        // Skor al�nd�, dalga zamanlay�c�s�n� s�f�rla
        timeSinceLastScore = 0.0f;
        // --- YEN� EKLENEN KOD B�TT� ---

        // 2. G�sterilecek metni olu�tur
        string textToShow = $"{playerLeftScore} - {playerRightScore}";

        // 3. Skoru g�ster
        if (commonScoreAnchor != null)
        {
            ShowScorePopup(commonScoreAnchor, textToShow);
        }

        CheckForWin();
    }

    /// <summary>
    /// CAN BARLARI ���N
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

        // Ses kodu...
        if (sfxSource != null && failureSound != null)
        {
            sfxSource.PlayOneShot(failureSound);
        }

        // --- YEN� EKLENEN KOD ---
        // Can kaybetmek de skoru etkiledi�i i�in zamanlay�c�y� s�f�rlar
        timeSinceLastScore = 0.0f;
        // --- YEN� EKLENEN KOD B�TT� ---

        CheckForWin();
    }

    // --- YEN� EKLENEN MEKS�KA DALGASI FONKS�YONLARI ---

    // Bu fonksiyon hem manuel 'M' tu�uyla hem de otomatik zamanlay�c�yla �a�r�l�r
    private void StartAutomaticWave()
    {
        if (tribuneManager == null) return;

        Debug.Log("Otomatik Meksika Dalgas� ba�lad�!");
        tribuneManager.StartMexicanWave(); // TribuneManager'daki dalgay� ba�lat�r

        // Zamanlay�c�y� s�f�rla ve bekleme moduna ge�
        timeSinceLastScore = 0.0f;
        StartCoroutine(StartWaveCooldown());
    }

    // 20 saniyelik bekleme s�resini ba�latan Coroutine
    private IEnumerator StartWaveCooldown()
    {
        isWaveOnCooldown = true;
        Debug.Log($"Meksika Dalgas� bekleme s�resi ba�lad� ({waveCooldownTime} sn).");

        yield return new WaitForSeconds(waveCooldownTime);

        isWaveOnCooldown = false;
        timeSinceLastScore = 0.0f; // Cooldown bitti�inde, skor sayac�n� da s�f�rla
        Debug.Log("Meksika Dalgas� bekleme s�resi bitti. Zamanlay�c� tekrar say�yor.");
    }

    // --- YEN� EKLENEN KOD B�TT� ---


    // --- Kalan Fonksiyonlar (De�i�medi) ---
    void ShowLifeDisplay(Transform positionTransform, int currentLives)
    {
        if (lifeDisplayPrefab == null || mainCanvas == null) return;
        GameObject displayObject = Instantiate(lifeDisplayPrefab, mainCanvas);
        PlayerLifeDisplay displayScript = displayObject.GetComponent<PlayerLifeDisplay>();
        if (displayScript != null)
            displayScript.Initialize(currentLives, positionTransform);
    }

    void ShowScorePopup(Transform positionTransform, string text)
    {
        if (scorePopupPrefab == null || mainCanvas == null) return;
        GameObject popupObject = Instantiate(scorePopupPrefab, mainCanvas);
        ScorePopup popupScript = popupObject.GetComponent<ScorePopup>();
        if (popupScript != null)
            popupScript.Initialize(positionTransform, text);
    }

    void CheckForWin()
    {
        // (Win logic...)
    }

    public void RespawnBall(Transform shootingPlayer, ElementType ballType)
    {
        // (Respawn logic...)
    }
}