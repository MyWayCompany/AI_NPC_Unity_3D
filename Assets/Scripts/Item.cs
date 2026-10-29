using UnityEngine;

public enum ItemType
{
    Coin,
    Consumable,
    Weapon,
    Armor,
    Quest
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public ItemType itemType;
    public Sprite icon;
    public int value;
    public int maxStack = 1;
}
