using UnityEngine;

public class AimIndicator : MonoBehaviour
{
    public float rotationSpeed = 100f;    // Okun d�nme h�z�
    public float minAngle = -45f;         // Minimum a��
    public float maxAngle = 45f;          // Maximum a��

    private float currentAngle = 0f;
    private bool rotatingRight = true;

    void Update()
    {
        // Ok s�rekli sa�a sola d�ns�n
        if (rotatingRight)
        {
            currentAngle += rotationSpeed * Time.deltaTime;
            if (currentAngle >= maxAngle)
            {
                currentAngle = maxAngle;
                rotatingRight = false;
            }
        }
        else
        {
            currentAngle -= rotationSpeed * Time.deltaTime;
            if (currentAngle <= minAngle)
            {
                currentAngle = minAngle;
                rotatingRight = true;
            }
        }

        // A��y� uygula
        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }

    public Vector2 GetAimDirection()
    {
        // Okun g�sterdi�i y�n� Vector2 olarak d�nd�r
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }
}