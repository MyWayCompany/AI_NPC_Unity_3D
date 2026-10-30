using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Панель инвентаря")]
    public GameObject inventoryPanel;  // перетащи InventoryPanel
    public bool startOpen = false;

    [Header("Слоты")]
    public Transform slotsParent;      // перетащи SlotsParent
    public GameObject slotPrefab;      // перетащи SlotPrefab из Project
    public int slotCount = 20;

    private readonly List<InventorySlotUI> slots = new List<InventorySlotUI>();
    private readonly Dictionary<string, int> items = new Dictionary<string, int>();
    private bool isOpen;

    public event System.Action<string, int> OnItemAdded;
    public event System.Action<string, int> OnItemRemoved;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (inventoryPanel != null) inventoryPanel.SetActive(startOpen);
        isOpen = startOpen;

        BuildSlots();
        RefreshAllSlots();
    }

    private void Start()
    {
        if (AIService.Instance != null)
            AIService.Instance.OnItemGiven += HandleItemGiven;
    }

    private void OnDestroy()
    {
        if (AIService.Instance != null)
            AIService.Instance.OnItemGiven -= HandleItemGiven;
    }

    /// <summary>Собирает слоты из сцены, а если их нет — создаёт из префаба.</summary>
    private void BuildSlots()
    {
        if (slotsParent == null)
        {
            Debug.LogError("[Inventory] slotsParent не назначен в Inspector!");
            return;
        }

        slots.Clear();
        slots.AddRange(slotsParent.GetComponentsInChildren<InventorySlotUI>(true));

        if (slots.Count == 0 && slotPrefab != null)
        {
            for (int i = 0; i < slotCount; i++)
            {
                GameObject go = Instantiate(slotPrefab, slotsParent);
                go.name = $"Slot_{i}";
                slots.Add(go.GetComponent<InventorySlotUI>());
            }
            Debug.Log($"[Inventory] Создано слотов: {slotCount}");
        }

        if (slots.Count == 0)
            Debug.LogError("[Inventory] Слоты не найдены и не созданы! Проверь slotPrefab и компонент InventorySlotUI на нём.");
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Не открываем инвентарь, пока игрок печатает в поле ввода диалога
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            return;

        if (kb[Key.I].wasPressedThisFrame)
            Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        if (inventoryPanel != null) inventoryPanel.SetActive(isOpen);
    }

    private void HandleItemGiven(string itemId) => AddItem(itemId, 1);

    public bool AddItem(string itemId, int count = 1)
    {
        if (items.ContainsKey(itemId))
        {
            items[itemId] += count;
        }
        else
        {
            if (items.Count >= slots.Count)
            {
                Debug.LogWarning("[Inventory] Нет свободных слотов!");
                return false;
            }
            items[itemId] = count;
        }

        RefreshAllSlots();
        OnItemAdded?.Invoke(itemId, count);
        return true;
    }

    public bool HasItem(string itemId) => GetCount(itemId) > 0;
    public int GetCount(string itemId) => items.TryGetValue(itemId, out var c) ? c : 0;

    private void RefreshAllSlots()
    {
        int i = 0;
        foreach (var kv in items)
        {
            if (i >= slots.Count) break;
            var info = ItemDatabase.Get(kv.Key);
            slots[i].Set(kv.Value, info.iconColor);
            i++;
        }
        for (; i < slots.Count; i++)
            slots[i].SetEmpty();
    }
    public bool RemoveItem(string itemId, int count = 1)
    {
        if (!items.ContainsKey(itemId) || items[itemId] < count) return false;
        items[itemId] -= count;
        if (items[itemId] <= 0) items.Remove(itemId);
        RefreshAllSlots();
        OnItemRemoved?.Invoke(itemId, count);
        return true;
    }

}