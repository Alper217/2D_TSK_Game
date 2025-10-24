using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isHeld = false;
    public Transform holder;

    private Rigidbody2D rb;
    private Collider2D ballCollider;

    // Atıldıktan sonra bekleme süresi
    private float shootCooldown = 0f;
    public float cooldownTime = 0.5f;  // 0.5 saniye bekleme

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Cooldown'u azalt
        if (shootCooldown > 0)
        {
            shootCooldown -= Time.deltaTime;
        }

        // Eğer tutuluyorsa, tutan oyuncunun yanında kal
        if (isHeld && holder != null)
        {
            transform.position = holder.position;
            rb.linearVelocity = Vector2.zero;

            if (!rb.isKinematic)
            {
                rb.isKinematic = true;
            }

            // Tutarken oyuncu ile çarpışmayı kapat
            Collider2D playerCollider = holder.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(ballCollider, playerCollider, true);
            }
        }
    }

    public void Shoot(Vector2 direction, float power)
    {
        // Önce kinematic'i kapat (fizik tekrar aktif)
        rb.isKinematic = false;

        // Atan oyuncuyla çarpışmayı geçici olarak kapat
        if (holder != null)
        {
            Collider2D playerCollider = holder.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(ballCollider, playerCollider, true);

                // 1 saniye sonra tekrar çarpışmayı aç
                Invoke("ReenableCollision", 1f);
            }
        }

        isHeld = false;
        Transform lastHolder = holder;
        holder = null;

        // Atış yönünü uygula
        rb.linearVelocity = direction.normalized * power;

        // Cooldown başlat (hemen tekrar tutulmasın)
        shootCooldown = cooldownTime;
    }

    void ReenableCollision()
    {
        // Tüm oyuncularla tekrar çarpışmayı aç
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag("Player"))
            {
                Physics2D.IgnoreCollision(ballCollider, col, false);
            }
        }
    }

    // Top atıldıktan sonra hemen tutulmasın
    public bool CanBePickedUp()
    {
        return !isHeld && shootCooldown <= 0;
    }
}