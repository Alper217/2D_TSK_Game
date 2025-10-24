// ShieldPart.cs
using UnityEngine;

public class ShieldPart : MonoBehaviour
{
    public TargetOrb orb; // Bu par�aya ait hedef dairesi
    public Transform lineVisual; // �izgi g�rselinin TRANSFORM'u

    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;

    public void SetBoundaries(float top, float bottom)
    {
        topBoundary = top;
        bottomBoundary = bottom;

        float height = top - bottom; // Par�an�n toplam y�ksekli�i
        float centerY = bottom + (height / 2);

        // 1. Ana objeyi segmentin merkezine ta��
        transform.position = new Vector3(0, centerY, 0);

        // 2. ��ZG�Y� UZAT (BO�LUKLARI KAPATAN KOD)
        if (lineVisual != null)
        {
            lineVisual.localScale = new Vector3(lineVisual.localScale.x, height, lineVisual.localScale.z);
        }

        // 3. DA�REY� H�ZALA
        if (orb != null)
        {
            orb.myPart = this;
            orb.transform.localPosition = Vector3.zero; // Ebeveynin merkezine s�f�rla
        }
    }

    // YEN�: Hem dairenin hem de �izginin rengini ayarlar
    public void SetColor(Color newColor)
    {
        // ALFA D�ZELTMES�: Rengin �effafl���n� 1 (g�r�n�r) yap
        newColor.a = 1f;

        // Orb'un rengini ayarla
        if (orb != null)
        {
            orb.SetColor(newColor); // Orb'un kendi SetColor'�n� �a��r
        }

        // �izginin (stick) rengini ayarla
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