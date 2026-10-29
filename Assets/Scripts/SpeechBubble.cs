using UnityEngine;
using TMPro;
using System.Collections;

public class SpeechBubble : MonoBehaviour
{
    [Header("Настройки")]
    public TextMeshProUGUI bubbleText;
    public float charsPerSecond = 40f;
    public float holdTime = 2.5f;
    public bool faceCamera = true;

    public System.Action onTyped;

    private Coroutine routine;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (faceCamera && mainCamera != null)
            transform.rotation = mainCamera.transform.rotation;
    }

    public void Show(string text, bool autoHide = true)
    {
        gameObject.SetActive(true);
        if (routine != null)
            StopCoroutine(routine);        
        routine = StartCoroutine(TypeText(text, autoHide));
    }

    private IEnumerator TypeText(string text, bool autoHide)
    {
        bubbleText.text = "";
        float delay = 1f / Mathf.Max(1f, charsPerSecond);

        foreach (char c in text)
        {
            bubbleText.text += c;
            yield return new WaitForSeconds(delay);
        }

        onTyped?.Invoke();

        if (autoHide)
        {
            yield return new WaitForSeconds(holdTime + text.Length * 0.03f);
            Hide();
        }
    }

    public void Hide()
    {
        if (routine != null)
            StopCoroutine(routine);
        gameObject.SetActive(false);
    }
}
