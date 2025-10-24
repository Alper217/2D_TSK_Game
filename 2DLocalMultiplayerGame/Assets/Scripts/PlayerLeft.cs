using UnityEngine;

public class PlayerLeft : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    // Sol saha s�n�rlar�
    public float minX = -8f;
    public float maxX = -0.1f;
    public float minY = -4f;
    public float maxY = 4f;

    // Top sistemi
    public GameObject heldBall = null;
    public float pickupRange = 0.8f;
    public float shootPower = 10f;

    // Ok g�stergesi
    public GameObject aimIndicatorPrefab;
    private GameObject currentAimIndicator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // WASD ile hareket
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed;
        rb.linearVelocity = movement;

        // Pozisyonu s�n�rla
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

        // Top toplama
        if (heldBall == null)
        {
            TryPickupBall();
        }
        else
        {
            // Top tutuluyorsa ok g�stergesini g�ster
            if (currentAimIndicator == null)
            {
                ShowAimIndicator();
            }

            // SPACE tu�una bas�nca at
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShootBall();
            }
        }
    }

    void TryPickupBall()
    {
        // Ok g�stergesini kald�r
        if (currentAimIndicator != null)
        {
            Destroy(currentAimIndicator);
            currentAimIndicator = null;
        }

        // Yak�ndaki toplar� bul
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, pickupRange);

        foreach (Collider2D col in nearbyObjects)
        {
            if (col.CompareTag("Ball"))
            {
                Ball ballScript = col.GetComponent<Ball>();
                if (ballScript != null && ballScript.CanBePickedUp())  // ? BURASI DE���T�
                {
                    // Topu tut
                    heldBall = col.gameObject;
                    ballScript.isHeld = true;
                    ballScript.holder = this.transform;
                    break;
                }
            }
        }
    }

    void ShowAimIndicator()
    {
        if (aimIndicatorPrefab != null)
        {
            currentAimIndicator = Instantiate(aimIndicatorPrefab, transform.position, Quaternion.identity);
            currentAimIndicator.transform.SetParent(transform);
        }
    }

    void ShootBall()
    {
        if (heldBall != null && currentAimIndicator != null)
        {
            // Ok g�stergesinden at�� y�n�n� al
            AimIndicator aimScript = currentAimIndicator.GetComponent<AimIndicator>();
            if (aimScript != null)
            {
                Vector2 shootDirection = aimScript.GetAimDirection();

                // Topu at
                Ball ballScript = heldBall.GetComponent<Ball>();
                if (ballScript != null)
                {
                    ballScript.Shoot(shootDirection, shootPower);
                }

                heldBall = null;

                // Ok g�stergesini kald�r
                Destroy(currentAimIndicator);
                currentAimIndicator = null;
            }
        }
    }
}