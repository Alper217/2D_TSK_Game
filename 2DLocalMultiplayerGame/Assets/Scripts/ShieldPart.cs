// ShieldPart.cs
using UnityEngine;

public class ShieldPart : MonoBehaviour
{
    public TargetOrb orb; // Bu parçaya ait hedef dairesi
    public Transform lineVisual; // Çizgi görselinin TRANSFORM'u

    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;

    public void SetBoundaries(float top, float bottom)
    {
        topBoundary = top;
        bottomBoundary = bottom;

        float height = top - bottom; // Parçanýn toplam yüksekliði
        float centerY = bottom + (height / 2);

        // 1. Ana objeyi segmentin merkezine taþý
        transform.position = new Vector3(0, centerY, 0);

        // 2. ÇÝZGÝYÝ UZAT (BOÞLUKLARI KAPATAN KOD)
        if (lineVisual != null)
        {
            lineVisual.localScale = new Vector3(lineVisual.localScale.x, height, lineVisual.localScale.z);
        }

        // 3. DAÝREYÝ HÝZALA
        if (orb != null)
        {
            orb.myPart = this;
            orb.transform.localPosition = Vector3.zero; // Ebeveynin merkezine sýfýrla
        }
    }

    // YENÝ: Hem dairenin hem de çizginin rengini ayarlar
    public void SetColor(Color newColor)
    {
        // ALFA DÜZELTMESÝ: Rengin þeffaflýðýný 1 (görünür) yap
        newColor.a = 1f;

        // Orb'un rengini ayarla
        if (orb != null)
        {
            orb.SetColor(newColor); // Orb'un kendi SetColor'ýný çaðýr
        }

        // Çizginin (stick) rengini ayarla
        if (lineVisual != null)
        {
            SpriteRenderer lineRenderer = lineVisual.GetComponent<SpriteRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.color = newColor;
            }
        }
    }
}