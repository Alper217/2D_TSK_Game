// SupporterGroup.cs (Yeni Hali)
using UnityEngine;
using System.Collections;

// Enum (Team) tanýmý hala gerekli, TribuneManager da kullanýyor
public enum Team { PlayerLeft, PlayerRight }

public class SupporterGroup : MonoBehaviour
{
    [Tooltip("Bu tribün grubu hangi takýmý destekliyor?")]
    public Team supportedTeam;

    // Artýk bir fan listesine gerek yok, sadece kendi orijinal pozisyonumuzu saklayacaðýz
    private Vector3 originalLocalPosition;

    void Awake()
    {
        // Kendi orijinal lokal pozisyonumuzu sakla (Global deðil, local)
        originalLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// TribuneManager tarafýndan çaðrýlýr.
    /// Tüm grubu (yani bu objeyi) belli bir süre zýplatýr/titretir.
    /// </summary>
    public void TriggerCelebration(float duration, float jumpHeight, float speed)
    {
        // Eðer zaten bir sevinç animasyonu çalýþýyorsa, eskisini durdurup yenisini baþlat
        // Bu, üst üste gol olunca animasyonun bozulmasýný engeller
        StopAllCoroutines();
        StartCoroutine(Co_Celebrate(duration, jumpHeight, speed));
    }

    private IEnumerator Co_Celebrate(float duration, float jumpHeight, float speed)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Sinüs dalgasý kullanarak yumuþak bir zýplama efekti
            // Artýk 'fans' listesi yok, doðrudan bu objenin transform'unu hareket ettir
            float yOffset = Mathf.Abs(Mathf.Sin(Time.time * speed)) * jumpHeight; // Zýplamanýn hep pozitif olmasý için Abs
            transform.localPosition = originalLocalPosition + new Vector3(0, yOffset, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // Süre bitince objeyi eski yerine al
        ResetPosition();
    }


    /// <summary>
    /// Meksika dalgasý için çaðrýlýr.
    /// NOT: Bu script artýk tek bir objeyi (grubu) zýplatabilir.
    /// Bu yüzden dalga efekti, tüm grubun sýrayla zýplamasý þeklinde olacak.
    /// </summary>
    public void TriggerWave(float groupStartDelay, float jumpHeight, float jumpDuration)
    {
        StartCoroutine(Co_Wave(groupStartDelay, jumpHeight, jumpDuration));
    }

    private IEnumerator Co_Wave(float groupStartDelay, float jumpHeight, float jumpDuration)
    {
        // Diðer gruplarýn bitirmesini beklemek için ana gecikme
        yield return new WaitForSeconds(groupStartDelay);

        // Bu objeyi (grubu) tekil olarak zýplat
        StartCoroutine(Co_SingleGroupJump(jumpHeight, jumpDuration));
    }

    /// <summary>
    /// Sadece bu grubu zýplatýp indiren coroutine.
    /// </summary>
    private IEnumerator Co_SingleGroupJump(float height, float duration)
    {
        float timer = 0f;
        Vector3 startPos = originalLocalPosition; // Baþlangýç pozisyonu

        while (timer < duration)
        {
            // (progress * PI) sinüs eðrisi 0'dan baþlar, 1'e (tepeye) çýkar ve 0'a (yere) iner.
            float progress = timer / duration;
            float yOffset = Mathf.Sin(progress * Mathf.PI) * height;
            transform.localPosition = startPos + new Vector3(0, yOffset, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // Bittiðinde tam olarak eski yerine oturt
        transform.localPosition = startPos;
    }

    /// <summary>
    /// Grubu orijinal pozisyonuna döndürür.
    /// </summary>
    public void ResetPosition()
    {
        transform.localPosition = originalLocalPosition;
    }
}