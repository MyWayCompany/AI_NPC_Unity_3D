using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    public Transform notificationParent;
    public GameObject notificationPrefab;
    public float displayTime = 3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show(string message, Color color)
    {
        GameObject notification = Instantiate(notificationPrefab, notificationParent);
        TMPro.TextMeshProUGUI text = notification.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
        {
            text.text = message;
            text.color = color;
        }
        Destroy(notification, displayTime);
    }
}