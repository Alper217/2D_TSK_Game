// Yeni bir C# scripti olu�turun ve ad�n� ScorePopup koyun
using UnityEngine;
using TMPro; // TextMeshPro kullanmak i�in bu k�t�phane �art!
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    [Header("Bile�enler")]
    // Inspector'da prefab'�n i�indeki TextMeshPro objesini buraya s�r�kle
    public TextMeshProUGUI popupText;

    [Header("Ayarlar")]
    public float displayDuration = 1.5f; // Ekranda kalma s�resi (can g�stergesinden biraz daha k�sa)
    public float fadeOutTime = 0.5f;   // Yava��a kaybolma s�resi

    // Kalplerin biraz daha �zerinde ��ks�n diye Y=1.5 (veya istedi�in bir de�er)
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    // Ekstra efekt: Yaz� yukar� do�ru s�z�ls�n
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
    /// GameManager taraf�ndan �a�r�lacak
    /// </summary>
    /// <param name="player">Takip edilecek oyuncu</param>
    /// <param name="text">Ekranda g�sterilecek yaz� (�rn: "+1")</param>
    public void Initialize(Transform player, string text)
    {
        playerToFollow = player;
        popupText.text = text;

        UpdatePosition(); // �lk pozisyonu ayarla
        StartCoroutine(FadeOutAndDestroy());
    }

    void Update()
    {
        if (playerToFollow != null)
        {
            // Yaz�y� yava��a yukar� kayd�r
            offset.y += moveUpSpeed * Time.deltaTime;
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        if (playerToFollow == null) return;

        // Bu objenin pozisyonunu ayarla (World Space Canvas'ta �al���r)
        transform.position = playerToFollow.position + offset;
    }

    IEnumerator FadeOutAndDestroy()
    {
        // 1. Bekleme
        yield return new WaitForSeconds(displayDuration - fadeOutTime);

        // 2. Yava��a Kaybolma
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