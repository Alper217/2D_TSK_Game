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

        float height = top - bottom; // Yeni y�kseklik
        float centerY = bottom + (height / 2);

        // 1. Ana objeyi segmentin merkezine ta��
        transform.position = new Vector3(0, centerY, 0);

        // 2. ��ZG�Y� UZAT (YEN�DEN BOYUTLANMA BURADA OLUR)
        if (lineVisual != null)
        {
            // �izginin Y-�l�e�ini, par�an�n yeni y�ksekli�ine e�itler
            lineVisual.localScale = new Vector3(lineVisual.localScale.x, height, lineVisual.localScale.z);
        }

        // 3. DA�REY� H�ZALA
        if (orb != null)
        {
            orb.myPart = this;
            orb.transform.localPosition = Vector3.zero; // Ebeveynin merkezine s�f�rla
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