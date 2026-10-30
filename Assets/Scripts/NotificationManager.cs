using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("Настройки")]
    public Transform notificationsParent;  // NotificationsPanel
    public GameObject notificationPrefab;  // NotificationPrefab
    public float displayTime = 3f;
    public float fadeTime = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemAdded += HandleItemAdded;
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemAdded -= HandleItemAdded;
    }

    private void HandleItemAdded(string itemId, int count)
    {
        var info = ItemDatabase.Get(itemId);
        Show($"{info.displayName} +{count}");
    }

    public void Show(string text)
    {
        if (notificationPrefab == null || notificationsParent == null) return;

        GameObject go = Instantiate(notificationPrefab, notificationsParent);

        var tmp = go.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;            
            tmp.ForceMeshUpdate();

            // Размер одной строки без ограничений ширины
            Vector2 preferred = tmp.GetPreferredValues();

            const float padX = 32f;
            const float padY = 22f;

            var rt = go.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;  // страховка от чужого масштаба родителя
            rt.sizeDelta = new Vector2(preferred.x + padX, preferred.y + padY);
        }

        StartCoroutine(FadeAndDestroy(go));
    }

    private IEnumerator FadeAndDestroy(GameObject go)
    {
        yield return new WaitForSeconds(displayTime);

        var cg = go.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                cg.alpha = 1f - (t / fadeTime);
                yield return null;
            }
        }
        Destroy(go);
    }
}