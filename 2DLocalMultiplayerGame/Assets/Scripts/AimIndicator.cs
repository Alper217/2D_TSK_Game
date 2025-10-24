using UnityEngine;

public class AimIndicator : MonoBehaviour
{
    public float rotationSpeed = 100f;    // Okun dönme hýzý
    public float minAngle = -45f;         // Minimum açý
    public float maxAngle = 45f;          // Maximum açý

    private float currentAngle = 0f;
    private bool rotatingRight = true;

    void Update()
    {
        // Ok sürekli saða sola dönsün
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

        // Açýyý uygula
        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }

    public Vector2 GetAimDirection()
    {
        // Okun gösterdiði yönü Vector2 olarak döndür
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }
}