// GameManager.cs (G�ncellenmi� Tam Hali)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // <-- YEN�: Sahne y�netimi i�in eklendi

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
    public GameOverScreen gameOverScreen; // <-- YEN�: Oyun sonu ekran� referans�

    [Header("Ses Efektleri")]
    public AudioSource sfxSource;
    public AudioClip successSound;
    public AudioClip failureSound;

    [Header("Meksika Dalgas� Otomasyonu")]
    [Tooltip("Skor al�nmazsa ka� saniye sonra dalga ba�las�n? (�rn: 12)")]
    public float waveTriggerTime = 12.0f;
    [Tooltip("Dalga ba�lad�ktan sonra bir sonrakinin tetiklenmesi i�in ka� saniye ge�meli? (�rn: 20)")]
    public float waveCooldownTime = 20.0f;

    private float timeSinceLastScore = 0.0f;
    private bool isWaveOnCooldown = false;


    void Start()
    {
        // <-- YEN�: Oyunun donmu� olmad���ndan (�rn: bir �nceki oyundan) emin ol
        Time.timeScale = 1f;

        // <-- YEN�: Oyun ba��nda 'Game Over' ekran�n� gizle
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
        // 1. Manuel Test ('M' tu�u)
        if (Input.GetKeyDown(KeyCode.M) && !isWaveOnCooldown)
        {
            StartAutomaticWave();
        }

        // 2. Otomatik Zamanlay�c�
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

        // Skor al�nd�, dalga zamanlay�c�s�n� s�f�rla
        timeSinceLastScore = 0.0f;

        // 2. G�sterilecek metni olu�tur
        string textToShow = $"{playerLeftScore} - {playerRightScore}";

        // 3. Skoru g�ster
        if (commonScoreAnchor != null)
        {
            ShowScorePopup(commonScoreAnchor, textToShow);
        }

        CheckForWin(); // <-- �NEML�: Puan al�nd�ktan sonra kazanma durumunu kontrol et
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

        // Can kaybetmek de skoru etkiledi�i i�in zamanlay�c�y� s�f�rlar
        timeSinceLastScore = 0.0f;

        CheckForWin(); // <-- �NEML�: Can kaybettikten sonra kaybetme durumunu kontrol et
    }

    // --- YEN� EKLENEN MEKS�KA DALGASI FONKS�YONLARI ---

    private void StartAutomaticWave()
    {
        if (tribuneManager == null) return;

        Debug.Log("Otomatik Meksika Dalgas� ba�lad�!");
        tribuneManager.StartMexicanWave();

        timeSinceLastScore = 0.0f;
        StartCoroutine(StartWaveCooldown());
    }

    private IEnumerator StartWaveCooldown()
    {
        isWaveOnCooldown = true;
        Debug.Log($"Meksika Dalgas� bekleme s�resi ba�lad� ({waveCooldownTime} sn).");

        yield return new WaitForSeconds(waveCooldownTime);

        isWaveOnCooldown = false;
        timeSinceLastScore = 0.0f;
        Debug.Log("Meksika Dalgas� bekleme s�resi bitti. Zamanlay�c� tekrar say�yor.");
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
        // Bu fonksiyonun i�eri�i sizde mevcut, o y�zden dokunmad�m.
    }

    // --- YEN� EKLENEN OYUN SONU FONKS�YONLARI ---

    /// <summary>
    /// Oyunun bitip bitmedi�ini kontrol eder.
    /// AddPoint ve PlayerLosesLife taraf�ndan �a�r�l�r.
    /// </summary>
    void CheckForWin()
    {
        string winTitle = "";
        bool gameIsOver = false;

        // Kaybetme ko�ullar� (canlara g�re)
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
        // Kazanma ko�ullar� (skora g�re)
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

        // E�er oyun bittiyse, ilgili fonksiyonu �a��r
        if (gameIsOver)
        {
            EndGame(winTitle);
        }
    }

    /// <summary>
    /// Oyun bitti�inde �a�r�l�r, her �eyi durdurur ve UI'� g�sterir.
    /// </summary>
    void EndGame(string title)
    {
        // --- YEN� EKLENEN KOD BA�LANGICI ---
        // Oyunu dondurmadan �nce, sahnede kalan t�m popup'lar� bul ve yok et.

        // "Popup" etiketine sahip t�m aktif Game Object'leri bir diziye al
        GameObject[] allPopups = GameObject.FindGameObjectsWithTag("Popup");

        // Bu dizideki her bir objeyi d�ng�ye al
        foreach (GameObject popup in allPopups)
        {
            // An�nda yok et
            Destroy(popup);
        }
        // --- YEN� EKLENEN KOD B�TT� ---


        // Oyunu dondur (fizik durur, Update'ler yava�lar)
        Time.timeScale = 0f;

        // E�er gameOverScreen atanm��sa...
        if (gameOverScreen != null)
        {
            // GameOverScreen script'indeki Setup fonksiyonunu �a��r
            // ve final skorlar� g�nder
            gameOverScreen.Setup(title, playerLeftScore, playerRightScore);
        }
        else
        {
            Debug.LogError("Game Over Screen referans� GameManager'da atanmam��!");
        }
    }
}