using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour, IInteractable
{
    [Header("Ссылки")]
    public PlayerController playerController;

    [Header("UI Элементы")]
    public GameObject dialoguePanel;
    public TMP_InputField inputField;
    public Button sendButton;
    public TextMeshProUGUI goranResponseText;
    public TextMeshProUGUI thinkingText;
    public GameObject interactionPrompt;

    [Header("Настройки")]
    public string serverUrl = "http://127.0.0.1:8000/ask";
    public float interactionDistance = 3f;

    private Transform goranTransform;
    private bool isNearGoran = false;
    private bool isDialogueOpen = false;

    [Header("Инвентарь")]
    public TextMeshProUGUI inventoryList; 

    private List<string> inventory = new List<string>();


    void Start()
    {
        dialoguePanel.SetActive(false);
        thinkingText.gameObject.SetActive(false);
        interactionPrompt.SetActive(false);

        GameObject goran = GameObject.Find("Goran");
        if (goran != null)
        {
            goranTransform = goran.transform;
        }

        sendButton.onClick.AddListener(SendMessage);
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (goranTransform != null && !isDialogueOpen)
        {
            float distance = Vector2.Distance(transform.position, goranTransform.position);

            if (distance <= interactionDistance)
            {
                isNearGoran = true;
                interactionPrompt.SetActive(true);

                if (keyboard[Key.E].wasPressedThisFrame) {
                    OpenDialogue();
                }
            }
            else
            {
                isNearGoran = false;
                interactionPrompt.SetActive(false);
            }
        }

        if (isDialogueOpen && keyboard[Key.Escape].wasPressedThisFrame)
        {
            CloseDialogue();
        }
    }

    void OpenDialogue()
    {
        isDialogueOpen = true;
        dialoguePanel.SetActive(true);
        interactionPrompt.SetActive(false);
        inputField.text = "";
        inputField.Select();

        if (playerController != null) playerController.enabled = false;
    }

    void CloseDialogue()
    {
        isDialogueOpen = false;
        dialoguePanel.SetActive(false);

        if (playerController != null) playerController.enabled = true;
    }

    void SendMessage()
    {
        string playerMessage = inputField.text.Trim();
        if (string.IsNullOrEmpty(playerMessage)) return;

        inputField.text = "";
        inputField.interactable = false;
        sendButton.interactable = false;

        thinkingText.gameObject.SetActive(true);
        goranResponseText.text = "";

        StartCoroutine(SendRequestToServer(playerMessage));
    }

    IEnumerator SendRequestToServer(string message)
    {
        string jsonPayload = $"{{\"text\": \"{EscapeJson(message)}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;               

                DialogueResponse response = JsonUtility.FromJson<DialogueResponse>(responseJson);

                goranResponseText.text = "Горан: " + response.answer;

                if (!string.IsNullOrEmpty(response.action) && response.action == "give_item")
                {
                    string itemName = response.item;                    
                    
                    if (!inventory.Contains(itemName))
                    {
                        inventory.Add(itemName);
                        UpdateInventoryUI();
                    }
                }                
            }
            else
            {
                goranResponseText.text = "Ошибка связи с сервером: " + request.error;
            }
        }

        thinkingText.gameObject.SetActive(false);
        inputField.interactable = true;
        sendButton.interactable = true;
        inputField.Select();
    }

    void UpdateInventoryUI()
    {
        if (inventoryList == null)
        {          
            return;
        }        

        if (inventory.Count == 0)
        {
            inventoryList.text = "Пусто";
        }
        else
        {
            string inventoryText = "";
            foreach (string item in inventory)
            {
                string displayName = item;
                switch (item)
                {
                    case "ale": displayName = "Эль"; break;
                    case "bread": displayName = "Хлеб"; break;
                    case "stew": displayName = "Похлёбка"; break;
                    case "cellar_key": displayName = "Ключ от подвала"; break;
                    case "dragon_scroll": displayName = "Свиток о драконе"; break;
                    case "room": displayName = "Комната на ночь"; break;
                }
                inventoryText += displayName + "\n";
            }
            inventoryList.text = inventoryText;            
        }
    }

    string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
    }

    public string GetPromptText()
    {
        return "Поговорить с Гораном [E]";
    }

    public void Interact(Vector3 interactorPosition)
    {
        OpenDialogue();
    }

    [System.Serializable]
    public class DialogueResponse
    {
        public string answer;
        public string action;
        public string item;
    }

    
}