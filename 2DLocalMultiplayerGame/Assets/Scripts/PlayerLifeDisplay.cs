using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine (IEnumerator) kullanmak için bu kütüphane gerekli

/// <summary>
/// Oyuncunun can kaybettiðinde üzerinde beliren kalp göstergesini yönetir.
/// Bu script'in bir prefab üzerinde olmasý ve bu prefab'ýn 'GameManager' tarafýndan
/// bir 'World Space' Canvas içine 'Instantiate' edilmesi gerekir.
/// </summary>
public class PlayerLifeDisplay : MonoBehaviour
{
    [Header("Görseller (UI Images)")]
    // Inspector'da 3 kalp 'Image' objesini buraya sürükleyin
    public Image[] heartIcons;

    // Proje panelinden dolu kalp görselini buraya sürükleyin
    public Sprite fullHeartSprite;

    // Proje panelinden kýrýk kalp görselini buraya sürükleyin
    public Sprite brokenHeartSprite;

    [Header("Ayarlar")]
    [Tooltip("Kalplerin ekranda tam görünür olarak kalacaðý süre (kaybolma süresi hariç)")]
    public float displayDuration = 2.0f;

    [Tooltip("Kalplerin görünürlüðünü 1'den 0'a düþürmesinin ne kadar süreceði")]
    public float fadeOutTime = 0.5f;

    [Tooltip("Kalplerin, oyuncunun pozisyonuna göre ne kadar yukarýda/saðda/solda duracaðý")]
    public Vector3 offset = new Vector3(0, 1.2f, 0); // Oyuncunun 1.2 birim üstü

    private Transform playerToFollow; // Takip edilecek oyuncu (GameManager tarafýndan atanýr)
    private CanvasGroup canvasGroup;    // Yavaþça kaybolma (fade-out) efekti için kullanýlýr

    void Awake()
    {
        // Obje uyanýr uyanmaz CanvasGroup bileþenini bul veya ekle
        // Bu, tüm objenin (ve içindeki kalplerin) alfasýný (görünürlüðünü)
        // tek seferde kontrol etmemizi saðlar.
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Baþlangýçta tam görünür yap
        canvasGroup.alpha = 1.0f;
    }

    /// <summary>
    /// Bu fonksiyon GameManager tarafýndan prefab oluþturulduktan hemen sonra çaðrýlýr.
    /// </summary>
    /// <param name="currentLives">Oyuncunun kalan caný (örn: 2)</param>
    /// <param name="player">Hangi oyuncuyu takip edeceði</param>
    public void Initialize(int currentLives, Transform player)
    {
        playerToFollow = player;

        // Kalp görsellerini ayarla (örn: 2 dolu, 1 kýrýk)
        UpdateHearts(currentLives);

        // Ýlk pozisyonu hemen ayarla
        UpdatePosition();

        // Belirlenen süre sonra kaybolma iþlemini (Coroutine) baþlat
        StartCoroutine(FadeOutAndDestroy());
    }

    // Her frame çalýþýr
    void Update()
    {
        // Takip edilecek bir oyuncu atanmýþsa, pozisyonu güncelle
        if (playerToFollow != null)
        {
            UpdatePosition();
        }
    }

    /// <summary>
    /// Bu objenin pozisyonunu, takip edilen oyuncunun pozisyonuna + 'offset' deðerine eþitler.
    /// Bu fonksiyonun çalýþmasý için bu objenin bir 'World Space' Canvas içinde olmasý gerekir.
    /// </summary>
    void UpdatePosition()
    {
        if (playerToFollow == null) return;

        // Dünya uzayýndaki pozisyonumuzu, oyuncunun dünya uzayýndaki
        // pozisyonu + ayarladýðýmýz 'offset' deðeri yap.
        transform.position = playerToFollow.position + offset;
    }

    /// <summary>
    /// Kalan can sayýsýna göre kalp görsellerini (dolu/kýrýk) günceller.
    /// </summary>
    void UpdateHearts(int currentLives)
    {
        if (heartIcons == null || fullHeartSprite == null || brokenHeartSprite == null)
        {
            Debug.LogError("PlayerLifeDisplay: Kalp görselleri (Sprites) veya ikonlarý (Images) atanmamýþ!");
            return;
        }

        // 3 kalp üzerinden döngü
        for (int i = 0; i < heartIcons.Length; i++)
        {
            // 'i' -> mevcut kalbin indeksi (0, 1, veya 2)
            // 'currentLives' -> oyuncunun kalan caný (örn: 2)

            // Eðer kalp indeksi, kalan candan azsa (örn: 0 < 2 veya 1 < 2), bu kalp doludur.
            if (i < currentLives)
            {
                heartIcons[i].sprite = fullHeartSprite;
            }
            // Eðer kalp indeksi, kalan cana eþit veya fazlaysa (örn: 2 < 2 deðil), bu kalp kýrýktýr.
            else
            {
                heartIcons[i].sprite = brokenHeartSprite;
            }
        }
    }

    /// <summary>
    /// Önce 'displayDuration' kadar bekler, sonra 'fadeOutTime' süresi boyunca
    /// yavaþça kaybolur ve sonunda objeyi yok eder.
    /// </summary>
    IEnumerator FadeOutAndDestroy()
    {
        // 1. Aþama: Bekleme
        // 'displayDuration' süresi kadar bekle (eðer fadeOutTime 0.5 ise 2 - 0.5 = 1.5 saniye bekle)
        // Bu, toplam sürenin 'displayDuration' olmasýný saðlar.
        yield return new WaitForSeconds(displayDuration - fadeOutTime);

        // 2. Aþama: Yavaþça Kaybolma (Fade-out)
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            // Geçen süreyi artýr
            timer += Time.deltaTime;

            // CanvasGroup'un alfasýný 1'den 0'a doðru zamanla azalt
            // Lerp (Linear Interpolation), iki deðer arasýnda yumuþak geçiþ saðlar
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);

            // Bir sonraki frame'e kadar bekle
            yield return null;
        }

        // 3. Aþama: Yok Etme
        // Kaybolma bitti, bu objeyi (prefab'ý) sahneden sil
        Destroy(gameObject);
    }
}