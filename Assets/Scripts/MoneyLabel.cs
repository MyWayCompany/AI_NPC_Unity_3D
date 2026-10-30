using UnityEngine;
using TMPro;

public class MoneyLabel : MonoBehaviour
{
    [Header("Ссылки")]
    public TextMeshProUGUI label; // Перетащи сюда TMP-текст из CoinPanel

    private void Start()
    {
        // Подписываемся на изменение баланса при старте игры
        if (MoneyManager.Instance != null)
        {
            Refresh(); // Сразу обновляем текст текущим значением
            MoneyManager.Instance.OnMoneyChanged += _ => Refresh();
        }
        else
        {
            Debug.LogError("[MoneyLabel] MoneyManager.Instance is NULL!");
        }
    }

    private void OnDestroy()
    {
        // Отписываемся при уничтожении объекта (чтобы не было утечек памяти)
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged -= _ => Refresh();
        }
    }

    private void Refresh()
    {
        if (label != null && MoneyManager.Instance != null)
        {
            label.text = $"Монеты: {MoneyManager.Instance.current}";
        }
    }
}