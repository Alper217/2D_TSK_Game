// ShieldManager.cs
using UnityEngine;
using System.Collections.Generic;

public class ShieldManager : MonoBehaviour
{
    public GameObject shieldPartPrefab;
    public int initialPartCount = 3;
    public float totalHeight = 8f;
    public GameManager gameManager;
    public Color[] partColors;

    // YENÝ: Elle aþaðý/yukarý kaydýrmak için
    public float verticalOffset = 0f;

    public List<ShieldPart> activeParts = new List<ShieldPart>();

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        InitializeShields(initialPartCount);
    }

    void InitializeShields(int count)
    {
        foreach (ShieldPart part in activeParts) { Destroy(part.gameObject); }
        activeParts.Clear();

        float segmentHeight = totalHeight / count;

        // DÜZELTME: Baþlangýç pozisyonuna offset'i ekle
        float currentY_Bottom = (-totalHeight / 2) + verticalOffset;

        for (int i = 0; i < count; i++)
        {
            float partTop = currentY_Bottom + segmentHeight;
            float partBottom = currentY_Bottom;
            float partCenterY = currentY_Bottom + (segmentHeight / 2);

            Vector3 spawnPos = new Vector3(0, partCenterY, 0);
            GameObject partObj = Instantiate(shieldPartPrefab, spawnPos, Quaternion.identity);
            partObj.transform.SetParent(this.transform);

            ShieldPart part = partObj.GetComponent<ShieldPart>();
            part.SetBoundaries(partTop, partBottom);
            part.orb.manager = this;

            if (partColors.Length > i)
            {
                part.SetColor(partColors[i]);
            }

            activeParts.Add(part);
            currentY_Bottom += segmentHeight;
        }
    }

    public void OnTargetDestroyed(ShieldPart destroyedPart, Transform shootingPlayer)
    {
        activeParts.Remove(destroyedPart);
        Destroy(destroyedPart.gameObject);
        gameManager.AddPoint(shootingPlayer);

        if (activeParts.Count > 0)
        {
            RebalanceShields();
        }
    }

    void RebalanceShields()
    {
        int count = activeParts.Count;
        float segmentHeight = totalHeight / count;

        // DÜZELTME: Baþlangýç pozisyonuna offset'i ekle
        float currentY_Bottom = (-totalHeight / 2) + verticalOffset;

        for (int i = 0; i < count; i++)
        {
            ShieldPart part = activeParts[i];
            float partTop = currentY_Bottom + segmentHeight;
            float partBottom = currentY_Bottom;
            part.SetBoundaries(partTop, partBottom);

            currentY_Bottom += segmentHeight;
        }
    }
}