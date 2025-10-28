using UnityEngine;

// Bu "static" bir s�n�ft�r, bir objeye eklenmez.
// Her yerden InputManager.P1_Up diyerek eri�ilir.
public static class InputManager
{
    // Oyuncu 1 Tu�lar�
    public static KeyCode P1_Up { get; set; }
    public static KeyCode P1_Down { get; set; }
    public static KeyCode P1_Left { get; set; }
    public static KeyCode P1_Right { get; set; }
    public static KeyCode P1_Shoot { get; set; }

    // Oyuncu 2 Tu�lar�
    public static KeyCode P2_Up { get; set; }
    public static KeyCode P2_Down { get; set; }
    public static KeyCode P2_Left { get; set; }
    public static KeyCode P2_Right { get; set; }
    public static KeyCode P2_Shoot { get; set; }

    // "Static Constructor" - Oyun ilk ba�lad���nda SADECE B�R KEZ �al���r.
    // (SettingsManager'daki Awake'ten bile �nce �al��abilir)
    static InputManager()
    {
        // Kay�tl� tu�lar� PlayerPrefs'ten y�kle VEYA varsay�lanlar� ata
        P1_Up = LoadKey("P1_Up", KeyCode.W);
        P1_Down = LoadKey("P1_Down", KeyCode.S);
        P1_Left = LoadKey("P1_Left", KeyCode.A);
        P1_Right = LoadKey("P1_Right", KeyCode.D);
        P1_Shoot = LoadKey("P1_Shoot", KeyCode.Space);

        P2_Up = LoadKey("P2_Up", KeyCode.UpArrow);
        P2_Down = LoadKey("P2_Down", KeyCode.DownArrow);
        P2_Left = LoadKey("P2_Left", KeyCode.LeftArrow);
        P2_Right = LoadKey("P2_Right", KeyCode.RightArrow);
        P2_Shoot = LoadKey("P2_Shoot", KeyCode.Return); // Return = Enter tu�u
    }

    // PlayerPrefs'ten veriyi �eken yard�mc� fonksiyon
    private static KeyCode LoadKey(string keyName, KeyCode defaultKey)
    {
        // Kay�tl� string'i �ek (�rn: "W")
        string keyString = PlayerPrefs.GetString(keyName, defaultKey.ToString());

        try
        {
            // String'i KeyCode'a �evir (�rn: KeyCode.W)
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyString);
        }
        catch (System.Exception)
        {
            // E�er kay�tl� veri bozuksa veya eski bir Unity versiyonundan kalm��sa
            // varsay�lan tu�u kullan
            Debug.LogWarning($"PlayerPrefs'te {keyName} i�in bozuk veri bulundu. Varsay�lan tu�a d�n�l�yor: {defaultKey}");
            return defaultKey;
        }
    }

    // Ayarlar men�s� bu fonksiyonu �a��racak
    public static void SetKey(string keyName, KeyCode newKey)
    {
        // Tu�u hem bu script'te (statik de�i�kende) g�ncelle
        switch (keyName)
        {
            case "P1_Up": P1_Up = newKey; break;
            case "P1_Down": P1_Down = newKey; break;
            case "P1_Left": P1_Left = newKey; break;
            case "P1_Right": P1_Right = newKey; break;
            case "P1_Shoot": P1_Shoot = newKey; break;

            case "P2_Up": P2_Up = newKey; break;
            case "P2_Down": P2_Down = newKey; break;
            case "P2_Left": P2_Left = newKey; break;
            case "P2_Right": P2_Right = newKey; break;
            case "P2_Shoot": P2_Shoot = newKey; break;
        }

        // Yeni tu�u PlayerPrefs'e de string olarak kaydet (�rn: "L")
        PlayerPrefs.SetString(keyName, newKey.ToString());
        PlayerPrefs.Save(); // Kayd� diske yazmay� garantile
    }
}