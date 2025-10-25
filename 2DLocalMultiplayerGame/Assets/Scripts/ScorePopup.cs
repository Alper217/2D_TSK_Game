// Yeni bir C# scripti oluþturun ve adýný ScorePopup koyun
using UnityEngine;
using TMPro; // TextMeshPro kullanmak için bu kütüphane þart!
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    [Header("Bileþenler")]
    // Inspector'da prefab'ýn içindeki TextMeshPro objesini buraya sürükle
    public TextMeshProUGUI popupText;

    [Header("Ayarlar")]
    public float displayDuration = 1.5f; // Ekranda kalma süresi (can göstergesinden biraz daha kýsa)
    public float fadeOutTime = 0.5f;   // Yavaþça kaybolma süresi

    // Kalplerin biraz daha üzerinde çýksýn diye Y=1.5 (veya istediðin bir deðer)
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    // Ekstra efekt: Yazý yukarý doðru süzülsün
    public float moveUpSpeed = 0.5f;

    private Transform playerToFollow;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1.0f;
    }

    /// <summary>
    /// GameManager tarafýndan çaðrýlacak
    /// </summary>
    /// <param name="player">Takip edilecek oyuncu</param>
    /// <param name="text">Ekranda gösterilecek yazý (örn: "+1")</param>
    public void Initialize(Transform player, string text)
    {
        playerToFollow = player;
        popupText.text = text;

        UpdatePosition(); // Ýlk pozisyonu ayarla
        StartCoroutine(FadeOutAndDestroy());
    }

    void Update()
    {
        if (playerToFollow != null)
        {
            // Yazýyý yavaþça yukarý kaydýr
            offset.y += moveUpSpeed * Time.deltaTime;
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        if (playerToFollow == null) return;

        // Bu objenin pozisyonunu ayarla (World Space Canvas'ta çalýþýr)
        transform.position = playerToFollow.position + offset;
    }

    IEnumerator FadeOutAndDestroy()
    {
        // 1. Bekleme
        yield return new WaitForSeconds(displayDuration - fadeOutTime);

        // 2. Yavaþça Kaybolma
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            yield return null;
        }

        // 3. Yok Etme
        Destroy(gameObject);
    }
}