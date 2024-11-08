using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;
    private float shakeTimeRemaining = 0f;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            
            if (shakeTimeRemaining > 0)
            {
                smoothedPosition += (Vector3)Random.insideUnitCircle * shakeMagnitude;
                shakeTimeRemaining -= Time.deltaTime;
            }

            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, -10f);
        }
    }
    
    public void ShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimeRemaining = duration;
    }
}