// Ball.cs
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isHeld = false;
    public Transform holder;
    public Transform lastHolder = null; // KİMİN ATTIĞINI TUTMAK İÇİN GÜNCELLENDİ
    public float damage = 34f; // Hedefe vereceği hasar (100/3 ≈ 34)

    private Rigidbody2D rb;
    private Collider2D ballCollider;
    private GameManager gameManager; // YENİ

    // Atıldıktan sonra bekleme süresi
    private float shootCooldown = 0f;
    public float cooldownTime = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<Collider2D>();
        gameManager = FindObjectOfType<GameManager>(); // YENİ
    }

    void Update()
    {
        if (shootCooldown > 0)
        {
            shootCooldown -= Time.deltaTime;
        }

        if (isHeld && holder != null)
        {
            transform.position = holder.position;
            rb.linearVelocity = Vector2.zero;
            if (!rb.isKinematic) rb.isKinematic = true;

            Collider2D playerCollider = holder.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(ballCollider, playerCollider, true);
            }
        }
    }

    public void Shoot(Vector2 direction, float power)
    {
        rb.isKinematic = false;

        if (holder != null)
        {
            Collider2D playerCollider = holder.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(ballCollider, playerCollider, true);
                Invoke("ReenableCollision", 1f);
            }
        }

        isHeld = false;
        lastHolder = holder; // GÜNCELLENDİ: 'Transform lastHolder' değil, class değişkeni
        holder = null;

        rb.linearVelocity = direction.normalized * power;
        shootCooldown = cooldownTime;
    }

    void ReenableCollision()
    {
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag("Player"))
            {
                Physics2D.IgnoreCollision(ballCollider, col, false);
            }
        }
    }

    public bool CanBePickedUp()
    {
        return !isHeld && shootCooldown <= 0;
    }

    // YENİ: ÇARPIŞMA MANTIĞI
    void OnCollisionEnter2D(Collision2D collision)
    {
        // "TargetOrb" tag'ine sahip bir şeye çarparsa
        if (collision.gameObject.CompareTag("TargetOrb"))
        {
            TargetOrb orb = collision.gameObject.GetComponent<TargetOrb>();
            if (orb != null && lastHolder != null) // lastHolder null değilse
            {
                // Hasar ver
                orb.TakeDamage(damage, lastHolder);

                // Topu yeniden doğur
                if (gameManager != null)
                {
                    gameManager.RespawnBall(lastHolder);
                }

                // Topu yok et
                if (gameManager.ballSpawner != null)
                {
                    // Spawner listesinden de kaldır
                    gameManager.ballSpawner.RemoveBall(gameObject);
                }
                else
                {
                    Destroy(gameObject); // Spawner yoksa direkt yok et
                }
            }
        }
    }
}