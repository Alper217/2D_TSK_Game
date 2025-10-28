// SupporterGroup.cs (Yeni Hali)
using UnityEngine;
using System.Collections;

// Enum (Team) tan�m� hala gerekli, TribuneManager da kullan�yor
public enum Team { PlayerLeft, PlayerRight }

public class SupporterGroup : MonoBehaviour
{
    [Tooltip("Bu trib�n grubu hangi tak�m� destekliyor?")]
    public Team supportedTeam;

    // Art�k bir fan listesine gerek yok, sadece kendi orijinal pozisyonumuzu saklayaca��z
    private Vector3 originalLocalPosition;

    void Awake()
    {
        // Kendi orijinal lokal pozisyonumuzu sakla (Global de�il, local)
        originalLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// TribuneManager taraf�ndan �a�r�l�r.
    /// T�m grubu (yani bu objeyi) belli bir s�re z�plat�r/titretir.
    /// </summary>
    public void TriggerCelebration(float duration, float jumpHeight, float speed)
    {
        // E�er zaten bir sevin� animasyonu �al���yorsa, eskisini durdurup yenisini ba�lat
        // Bu, �st �ste gol olunca animasyonun bozulmas�n� engeller
        StopAllCoroutines();
        StartCoroutine(Co_Celebrate(duration, jumpHeight, speed));
    }

    private IEnumerator Co_Celebrate(float duration, float jumpHeight, float speed)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Sin�s dalgas� kullanarak yumu�ak bir z�plama efekti
            // Art�k 'fans' listesi yok, do�rudan bu objenin transform'unu hareket ettir
            float yOffset = Mathf.Abs(Mathf.Sin(Time.time * speed)) * jumpHeight; // Z�plaman�n hep pozitif olmas� i�in Abs
            transform.localPosition = originalLocalPosition + new Vector3(0, yOffset, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // S�re bitince objeyi eski yerine al
        ResetPosition();
    }


    /// <summary>
    /// Meksika dalgas� i�in �a�r�l�r.
    /// NOT: Bu script art�k tek bir objeyi (grubu) z�platabilir.
    /// Bu y�zden dalga efekti, t�m grubun s�rayla z�plamas� �eklinde olacak.
    /// </summary>
    public void TriggerWave(float groupStartDelay, float jumpHeight, float jumpDuration)
    {
        StartCoroutine(Co_Wave(groupStartDelay, jumpHeight, jumpDuration));
    }

    private IEnumerator Co_Wave(float groupStartDelay, float jumpHeight, float jumpDuration)
    {
        // Di�er gruplar�n bitirmesini beklemek i�in ana gecikme
        yield return new WaitForSeconds(groupStartDelay);

        // Bu objeyi (grubu) tekil olarak z�plat
        StartCoroutine(Co_SingleGroupJump(jumpHeight, jumpDuration));
    }

    /// <summary>
    /// Sadece bu grubu z�plat�p indiren coroutine.
    /// </summary>
    private IEnumerator Co_SingleGroupJump(float height, float duration)
    {
        float timer = 0f;
        Vector3 startPos = originalLocalPosition; // Ba�lang�� pozisyonu

        while (timer < duration)
        {
            // (progress * PI) sin�s e�risi 0'dan ba�lar, 1'e (tepeye) ��kar ve 0'a (yere) iner.
            float progress = timer / duration;
            float yOffset = Mathf.Sin(progress * Mathf.PI) * height;
            transform.localPosition = startPos + new Vector3(0, yOffset, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // Bitti�inde tam olarak eski yerine oturt
        transform.localPosition = startPos;
    }

    /// <summary>
    /// Grubu orijinal pozisyonuna d�nd�r�r.
    /// </summary>
    public void ResetPosition()
    {
        transform.localPosition = originalLocalPosition;
    }
}