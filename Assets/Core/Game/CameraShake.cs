using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraShake : MonoBehaviour
{
    Vector3 basePos;
    float timeLeft;
    float intensity;

    void Awake() { basePos = transform.localPosition; }

    void LateUpdate()
    {
        if (timeLeft > 0f)
        {
            timeLeft -= Time.unscaledDeltaTime;
            float t = timeLeft;
            float x = (Mathf.PerlinNoise(0, Time.time * 50f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(1, Time.time * 50f) - 0.5f) * 2f;
            transform.localPosition = basePos + new Vector3(x, y, 0) * intensity * t;
            if (timeLeft <= 0f) transform.localPosition = basePos;
        }
    }

    public void Shake(float duration, float amp)
    {
        timeLeft = duration;
        intensity = amp;
    }
}
