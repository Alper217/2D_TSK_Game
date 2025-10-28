using UnityEngine;
using UnityEngine.UI; // Slider ve Button için bu gerekli
using TMPro; // TextMeshPro kullanýyorsan bu gerekli
// Eðer standart Text kullanýyorsan:
// using UnityEngine.UI; 
using System.Collections;

public class SettingsPageUI : MonoBehaviour
{
    [Header("Ses Slider'larý")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;

    [Header("P1 Tuþ Atama UI")]
    public Button p1UpButton;
    public TextMeshProUGUI p1UpText; // Veya: public Text p1UpText;
    public Button p1DownButton;
    public TextMeshProUGUI p1DownText; // Veya: public Text p1DownText;
    public Button p1LeftButton;
    public TextMeshProUGUI p1LeftText; // Veya: public Text p1LeftText;
    public Button p1RightButton;
    public TextMeshProUGUI p1RightText; // Veya: public Text p1RightText;
    public Button p1ShootButton;
    public TextMeshProUGUI p1ShootText; // Veya: public Text p1ShootText;

    [Header("P2 Tuþ Atama UI")]
    public Button p2UpButton;
    public TextMeshProUGUI p2UpText; // Veya: public Text p2UpText;
    public Button p2DownButton;
    public TextMeshProUGUI p2DownText; // Veya: public Text p2DownText;
    public Button p2LeftButton;
    public TextMeshProUGUI p2LeftText; // Veya: public Text p2LeftText;
    public Button p2RightButton;
    public TextMeshProUGUI p2RightText; // Veya: public Text p2RightText;
    public Button p2ShootButton;
    public TextMeshProUGUI p2ShootText; // Veya: public Text p2ShootText;

    [Header("Diðer")]
    [Tooltip("Tuþ atamasý sýrasýnda 'Lütfen bir tuþa basýn...' diyen uyarý objesi")]
    public GameObject rebindPrompt; // "Lütfen bir tuþa basýn..." uyarýsý

    private string keyToRebind = null; // Hangi tuþu atadýðýmýzý tutar
    private GameObject currentButtonTextObject; // Yazýsýný "..." olarak deðiþtireceðimiz buton

    // Bu script "settingsPage" objesinin üzerinde olacaðý için
    // OnEnable/OnDisable kullanmak Start'tan daha garantidir.
    void OnEnable()
    {
        // 1. SES AYARLARINI YÜKLE
        LoadAndSetupVolumeSliders();

        // 2. TUÞ ATAMALARINI YÜKLE
        SetupKeybindButtons();

        // 3. MEVCUT TUÞLARI EKRANA YAZ
        UpdateKeybindUI();

        // 4. Tuþ atama uyarýsýný gizle
        if (rebindPrompt != null)
            rebindPrompt.SetActive(false);
    }

    void LoadAndSetupVolumeSliders()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogError("SettingsManager bulunamadý! _SettingsManager objesini Menü sahnesine eklediðinizden emin olun.");
            return;
        }

        // --- Slider Deðerlerini Yükle ---
        // PlayerPrefs'ten kayýtlý deðerleri oku (0 olmamasý için 0.0001f yap)
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        effectVolumeSlider.value = PlayerPrefs.GetFloat("EffectVolume", 1f);

        // --- Slider Fonksiyonlarýný Baðla ---
        // Listener'larý önce temizle ki üst üste eklenmesin
        //masterVolumeSlider.onValueChanged.RemoveAllListeners();
        //musicVolumeSlider.onValueChanged.RemoveAllListeners();
        //effectVolumeSlider.onValueChanged.RemoveAllListeners();

