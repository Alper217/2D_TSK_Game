using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine (IEnumerator) kullanmak i�in bu k�t�phane gerekli

/// <summary>
/// Oyuncunun can kaybetti�inde �zerinde beliren kalp g�stergesini y�netir.
/// Bu script'in bir prefab �zerinde olmas� ve bu prefab'�n 'GameManager' taraf�ndan
/// bir 'World Space' Canvas i�ine 'Instantiate' edilmesi gerekir.
/// </summary>
public class PlayerLifeDisplay : MonoBehaviour
{
    [Header("G�rseller (UI Images)")]
    // Inspector'da 3 kalp 'Image' objesini buraya s�r�kleyin
    public Image[] heartIcons;

    // Proje panelinden dolu kalp g�rselini buraya s�r�kleyin
    public Sprite fullHeartSprite;

    // Proje panelinden k�r�k kalp g�rselini buraya s�r�kleyin
    public Sprite brokenHeartSprite;

    [Header("Ayarlar")]
    [Tooltip("Kalplerin ekranda tam g�r�n�r olarak kalaca�� s�re (kaybolma s�resi hari�)")]
    public float displayDuration = 2.0f;

    [Tooltip("Kalplerin g�r�n�rl���n� 1'den 0'a d���rmesinin ne kadar s�rece�i")]
    public float fadeOutTime = 0.5f;

    [Tooltip("Kalplerin, oyuncunun pozisyonuna g�re ne kadar yukar�da/sa�da/solda duraca��")]
    public Vector3 offset = new Vector3(0, 1.2f, 0); // Oyuncunun 1.2 birim �st�

    private Transform playerToFollow; // Takip edilecek oyuncu (GameManager taraf�ndan atan�r)
    private CanvasGroup canvasGroup;    // Yava��a kaybolma (fade-out) efekti i�in kullan�l�r

    void Awake()
    {
        // Obje uyan�r uyanmaz CanvasGroup bile�enini bul veya ekle
        // Bu, t�m objenin (ve i�indeki kalplerin) alfas�n� (g�r�n�rl���n�)
        // tek seferde kontrol etmemizi sa�lar.
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Ba�lang��ta tam g�r�n�r yap
        canvasGroup.alpha = 1.0f;
    }

    /// <summary>
    /// Bu fonksiyon GameManager taraf�ndan prefab olu�turulduktan hemen sonra �a�r�l�r.
    /// </summary>
    /// <param name="currentLives">Oyuncunun kalan can� (�rn: 2)</param>
    /// <param name="player">Hangi oyuncuyu takip edece�i</param>
    public void Initialize(int currentLives, Transform player)
    {
        playerToFollow = player;

        // Kalp g�rsellerini ayarla (�rn: 2 dolu, 1 k�r�k)
        UpdateHearts(currentLives);

        // �lk pozisyonu hemen ayarla
        UpdatePosition();

        // Belirlenen s�re sonra kaybolma i�lemini (Coroutine) ba�lat
        StartCoroutine(FadeOutAndDestroy());
    }

    // Her frame �al���r
    void Update()
    {
        // Takip edilecek bir oyuncu atanm��sa, pozisyonu g�ncelle
        if (playerToFollow != null)
        {
            UpdatePosition();
        }
    }

    /// <summary>
    /// Bu objenin pozisyonunu, takip edilen oyuncunun pozisyonuna + 'offset' de�erine e�itler.
    /// Bu fonksiyonun �al��mas� i�in bu objenin bir 'World Space' Canvas i�inde olmas� gerekir.
    /// </summary>
    void UpdatePosition()
    {
        if (playerToFollow == null) return;

        // D�nya uzay�ndaki pozisyonumuzu, oyuncunun d�nya uzay�ndaki
        // pozisyonu + ayarlad���m�z 'offset' de�eri yap.
        transform.position = playerToFollow.position + offset;
    }

    /// <summary>
    /// Kalan can say�s�na g�re kalp g�rsellerini (dolu/k�r�k) g�nceller.
    /// </summary>
    void UpdateHearts(int currentLives)
    {
        if (heartIcons == null || fullHeartSprite == null || brokenHeartSprite == null)
        {
            Debug.LogError("PlayerLifeDisplay: Kalp g�rselleri (Sprites) veya ikonlar� (Images) atanmam��!");
            return;
        }

        // 3 kalp �zerinden d�ng�
        for (int i = 0; i < heartIcons.Length; i++)
        {
            // 'i' -> mevcut kalbin indeksi (0, 1, veya 2)
            // 'currentLives' -> oyuncunun kalan can� (�rn: 2)

            // E�er kalp indeksi, kalan candan azsa (�rn: 0 < 2 veya 1 < 2), bu kalp doludur.
            if (i < currentLives)
            {
                heartIcons[i].sprite = fullHeartSprite;
            }
            // E�er kalp indeksi, kalan cana e�it veya fazlaysa (�rn: 2 < 2 de�il), bu kalp k�r�kt�r.
            else
            {
                heartIcons[i].sprite = brokenHeartSprite;
            }
        }
    }

    /// <summary>
    /// �nce 'displayDuration' kadar bekler, sonra 'fadeOutTime' s�resi boyunca
    /// yava��a kaybolur ve sonunda objeyi yok eder.
    /// </summary>
    IEnumerator FadeOutAndDestroy()
    {
        // 1. A�ama: Bekleme
        // 'displayDuration' s�resi kadar bekle (e�er fadeOutTime 0.5 ise 2 - 0.5 = 1.5 saniye bekle)
        // Bu, toplam s�renin 'displayDuration' olmas�n� sa�lar.
        yield return new WaitForSeconds(displayDuration - fadeOutTime);

        // 2. A�ama: Yava��a Kaybolma (Fade-out)
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            // Ge�en s�reyi art�r
            timer += Time.deltaTime;

            // CanvasGroup'un alfas�n� 1'den 0'a do�ru zamanla azalt
            // Lerp (Linear Interpolation), iki de�er aras�nda yumu�ak ge�i� sa�lar
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);

            // Bir sonraki frame'e kadar bekle
            yield return null;
        }

        // 3. A�ama: Yok Etme
        // Kaybolma bitti, bu objeyi (prefab'�) sahneden sil
        Destroy(gameObject);
    }
}