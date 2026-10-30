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

    /// <summary>
    /// Передаём реплику игрока в AI-сервис (который идёт на Flask-сервер).
    /// Колбэк получит текстовый ответ Горана. Предметы обрабатываются отдельно
    /// через событие AIService.OnItemGiven (подписывается InventoryManager).
    /// </summary>
    public void AskAI(string playerMessage, System.Action<string> onResponse)
    {
        AIService.Instance.AskGoran(playerMessage, r => onResponse?.Invoke(r.answer));
    }

    /// <summary>Полный ответ от AI (текст + действие + предмет + цена).</summary>
    public void AskAIFull(string playerMessage, System.Action<AIResponse> onResponse)
    {
        if (AIService.Instance == null)
        {
            Debug.LogError("[NPCDialogue] AIService.Instance is NULL!");
            onResponse?.Invoke(new AIResponse { answer = "...Связь потеряна...", action = "", item = "", price = 0 });
            return;
        }

        AIService.Instance.AskGoran(playerMessage, onResponse);
    }
}