        // SettingsManager'daki fonksiyonlarý baðla
        masterVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMusicVolume);
        effectVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetEffectVolume);
    }

    void SetupKeybindButtons()
    {
        // Butonlarýn listener'larýný temizle
        p1UpButton.onClick.RemoveAllListeners();
        p1DownButton.onClick.RemoveAllListeners();
        // ... (diðer tüm butonlar için) ...

        // Butonlara týklandýðýnda hangi fonksiyonun çalýþacaðýný ayarla
        p1UpButton.onClick.AddListener(() => StartRebinding("P1_Up", p1UpText.gameObject));
        p1DownButton.onClick.AddListener(() => StartRebinding("P1_Down", p1DownText.gameObject));
        p1LeftButton.onClick.AddListener(() => StartRebinding("P1_Left", p1LeftText.gameObject));
        p1RightButton.onClick.AddListener(() => StartRebinding("P1_Right", p1RightText.gameObject));
        p1ShootButton.onClick.AddListener(() => StartRebinding("P1_Shoot", p1ShootText.gameObject));

        p2UpButton.onClick.AddListener(() => StartRebinding("P2_Up", p2UpText.gameObject));
        p2DownButton.onClick.AddListener(() => StartRebinding("P2_Down", p2DownText.gameObject));
        p2LeftButton.onClick.AddListener(() => StartRebinding("P2_Left", p2LeftText.gameObject));
        p2RightButton.onClick.AddListener(() => StartRebinding("P2_Right", p2RightText.gameObject));
        p2ShootButton.onClick.AddListener(() => StartRebinding("P2_Shoot", p2ShootText.gameObject));
    }

    void UpdateKeybindUI()
    {
        // InputManager'daki mevcut tuþlarý alýp UI'daki Text'lere yaz
        p1UpText.text = InputManager.P1_Up.ToString();
        p1DownText.text = InputManager.P1_Down.ToString();
        p1LeftText.text = InputManager.P1_Left.ToString();
        p1RightText.text = InputManager.P1_Right.ToString();
        p1ShootText.text = InputManager.P1_Shoot.ToString();

        p2UpText.text = InputManager.P2_Up.ToString();
        p2DownText.text = InputManager.P2_Down.ToString();
        p2LeftText.text = InputManager.P2_Left.ToString();
        p2RightText.text = InputManager.P2_Right.ToString();
        p2ShootText.text = InputManager.P2_Shoot.ToString();
    }

    // --- Tuþ Atama Ýþlemi ---

    public void StartRebinding(string keyName, GameObject buttonTextObject)
    {
        keyToRebind = keyName;
        currentButtonTextObject = buttonTextObject; // Hangi text objesini "..." yapacaðýmýzý sakla

        if (rebindPrompt != null)
            rebindPrompt.SetActive(true); // "Tuþa basýn" uyarýsýný göster

        // O anki butonun yazýsýný "..." yap
        if (currentButtonTextObject.GetComponent<TextMeshProUGUI>() != null)
            currentButtonTextObject.GetComponent<TextMeshProUGUI>().text = "...";
        // else if (currentButtonTextObject.GetComponent<Text>() != null)
        //     currentButtonTextObject.GetComponent<Text>().text = "...";

        // Tuþ atamasý bitene kadar diðer tuþlarý dinlemeyi durdur
        StartCoroutine(WaitForInput());
    }

    IEnumerator WaitForInput()
    {
        // Bu Coroutine, bir tuþa basýlana kadar her frame bekler
        while (keyToRebind != null)
        {
            if (Input.anyKeyDown)
            {
                // Basýlan tuþu bul
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    // Fare týklamalarýný engelle (isteðe baðlý)
                    if (keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2)
                        continue;

                    if (Input.GetKeyDown(keyCode))
                    {
                        // Escape tuþu iþlemi iptal etsin
                        if (keyCode == KeyCode.Escape)
                        {
                            keyToRebind = null; // Beklemeyi durdur
                            break; // foreach'i kýr
                        }

                        // Bulunan tuþu ata
                        InputManager.SetKey(keyToRebind, keyCode);
                        keyToRebind = null; // Beklemeyi durdur
                        break; // foreach'i kýr
                    }
                }
            }
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Atama bitti veya iptal edildi
        if (rebindPrompt != null)
            rebindPrompt.SetActive(false); // Uyarýyý gizle

        UpdateKeybindUI(); // UI'daki yazýlarý (iptal edildiyse eski haline) güncelle
    }
}