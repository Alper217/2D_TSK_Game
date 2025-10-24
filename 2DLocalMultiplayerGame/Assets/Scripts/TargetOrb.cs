// TargetOrb.cs
using UnityEngine;

public class TargetOrb : MonoBehaviour
{
    // YENÝ: Bu hedefin element tipi
    public ElementType orbType;

    public float maxHealth = 100f;
    private float currentHealth;
    public float moveSpeed = 2f;

    [HideInInspector] public ShieldManager manager;
    [HideInInspector] public ShieldPart myPart;

    private bool movingUp = true;
    private Vector3 initialScale;
    private SpriteRenderer orbRenderer;

    void Awake()
    {
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
        if (myPart == null) return;
        float newY = transform.position.y + (movingUp ? 1 : -1) * moveSpeed * Time.deltaTime;
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
            // YENÝDEN BOYUTLANMAYI BAÞLAT (ÝKÝNCÝ TEST ÝSTEÐÝN)
            manager.OnTargetDestroyed(myPart, shootingPlayer);
        }
    }

    public void SetColor(Color newColor)
    {
        if (orbRenderer != null)
        {
            newColor.a = 1f;
            orbRenderer.color = newColor;
        }
    }
}