// GameManager.cs (Güncellenmiþ Tam Hali)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // <-- YENÝ: Sahne yönetimi için eklendi

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
    public GameOverScreen gameOverScreen; // <-- YENÝ: Oyun sonu ekraný referansý

    [Header("Ses Efektleri")]
    public AudioSource sfxSource;
    public AudioClip successSound;
    public AudioClip failureSound;

    [Header("Meksika Dalgasý Otomasyonu")]
    [Tooltip("Skor alýnmazsa kaç saniye sonra dalga baþlasýn? (örn: 12)")]
    public float waveTriggerTime = 12.0f;
    [Tooltip("Dalga baþladýktan sonra bir sonrakinin tetiklenmesi için kaç saniye geçmeli? (örn: 20)")]
    public float waveCooldownTime = 20.0f;

    private float timeSinceLastScore = 0.0f;
    private bool isWaveOnCooldown = false;


    void Start()
    {
        // <-- YENÝ: Oyunun donmuþ olmadýðýndan (örn: bir önceki oyundan) emin ol
        Time.timeScale = 1f;

        // <-- YENÝ: Oyun baþýnda 'Game Over' ekranýný gizle
        if (gameOverScreen != null)
        {
            gameOverScreen.gameObject.SetActive(false);
        }

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

    void Update()
    {
        // 1. Manuel Test ('M' tuþu)
        if (Input.GetKeyDown(KeyCode.M) && !isWaveOnCooldown)
        {
            StartAutomaticWave();
        }

        // 2. Otomatik Zamanlayýcý
        if (!isWaveOnCooldown)
        {
            timeSinceLastScore += Time.deltaTime;

            if (timeSinceLastScore >= waveTriggerTime)
            {
                StartAutomaticWave();
            }
        }
    }


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

        // Skor alýndý, dalga zamanlayýcýsýný sýfýrla
        timeSinceLastScore = 0.0f;

        // 2. Gösterilecek metni oluþtur
        string textToShow = $"{playerLeftScore} - {playerRightScore}";

        // 3. Skoru göster
        if (commonScoreAnchor != null)
        {
            ShowScorePopup(commonScoreAnchor, textToShow);
        }

        CheckForWin(); // <-- ÖNEMLÝ: Puan alýndýktan sonra kazanma durumunu kontrol et
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

        // Can kaybetmek de skoru etkilediði için zamanlayýcýyý sýfýrlar
        timeSinceLastScore = 0.0f;

        CheckForWin(); // <-- ÖNEMLÝ: Can kaybettikten sonra kaybetme durumunu kontrol et
    }

    // --- YENÝ EKLENEN MEKSÝKA DALGASI FONKSÝYONLARI ---

    private void StartAutomaticWave()
    {
        if (tribuneManager == null) return;

        Debug.Log("Otomatik Meksika Dalgasý baþladý!");
        tribuneManager.StartMexicanWave();

        timeSinceLastScore = 0.0f;
        StartCoroutine(StartWaveCooldown());
    }

    private IEnumerator StartWaveCooldown()
    {
        isWaveOnCooldown = true;
        Debug.Log($"Meksika Dalgasý bekleme süresi baþladý ({waveCooldownTime} sn).");

        yield return new WaitForSeconds(waveCooldownTime);

        isWaveOnCooldown = false;
        timeSinceLastScore = 0.0f;
        Debug.Log("Meksika Dalgasý bekleme süresi bitti. Zamanlayýcý tekrar sayýyor.");
    }

    // --- Kalan Fonksiyonlar ---
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

    public void RespawnBall(Transform shootingPlayer, ElementType ballType)
    {
        // (Respawn logic...)
        // Bu fonksiyonun içeriði sizde mevcut, o yüzden dokunmadým.
    }

    // --- YENÝ EKLENEN OYUN SONU FONKSÝYONLARI ---

    /// <summary>
    /// Oyunun bitip bitmediðini kontrol eder.
    /// AddPoint ve PlayerLosesLife tarafýndan çaðrýlýr.
    /// </summary>
    void CheckForWin()
    {
        string winTitle = "";
        bool gameIsOver = false;

        // Kaybetme koþullarý (canlara göre)
        if (playerLeftLives <= 0)
        {
            winTitle = "PLAYER 2 WINS!";
            gameIsOver = true;
        }
        else if (playerRightLives <= 0)
        {
            winTitle = "PLAYER 1 WINS!";
            gameIsOver = true;
        }
        // Kazanma koþullarý (skora göre)
        else if (playerLeftScore >= scoreToWin)
        {
            winTitle = "PLAYER 1 WINS!";
            gameIsOver = true;
        }
        else if (playerRightScore >= scoreToWin)
        {
            winTitle = "PLAYER 2 WINS!";
            gameIsOver = true;
        }

        // Eðer oyun bittiyse, ilgili fonksiyonu çaðýr
        if (gameIsOver)
        {
            EndGame(winTitle);
        }
    }

    /// <summary>
    /// Oyun bittiðinde çaðrýlýr, her þeyi durdurur ve UI'ý gösterir.
    /// </summary>
    void EndGame(string title)
    {
        // --- YENÝ EKLENEN KOD BAÞLANGICI ---
        // Oyunu dondurmadan önce, sahnede kalan tüm popup'larý bul ve yok et.

        // "Popup" etiketine sahip tüm aktif Game Object'leri bir diziye al
        GameObject[] allPopups = GameObject.FindGameObjectsWithTag("Popup");

        // Bu dizideki her bir objeyi döngüye al
        foreach (GameObject popup in allPopups)
        {
            // Anýnda yok et
            Destroy(popup);
        }
        // --- YENÝ EKLENEN KOD BÝTTÝ ---


        // Oyunu dondur (fizik durur, Update'ler yavaþlar)
        Time.timeScale = 0f;

        // Eðer gameOverScreen atanmýþsa...
        if (gameOverScreen != null)
        {
            // GameOverScreen script'indeki Setup fonksiyonunu çaðýr
            // ve final skorlarý gönder
            gameOverScreen.Setup(title, playerLeftScore, playerRightScore);
        }
        else
        {
            Debug.LogError("Game Over Screen referansý GameManager'da atanmamýþ!");
        }
    }
}