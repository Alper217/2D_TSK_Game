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
    public float verticalOffset = 0f; // Elle kaydýrma

    // YENÝ: Parçalarýn tiplerini belirleyen dizi
    public ElementType[] partTypes;

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

        float segmentHeight = totalHeight / count; // 1. Deðiþkenin doðru adý bu
        float currentY_Bottom = (-totalHeight / 2) + verticalOffset;

        for (int i = 0; i < count; i++)
        {
            float partTop = currentY_Bottom + segmentHeight;
            float partBottom = currentY_Bottom;
            // DÜZELTME: 'height' yerine 'segmentHeight' kullanýlmalý
            float partCenterY = currentY_Bottom + (segmentHeight / 2);
            Vector3 spawnPos = new Vector3(0, partCenterY, 0);
            GameObject partObj = Instantiate(shieldPartPrefab, spawnPos, Quaternion.identity);
            partObj.transform.SetParent(this.transform);

            ShieldPart part = partObj.GetComponent<ShieldPart>();
            part.SetBoundaries(partTop, partBottom);
            part.orb.manager = this;

            // Renkleri ata
            if (partColors.Length > i)
            {
                part.SetColor(partColors[i]);
            }

            // YENÝ: Tipleri ata
            if (partTypes.Length > i)
            {
                part.SetType(partTypes[i]);
            }

            // DÜZELTME: 'activeBalls' yerine 'activeParts' olmalý
            activeParts.Add(part);
            currentY_Bottom += segmentHeight;
        }
    }

    // HEDEF YOK OLDUÐUNDA ÇAÐRILIR
    public void OnTargetDestroyed(ShieldPart destroyedPart, Transform shootingPlayer)
    {
        activeParts.Remove(destroyedPart);
        Destroy(destroyedPart.gameObject);

        gameManager.AddPoint(shootingPlayer); // Puan ekle

        // YENÝDEN BOYUTLANMAYI ÇAÐIR
        if (activeParts.Count > 0)
        {
            RebalanceShields();
        }
    }

    // KALANLARI YENÝDEN BOYUTLANDIR
    void RebalanceShields()
    {
        int count = activeParts.Count;
        float segmentHeight = totalHeight / count;
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