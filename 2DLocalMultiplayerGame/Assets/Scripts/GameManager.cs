// GameManager.cs (Güncellenmiþ Hali)
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine için bu satýr gerekli!

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

    [Tooltip("Sahnedeki TribuneManager script'ini buraya sürükleyin")]
    public TribuneManager tribuneManager;

    [Header("UI & Efekt Prefab'larý")]
    public Transform mainCanvas;
    public GameObject lifeDisplayPrefab;
    public GameObject scorePopupPrefab;
    public Transform commonScoreAnchor;

    [Header("Ses Efektleri")]
    public AudioSource sfxSource;
    public AudioClip successSound;
    public AudioClip failureSound;

    // --- YENÝ EKLENEN MEKSÝKA DALGASI OTOMASYONU ---
    [Header("Meksika Dalgasý Otomasyonu")]
    [Tooltip("Skor alýnmazsa kaç saniye sonra dalga baþlasýn? (örn: 12)")]
    public float waveTriggerTime = 12.0f;
    [Tooltip("Dalga baþladýktan sonra bir sonrakinin tetiklenmesi için kaç saniye geçmeli? (örn: 20)")]
    public float waveCooldownTime = 20.0f;

    private float timeSinceLastScore = 0.0f;
    private bool isWaveOnCooldown = false;
    // --- YENÝ EKLENEN KOD BÝTTÝ ---


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

    // --- YENÝ EKLENEN UPDATE FONKSÝYONU ---
    void Update()
    {
        // 1. Manuel Test ('M' tuþu)
        // 'M' tuþuna basýlýrsa VE bekleme modunda deðilse dalgayý baþlat
        if (Input.GetKeyDown(KeyCode.M) && !isWaveOnCooldown)
        {
            StartAutomaticWave();
        }

        // 2. Otomatik Zamanlayýcý
        // Eðer dalga bekleme modunda deðilse, skor zamanlayýcýsýný artýr
        if (!isWaveOnCooldown)
        {
            timeSinceLastScore += Time.deltaTime;

            // Zamanlayýcý, tetikleme süresini geçti mi?
            if (timeSinceLastScore >= waveTriggerTime)
            {
                StartAutomaticWave();
            }
        }
    }
    // --- YENÝ EKLENEN KOD BÝTTÝ ---


    /// <summary>
    /// SKOR POPUP'I ÝÇÝN
    /// </summary>
    public void AddPoint(Transform shootingPlayer)
    {
        // 1. Skoru artýr
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

        // --- YENÝ EKLENEN KOD ---
        // Skor alýndý, dalga zamanlayýcýsýný sýfýrla
        timeSinceLastScore = 0.0f;
        // --- YENÝ EKLENEN KOD BÝTTÝ ---

        // 2. Gösterilecek metni oluþtur
        string textToShow = $"{playerLeftScore} - {playerRightScore}";

        // 3. Skoru göster
        if (commonScoreAnchor != null)
        {
            ShowScorePopup(commonScoreAnchor, textToShow);
        }

        CheckForWin();
    }

    /// <summary>
    /// CAN BARLARI ÝÇÝN
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

        // --- YENÝ EKLENEN KOD ---
        // Can kaybetmek de skoru etkilediði için zamanlayýcýyý sýfýrlar
        timeSinceLastScore = 0.0f;
        // --- YENÝ EKLENEN KOD BÝTTÝ ---

        CheckForWin();
    }

    // --- YENÝ EKLENEN MEKSÝKA DALGASI FONKSÝYONLARI ---

    // Bu fonksiyon hem manuel 'M' tuþuyla hem de otomatik zamanlayýcýyla çaðrýlýr
    private void StartAutomaticWave()
    {
        if (tribuneManager == null) return;

        Debug.Log("Otomatik Meksika Dalgasý baþladý!");
        tribuneManager.StartMexicanWave(); // TribuneManager'daki dalgayý baþlatýr

        // Zamanlayýcýyý sýfýrla ve bekleme moduna geç
        timeSinceLastScore = 0.0f;
        StartCoroutine(StartWaveCooldown());
    }

    // 20 saniyelik bekleme süresini baþlatan Coroutine
    private IEnumerator StartWaveCooldown()
    {
        isWaveOnCooldown = true;
        Debug.Log($"Meksika Dalgasý bekleme süresi baþladý ({waveCooldownTime} sn).");

        yield return new WaitForSeconds(waveCooldownTime);

        isWaveOnCooldown = false;
        timeSinceLastScore = 0.0f; // Cooldown bittiðinde, skor sayacýný da sýfýrla
        Debug.Log("Meksika Dalgasý bekleme süresi bitti. Zamanlayýcý tekrar sayýyor.");
    }

    // --- YENÝ EKLENEN KOD BÝTTÝ ---


    // --- Kalan Fonksiyonlar (Deðiþmedi) ---
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