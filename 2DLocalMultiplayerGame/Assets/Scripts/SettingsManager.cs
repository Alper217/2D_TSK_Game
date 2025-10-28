using UnityEngine;
using UnityEngine.Audio; // AudioMixer'ý kontrol etmek için bu satýr þart

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public AudioMixer mainMixer;

    void Awake()
    {
        // --- Singleton Deseni ---
        // Sahnede zaten bir SettingsManager var mý?
        if (Instance == null)
        {
            // Yoksa, bu objeyi ana yönetici yap
            Instance = this;
            // Sahneler arasý geçiþte bu objeyi koru
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Varsa (örn: menüye geri dönüldüyse), bu yeni (kopyayý) yok et
            Destroy(gameObject);
            return;
        }
        // --- Singleton Bitti ---

        // Oyunu baþlattýðýmýzda kayýtlý ayarlarý yükle
        LoadVolumeSettings();
    }

    // --- SES KONTROL ---

    // Bu fonksiyon Awake() tarafýndan çaðrýlýr
    public void LoadVolumeSettings()
    {
        // PlayerPrefs'ten kayýtlý ayarlarý çek (veya varsayýlan olarak 1 (full ses) kullan)
        // Not: Slider'lar 0.0001 ile 1 arasý çalýþmalý (0 olmamalý)
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("EffectVolume", 1f);

        // Deðerleri Mixer'a uygula
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetEffectVolume(sfx);

        // UI slider'larý bu deðerleri SettingsPageUI script'inde (sonraki adýmda) alacak
    }

    // UI Slider'lar bu fonksiyonlarý çaðýracak
    public void SetMasterVolume(float volume)
    {
        // Mixer logaritmik (dB) çalýþýr, slider lineer (0-1).
        // Bu formül lineer'i logaritmik'e çevirir.
        // volume = 0 ise -80dB (sessiz) olur.
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        // Ayarý kaydet
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetEffectVolume(float volume)
    {
        // "EffectVolume" ismini, AudioMixer'da verdiðiniz isimle (EffectVolume)
        // ayný yazdýðýnýza emin olun.
        mainMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }
}