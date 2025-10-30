using UnityEngine;
using UnityEngine.UI; // Button i�in bu gerekli
using UnityEngine.SceneManagement; // Sahne y�klemek i�in bu gerekli
using TMPro; // TextMeshPro kullan�yorsan�z bu gerekli

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Referanslar�")]
    [Tooltip("Gerekirse 'PLAYER 1 WINS!' olarak de�i�ecek ba�l�k")]
    public TextMeshProUGUI titleText;

    [Tooltip("Player 1'in skorunu g�steren '0' yaz�s�")]
    public TextMeshProUGUI player1ScoreText;

    [Tooltip("Player 2'nin skorunu g�steren '0' yaz�s�")]
    public TextMeshProUGUI player2ScoreText;

    [Tooltip("Yeniden ba�latma butonu")]
    public Button restartButton;

    void Start()
    {
        // Butona t�kland���nda hangi fonksiyonun �al��aca��n� ayarla
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    /// <summary>
    /// GameManager bu fonksiyonu �a��rarak ekran� kuracak
    /// </summary>
    public void Setup(string title, int p1Score, int p2Score)
    {
        // 1. Paneli g�r�n�r yap
        gameObject.SetActive(true);

        // 2. Ba�l��� ayarla (�rn: "PLAYER 1 WINS!")
        if (titleText != null)
        {
            titleText.text = title;
        }

        // 3. Skorlar� ayarla
        player1ScoreText.text = p1Score.ToString();
        player2ScoreText.text = p2Score.ToString();
    }

    /// <summary>
    /// Restart butonuna t�kland���nda �al���r
    /// </summary>
    public void RestartGame()
    {
        // Oyunu durdurduysak (Time.timeScale = 0), tekrar ba�lat
        Time.timeScale = 1f;

        // Mevcut sahneyi yeniden y�kle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}