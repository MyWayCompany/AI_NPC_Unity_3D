using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light light;
    public float baseIntensity = 5f;
    public float flickerSpeed = 8f;
    public float flickerAmount = 1.5f;

    private void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        light.intensity = baseIntensity + (noise - 0.5f) * 2f * flickerAmount;
    }

}
