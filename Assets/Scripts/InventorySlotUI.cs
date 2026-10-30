using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Визуал слота")]
    public Image iconImage;
    public TextMeshProUGUI countText;

    private void Awake()
    {        
        if (iconImage == null)
        {
            var t = transform.Find("Icon");
            if (t != null) iconImage = t.GetComponent<Image>();
        }
        if (countText == null)
        {
            var t = transform.Find("Count");
            if (t != null) countText = t.GetComponent<TextMeshProUGUI>();
        }
    }

    public void SetEmpty()
    {
        if (iconImage != null) iconImage.gameObject.SetActive(false);
        if (countText != null) countText.gameObject.SetActive(false);
    }

    public void Set(int count, Color iconColor)
    {
        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.color = iconColor;
        }
        if (countText != null)
        {
            countText.gameObject.SetActive(count > 1);
            countText.text = count.ToString();
        }
    }
}