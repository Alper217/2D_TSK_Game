using UnityEngine;
using UnityEngine.Audio; // AudioMixer'� kontrol etmek i�in bu sat�r �art

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public AudioMixer mainMixer;

    void Awake()
    {
        // --- Singleton Deseni ---
        // Sahnede zaten bir SettingsManager var m�?
        if (Instance == null)
        {
            // Yoksa, bu objeyi ana y�netici yap
            Instance = this;
            // Sahneler aras� ge�i�te bu objeyi koru
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Varsa (�rn: men�ye geri d�n�ld�yse), bu yeni (kopyay�) yok et
            Destroy(gameObject);
            return;
        }
        // --- Singleton Bitti ---

        // Oyunu ba�latt���m�zda kay�tl� ayarlar� y�kle
        LoadVolumeSettings();
    }

    // --- SES KONTROL ---

    // Bu fonksiyon Awake() taraf�ndan �a�r�l�r
    public void LoadVolumeSettings()
    {
        // PlayerPrefs'ten kay�tl� ayarlar� �ek (veya varsay�lan olarak 1 (full ses) kullan)
        // Not: Slider'lar 0.0001 ile 1 aras� �al��mal� (0 olmamal�)
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("EffectVolume", 1f);

        // De�erleri Mixer'a uygula
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetEffectVolume(sfx);

        // UI slider'lar� bu de�erleri SettingsPageUI script'inde (sonraki ad�mda) alacak
    }

    // UI Slider'lar bu fonksiyonlar� �a��racak
    public void SetMasterVolume(float volume)
    {
        // Mixer logaritmik (dB) �al���r, slider lineer (0-1).
        // Bu form�l lineer'i logaritmik'e �evirir.
        // volume = 0 ise -80dB (sessiz) olur.
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        // Ayar� kaydet
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetEffectVolume(float volume)
    {
        // "EffectVolume" ismini, AudioMixer'da verdi�iniz isimle (EffectVolume)
        // ayn� yazd���n�za emin olun.
        mainMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }
}