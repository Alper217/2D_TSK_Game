using UnityEngine;

public class ShieldPart : MonoBehaviour
{
    public TargetOrb orb;
    public Transform lineVisual;

    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;

    public void SetBoundaries(float top, float bottom)
    {
        topBoundary = top;
        bottomBoundary = bottom;

        float height = top - bottom; // Yeni yükseklik
        float centerY = bottom + (height / 2);

        // 1. Ana objeyi segmentin merkezine taþý
        transform.position = new Vector3(0, centerY, 0);

        // 2. ÇÝZGÝYÝ UZAT (YENÝDEN BOYUTLANMA BURADA OLUR)
        if (lineVisual != null)
        {
            // Çizginin Y-ölçeðini, parçanýn yeni yüksekliðine eþitler
            lineVisual.localScale = new Vector3(lineVisual.localScale.x, height, lineVisual.localScale.z);
        }

        // 3. DAÝREYÝ HÝZALA
        if (orb != null)
        {
            orb.myPart = this;
            orb.transform.localPosition = Vector3.zero; // Ebeveynin merkezine sýfýrla
        }
    }

    public void SetColor(Color newColor)
    {
        newColor.a = 1f;
        if (orb != null)
        {
            orb.SetColor(newColor);
        }
        if (lineVisual != null)
        {
            SpriteRenderer lineRenderer = lineVisual.GetComponent<SpriteRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.color = newColor;
            }
        }
    }
    public void SetType(ElementType type)
    {
        if (orb != null)
        {
            orb.orbType = type;
        }
    }
}