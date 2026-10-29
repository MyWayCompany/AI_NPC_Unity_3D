using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int coins = 0;
    public List<ItemSlot> inventory = new List<ItemSlot>();
    public int maxSlots = 20;

    [Header("UI")]
    public GameObject inventoryPanel;
    public TMPro.TextMeshProUGUI coinText;
    public Transform slotsParent;
    public GameObject slotPrefab;

    private bool isOpen = false;

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

    private void Start()
    {
        UpdateCoinUI();
        CreateInventorySlots();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard[Key.I].wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void CreateInventorySlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            inventory.Add(new ItemSlot());
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Монеты: {coins}";
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateCoinUI();
        ShowNotification($"Получено {amount} ??", Color.yellow);
    }

    public void SpendCoin(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinUI();          
        }        
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        for(int i = 0; i < inventory.Count; i++)
        {
            if(inventory[i].item == item && inventory[i].quantity < item.maxStack)
            {
                int canAdd = Mathf.Min(quantity, item.maxStack - inventory[i].quantity);
                inventory[i].quantity += canAdd;
                quantity -= canAdd;

                if (quantity < 0)
                {
                    ShowNotification($"Получено: {item.itemName} x{canAdd}", Color.white);
                    return true;
                }
            }
        }

        for (int i = 0; i < inventory.Count;i++)
        {
            if (inventory[i].item == null)
            {
                inventory[i].item = item;
                inventory[i].quantity = Mathf.Min(quantity, item.maxStack);
                quantity -= inventory[i].quantity;

                ShowNotification($"Получено: {item.itemName} x{inventory[i].quantity}", Color.white);

                if (quantity <= 0)
                {
                    return AddItem(item, quantity);
                }

                return true;
            }
        }

        ShowNotification("Инвентарь полон!", Color.red);
        return false;
    }

    private void ShowNotification(string message, Color color)
    {
        NotificationManager.Instance.Show(message, color);
    }
}



[System.Serializable]
public class ItemSlot
{
    public Item item;
    public int quantity = 0;    
}