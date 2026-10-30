using UnityEngine;
using System.Collections.Generic;

/// <summary>База предметов: id с сервера -> человекочитаемое имя и цвет-заглушка иконки.</summary>
public static class ItemDatabase
{
    public class ItemInfo
    {
        public string id;
        public string displayName;
        public Color iconColor;
    }

    private static readonly Dictionary<string, ItemInfo> items = new Dictionary<string, ItemInfo>
    {
        { "bread",         new ItemInfo { id = "bread",         displayName = "Серый хлеб",       iconColor = new Color(0.76f, 0.70f, 0.50f) } },
        { "ale",           new ItemInfo { id = "ale",           displayName = "Эль",              iconColor = new Color(0.85f, 0.60f, 0.20f) } },
        { "stew",          new ItemInfo { id = "stew",          displayName = "Похлёбка",         iconColor = new Color(0.65f, 0.42f, 0.22f) } },
        { "cellar_key",    new ItemInfo { id = "cellar_key",    displayName = "Ключ от подвала",  iconColor = new Color(0.90f, 0.78f, 0.25f) } },
        { "dragon_scroll", new ItemInfo { id = "dragon_scroll", displayName = "Свиток о драконе", iconColor = new Color(0.92f, 0.88f, 0.72f) } },
        { "room",          new ItemInfo { id = "room",          displayName = "Ключ от комнаты",  iconColor = new Color(0.50f, 0.70f, 0.90f) } },
    };

    public static ItemInfo Get(string id)
    {
        if (items.TryGetValue(id, out var info)) return info;
        return new ItemInfo { id = id, displayName = id, iconColor = Color.gray };
    }
}