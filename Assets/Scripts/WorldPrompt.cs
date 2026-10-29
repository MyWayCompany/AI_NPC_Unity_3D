using UnityEngine;
using TMPro;

public class WorldPrompt : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public bool faceCamera = true;
    public float floatAmplitude = 0.05f;
    public float floatSpeed = 2f;

    private Camera mainCamera;
    private Vector3 basePosition;

    private void Awake()
    {
        mainCamera = Camera.main;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (faceCamera && mainCamera != null) 
            transform.rotation = mainCamera.transform.rotation;

        if (floatAmplitude > 0)
        {
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * floatSpeed) * floatAmplitude);

        }
    }

    public void Show(string text, Vector3 worldPosition)
    {
        basePosition = worldPosition;
        transform.position = worldPosition;
        promptText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
