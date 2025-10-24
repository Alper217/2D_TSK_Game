// Ball.cs
using UnityEngine;

public class Ball : MonoBehaviour
{
    // YENİ: Bu topun element tipi
    public ElementType ballType;

    public bool isHeld = false;
    public Transform holder;
    public Transform lastHolder = null;
    public float damage = 34f;

    private Rigidbody2D rb;
    private Collider2D ballCollider;
    private GameManager gameManager;

    private float shootCooldown = 0f;
    public float cooldownTime = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<Collider2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (shootCooldown > 0) { shootCooldown -= Time.deltaTime; }
        if (isHeld && holder != null)
        {
            transform.position = holder.position;
            rb.linearVelocity = Vector2.zero;
            if (!rb.isKinematic) rb.isKinematic = true;
            Collider2D playerCollider = holder.GetComponent<Collider2D>();
            if (playerCollider != null) { Physics2D.IgnoreCollision(ballCollider, playerCollider, true); }
        }
    }

    public void Shoot(Vector2 direction, float power)
    {
        rb.isKinematic = false;
        if (holder != null)
        {
            Collider2D playerCollider = holder.GetComponent<Collider2D>();
            if (playerCollider != null) { Physics2D.IgnoreCollision(ballCollider, playerCollider, true); Invoke("ReenableCollision", 1f); }
        }
        isHeld = false;
        lastHolder = holder;
        holder = null;
        rb.linearVelocity = direction.normalized * power;
        shootCooldown = cooldownTime;
    }

    void ReenableCollision()
    {
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders) { if (col.CompareTag("Player")) { Physics2D.IgnoreCollision(ballCollider, col, false); } }
    }

    public bool CanBePickedUp() { return !isHeld && shootCooldown <= 0; }


    // GÜNCELLENDİ: ÇARPIŞMA MANTIĞI
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Hedefe çarptı mı?
        if (collision.gameObject.CompareTag("TargetOrb"))
        {
            // YENİ KURAL: Eğer topun tipi "Normal" ise...
            if (this.ballType == ElementType.Normal)
            {
                Debug.Log("Normal top sekti.");
                // Hiçbir şey yapma ve fonksiyondan çık.
                // Top yok edilmeyecek, sadece sekecek.
                return;
            }

            // --- Buradan sonrası sadece Element Tipi (Pembe, Mavi, Yeşil) olan toplar için çalışır ---

            TargetOrb orb = collision.gameObject.GetComponent<TargetOrb>();
            if (orb == null || lastHolder == null || gameManager == null) return;

            // 1. DOĞRU EŞLEŞME (Pembe top pembe alana vurdu)
            if (orb.orbType == this.ballType)
            {
                Debug.Log("DOĞRU VURUŞ! Hasar verildi.");
                orb.TakeDamage(damage, lastHolder);
                gameManager.RespawnBall(lastHolder, this.ballType); // Topu yeniden doğur
            }
            // 2. YANLIŞ EŞLEŞME (Pembe top mavi alana vurdu)
            else if (orb.orbType != ElementType.None)
            {
                Debug.Log("YANLIŞ VURUŞ! Can kaybedildi.");
                gameManager.PlayerLosesLife(lastHolder); // Can kaybet
                gameManager.RespawnBall(lastHolder, this.ballType); // Topu yeniden doğur
            }

            // Normal top HARİÇ, vuran topu her durumda yok et
            if (gameManager.ballSpawner != null)
                gameManager.ballSpawner.RemoveBall(gameObject);
            else
                Destroy(gameObject);
        }
    }
}