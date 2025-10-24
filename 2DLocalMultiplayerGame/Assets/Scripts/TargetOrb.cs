// TargetOrb.cs
using UnityEngine;

public class TargetOrb : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float moveSpeed = 2f;

    [HideInInspector] public ShieldManager manager;
    [HideInInspector] public ShieldPart myPart; // Ebeveyn par�as�

    private bool movingUp = true;
    private Vector3 initialScale;
    private SpriteRenderer orbRenderer; // Renk i�in

    void Awake()
    {
        // Renk de�i�tirebilmek i�in SpriteRenderer'� al
        orbRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        initialScale = transform.localScale;
    }

    void Update()
    {
        MoveUpDown();
    }

    void MoveUpDown()
    {
        if (myPart == null) return; // Hen�z atanmad�ysa bekle

        // Yeni Y pozisyonunu hesapla
        float newY = transform.position.y + (movingUp ? 1 : -1) * moveSpeed * Time.deltaTime;

        // S�n�rlara ula�t� m�?
        if (newY >= myPart.topBoundary)
        {
            newY = myPart.topBoundary;
            movingUp = false;
        }
        else if (newY <= myPart.bottomBoundary)
        {
            newY = myPart.bottomBoundary;
            movingUp = true;
        }

        transform.position = new Vector3(myPart.transform.position.x, newY, 0);
    }

    public void TakeDamage(float amount, Transform shootingPlayer)
    {
        currentHealth -= amount;
        float scaleRatio = currentHealth / maxHealth;
        transform.localScale = initialScale * Mathf.Max(scaleRatio, 0.1f);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            manager.OnTargetDestroyed(myPart, shootingPlayer);
        }
    }

    // YEN�: Rengi ayarlar ve Alpha'y� 1 yapar
    public void SetColor(Color newColor)
    {
        if (orbRenderer != null)
        {
            // ALFA D�ZELTMES�: Rengin �effafl���n� 1 (g�r�n�r) yap
            newColor.a = 1f;
            orbRenderer.color = newColor;
        }
    }
}