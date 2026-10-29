using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    public string npcName = "Горан";
    public SpeechBubble bubble;

    public string GetPromptText()
    {
        return $"Поговорить с {npcName}";
    }

    public void Interact(Vector3 interactionPosition)
    {
        ConversationController.Instance.StartConversation(this);
    }

    public void AskAI(string playerMessage, System.Action<string> onResponse)
    {
        // TODO: сюда позже подключим настоящий AI из DialogueManager
        // Пока — заглушка для теста системы
        string aiResponse = $"Привет! Я {npcName}. Как я могу помочь тебе?";
        onResponse?.Invoke(aiResponse);
    }
}
