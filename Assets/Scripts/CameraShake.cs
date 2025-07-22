using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;

    private Transform cameraTransform;

    void Awake()
    {
        cameraTransform = GetComponent<Transform>();
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Vector3 pos = Random.insideUnitSphere * shakeMagnitude;
            cameraTransform.localPosition = new Vector3(pos.x, pos.y, -10);

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            cameraTransform.localPosition = Vector3.zero;
        }
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
