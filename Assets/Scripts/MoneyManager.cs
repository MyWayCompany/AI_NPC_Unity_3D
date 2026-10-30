using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public int current = 0;

    public event System.Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool CanAfford(int amount) => current >= amount;

    public void Add(int amount) { current += amount; OnMoneyChanged?.Invoke(current); }

    public bool TrySpend(int amount)
    {
        // 1. Проверяем, хватает ли денег ДО списания
        if (current < amount)
        {
            Debug.LogWarning($"[Money] Недостаточно средств! Есть: {current}, Нужно: {amount}");
            return false; // Отказываем в транзакции
        }

        // 2. Только теперь списываем
        current -= amount;

        // 3. Обновляем UI (если подписаны события)
        OnMoneyChanged?.Invoke(current);

        return true; // Успех
    }
}