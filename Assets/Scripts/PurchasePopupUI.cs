using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PurchasePopupUI : MonoBehaviour
{
    public static PurchasePopupUI Instance { get; private set; }

    [Header("Ссылки на UI элементы")]
    // ВАЖНО: Убедись, что эти поля заполнены ВНУТРИ префаба!
    public GameObject popupContent;      // Дочерний объект со всем содержимым (фон, текст, кнопки)
    public Image itemIcon;               // Иконка предмета
    public TextMeshProUGUI itemNameText; // Название
    public TextMeshProUGUI itemPriceText;// Цена
    public Button buyButton;             // Кнопка Купить
    public Button cancelButton;          // Кнопка Отмена
    public TextMeshProUGUI warningText;  // Предупреждение "Мало денег"

    private string pendingItemId;
    private int pendingPrice;
    private Action<bool> onCompleteCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Прячем контент при старте
        if (popupContent != null) popupContent.SetActive(false);

        // Слушатели кнопок
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyClicked);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClicked);
    }

    /// <summary>
    /// Показать попап покупки.
    /// </summary>
    public void Show(string itemId, int price, System.Action<bool> onComplete)
    {
        // 1. Сохраняем данные для колбэков
        pendingItemId = itemId;
        pendingPrice = price;
        this.onCompleteCallback = onComplete;

        var info = ItemDatabase.Get(itemId);

        // 2. Диагностика и безопасное заполнение текстов
        if (itemNameText == null) Debug.LogError("[Popup] itemNameText NULL!");
        else itemNameText.text = info.displayName;

        if (itemPriceText == null) Debug.LogError("[Popup] itemPriceText NULL!");
        else itemPriceText.text = $"{price} медяков";

        if (itemIcon == null) Debug.LogError("[Popup] itemIcon NULL!");
        else itemIcon.color = info.iconColor;

        // 3. Проверка денег и состояния кнопок
        bool canAfford = true;
        if (MoneyManager.Instance != null)
            canAfford = MoneyManager.Instance.CanAfford(price);
        else
            Debug.LogWarning("[Popup] MoneyManager is NULL! Assume affordable.");

        // КРИТИЧЕСКАЯ ПРОВЕРКА КНОПОК ПЕРЕД ОБРАЩЕНИЕМ
        if (buyButton == null)
        {
            Debug.LogError("[Popup] buyButton IS NULL! Check Inspector references in Prefab.");
            // Чтобы игра не упала дальше, создаем заглушку или пропускаем настройку
        }
        else
        {
            buyButton.interactable = canAfford;

            // Меняем цвет фона кнопки, если нельзя купить
            var btnImg = buyButton.GetComponent<Image>();
            if (btnImg != null)
                btnImg.color = canAfford ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }

        if (cancelButton == null)
        {
            Debug.LogError("[Popup] cancelButton IS NULL!");
        }

        if (warningText != null)
            warningText.gameObject.SetActive(!canAfford);

        // 4. Активация попапа
        if (popupContent != null)
        {
            gameObject.SetActive(true); // Убеждаемся, что родитель активен
            popupContent.SetActive(true);
            Debug.Log($"[Popup] Показан. Content active: {popupContent.activeSelf}");
        }
        else
        {
            Debug.LogError("[Popup] popupContent IS NULL! Cannot show UI.");
            // Фолбэк: сразу вызываем успех, чтобы диалог не завис
            onComplete?.Invoke(false);
        }
    }

    public void Hide()
    {
        if (popupContent != null)
        {
            popupContent.SetActive(false);
            Debug.Log("[Popup] Скрываю попап.");
        }   
         gameObject.SetActive(false);
    }

    private void OnBuyClicked()
    {
        if (!MoneyManager.Instance.TrySpend(pendingPrice))
        {
            Debug.LogError("[Popup] Покупка отменена: недостаточно монет.");
            Hide();
            onCompleteCallback?.Invoke(false); // Возвращаем false в диалог
            return; // Прерываем выполнение, предмет НЕ добавляется
        }

        InventoryManager.Instance.AddItem(pendingItemId, 1);
        AIService.Instance.ConfirmPurchase(pendingItemId, pendingPrice); // Лог сервера

        OnCancelClicked();
    }

    private void OnCancelClicked()
    {
        Hide();
        onCompleteCallback?.Invoke(false);
    }
}