using UnityEngine;
using UnityEngine.UI; // Button için bu gerekli
using UnityEngine.SceneManagement; // Sahne yüklemek için bu gerekli
using TMPro; // TextMeshPro kullanýyorsanýz bu gerekli

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Referanslarý")]
    [Tooltip("Gerekirse 'PLAYER 1 WINS!' olarak deðiþecek baþlýk")]
    public TextMeshProUGUI titleText;

    [Tooltip("Player 1'in skorunu gösteren '0' yazýsý")]
    public TextMeshProUGUI player1ScoreText;

    [Tooltip("Player 2'nin skorunu gösteren '0' yazýsý")]
    public TextMeshProUGUI player2ScoreText;

    [Tooltip("Yeniden baþlatma butonu")]
    public Button restartButton;

    void Start()
    {
        // Butona týklandýðýnda hangi fonksiyonun çalýþacaðýný ayarla
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    /// <summary>
    /// GameManager bu fonksiyonu çaðýrarak ekraný kuracak
    /// </summary>
    public void Setup(string title, int p1Score, int p2Score)
    {
        // 1. Paneli görünür yap
        gameObject.SetActive(true);

        // 2. Baþlýðý ayarla (örn: "PLAYER 1 WINS!")
        if (titleText != null)
        {
            titleText.text = title;
        }

        // 3. Skorlarý ayarla
        player1ScoreText.text = p1Score.ToString();
        player2ScoreText.text = p2Score.ToString();
    }

    /// <summary>
    /// Restart butonuna týklandýðýnda çalýþýr
    /// </summary>
    public void RestartGame()
    {
        // Oyunu durdurduysak (Time.timeScale = 0), tekrar baþlat
        Time.timeScale = 1f;

        // Mevcut sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}