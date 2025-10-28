using UnityEngine;
using UnityEngine.UI; // Slider ve Button i�in bu gerekli
using TMPro; // TextMeshPro kullan�yorsan bu gerekli
// E�er standart Text kullan�yorsan:
// using UnityEngine.UI; 
using System.Collections;

public class SettingsPageUI : MonoBehaviour
{
    [Header("Ses Slider'lar�")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;

    [Header("P1 Tu� Atama UI")]
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

    [Header("P2 Tu� Atama UI")]
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

    [Header("Di�er")]
    [Tooltip("Tu� atamas� s�ras�nda 'L�tfen bir tu�a bas�n...' diyen uyar� objesi")]
    public GameObject rebindPrompt; // "L�tfen bir tu�a bas�n..." uyar�s�

    private string keyToRebind = null; // Hangi tu�u atad���m�z� tutar
    private GameObject currentButtonTextObject; // Yaz�s�n� "..." olarak de�i�tirece�imiz buton

    // Bu script "settingsPage" objesinin �zerinde olaca�� i�in
    // OnEnable/OnDisable kullanmak Start'tan daha garantidir.
    void OnEnable()
    {
        // 1. SES AYARLARINI Y�KLE
        LoadAndSetupVolumeSliders();

        // 2. TU� ATAMALARINI Y�KLE
        SetupKeybindButtons();

        // 3. MEVCUT TU�LARI EKRANA YAZ
        UpdateKeybindUI();

        // 4. Tu� atama uyar�s�n� gizle
        if (rebindPrompt != null)
            rebindPrompt.SetActive(false);
    }

    void LoadAndSetupVolumeSliders()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogError("SettingsManager bulunamad�! _SettingsManager objesini Men� sahnesine ekledi�inizden emin olun.");
            return;
        }

        // --- Slider De�erlerini Y�kle ---
        // PlayerPrefs'ten kay�tl� de�erleri oku (0 olmamas� i�in 0.0001f yap)
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        effectVolumeSlider.value = PlayerPrefs.GetFloat("EffectVolume", 1f);

        // --- Slider Fonksiyonlar�n� Ba�la ---
        // Listener'lar� �nce temizle ki �st �ste eklenmesin
        //masterVolumeSlider.onValueChanged.RemoveAllListeners();
        //musicVolumeSlider.onValueChanged.RemoveAllListeners();
        //effectVolumeSlider.onValueChanged.RemoveAllListeners();

        // SettingsManager'daki fonksiyonlar� ba�la
        masterVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMusicVolume);
        effectVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetEffectVolume);
    }

    void SetupKeybindButtons()
    {
        // Butonlar�n listener'lar�n� temizle
        p1UpButton.onClick.RemoveAllListeners();
        p1DownButton.onClick.RemoveAllListeners();
        // ... (di�er t�m butonlar i�in) ...

        // Butonlara t�kland���nda hangi fonksiyonun �al��aca��n� ayarla
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
        // InputManager'daki mevcut tu�lar� al�p UI'daki Text'lere yaz
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

    // --- Tu� Atama ��lemi ---

    public void StartRebinding(string keyName, GameObject buttonTextObject)
    {
        keyToRebind = keyName;
        currentButtonTextObject = buttonTextObject; // Hangi text objesini "..." yapaca��m�z� sakla

        if (rebindPrompt != null)
            rebindPrompt.SetActive(true); // "Tu�a bas�n" uyar�s�n� g�ster

        // O anki butonun yaz�s�n� "..." yap
        if (currentButtonTextObject.GetComponent<TextMeshProUGUI>() != null)
            currentButtonTextObject.GetComponent<TextMeshProUGUI>().text = "...";
        // else if (currentButtonTextObject.GetComponent<Text>() != null)
        //     currentButtonTextObject.GetComponent<Text>().text = "...";

        // Tu� atamas� bitene kadar di�er tu�lar� dinlemeyi durdur
        StartCoroutine(WaitForInput());
    }

    IEnumerator WaitForInput()
    {
        // Bu Coroutine, bir tu�a bas�lana kadar her frame bekler
        while (keyToRebind != null)
        {
            if (Input.anyKeyDown)
            {
                // Bas�lan tu�u bul
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    // Fare t�klamalar�n� engelle (iste�e ba�l�)
                    if (keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2)
                        continue;

                    if (Input.GetKeyDown(keyCode))
                    {
                        // Escape tu�u i�lemi iptal etsin
                        if (keyCode == KeyCode.Escape)
                        {
                            keyToRebind = null; // Beklemeyi durdur
                            break; // foreach'i k�r
                        }

                        // Bulunan tu�u ata
                        InputManager.SetKey(keyToRebind, keyCode);
                        keyToRebind = null; // Beklemeyi durdur
                        break; // foreach'i k�r
                    }
                }
            }
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Atama bitti veya iptal edildi
        if (rebindPrompt != null)
            rebindPrompt.SetActive(false); // Uyar�y� gizle

        UpdateKeybindUI(); // UI'daki yaz�lar� (iptal edildiyse eski haline) g�ncelle
    }
}