using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 50f;
    public float TimeDecreaseFactor = 1.0f, ShakeDecreaseFactor = 5f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null) camTransform = GetComponent(typeof(Transform)) as Transform;
    }

    void OnEnable() => originalPos = camTransform.localPosition;
    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * TimeDecreaseFactor;
            shakeAmount -= Time.deltaTime * ShakeDecreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            shakeAmount = 10f;
            camTransform.localPosition = originalPos;
        }
    }

    public void startShake(float time, float shakeAmout = 10f)
    {
        this.shakeDuration = time;
        this.shakeAmount = shakeAmout;
        ShakeDecreaseFactor = shakeAmout / time;
    }
}
