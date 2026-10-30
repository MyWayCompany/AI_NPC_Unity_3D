using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System;

public class ConversationController : MonoBehaviour
{
    public static ConversationController Instance { get; private set; }

    [Header("UI диалога")]
    public GameObject inputPanel;
    public TMP_InputField inputField;

    [Header("Пузыри")]
    public SpeechBubble playerBubble;

    private NPCDialogue currentNPC;
    private bool inConversation;
    private bool busy;

    public bool IsBusy => inConversation || busy;

    private void Awake()
    {
        Instance = this;
        if (inputPanel != null) inputPanel.SetActive(false);
    }

    void Update()
    {
        if (!inConversation || busy) return;

        var kb = Keyboard.current;
        if (kb != null && kb[Key.Escape].wasPressedThisFrame)
            EndConversation();
    }

    public void StartConversation(NPCDialogue npc)
    {
        if (inConversation) return;
        currentNPC = npc;
        inConversation = true;
        ShowInput();
    }

    private void EndConversation()
    {
        inConversation = false;
        busy = false;
        currentNPC = null;
        if (inputPanel != null) inputPanel.SetActive(false);
        if (playerBubble != null) playerBubble.Hide();
    }

    private void ShowInput()
    {
        if (inputPanel == null || inputField == null) return;

        inputPanel.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
        inputField.Select();
    }

    public void OnSendClicked()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message) || busy) return;

        inputPanel.SetActive(false);
        StartCoroutine(PlayerSpeakRoutine(message));
    }

    public void OnSubmitString(string _) => OnSendClicked();

    /// <summary>
    /// Основная логика общения с Гораном через AI-сервис.
    /// </summary>
    private IEnumerator PlayerSpeakRoutine(string message)
    {
        busy = true;
     
        bool typed = false;
        playerBubble.onTyped = () => typed = true;
        playerBubble.Show(message);
        yield return new WaitUntil(() => typed);
        yield return new WaitForSeconds(0.8f);
        playerBubble.Hide();

        SpeechBubble npcBubble = currentNPC.bubble;
        npcBubble.Show("...", false);

        AIResponse response = null;
        
        currentNPC.AskAIFull(message, r => response = r);
        
        float timer = 0f;
        while (response == null && timer < 30f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
       
        if (response == null)
        {
            Debug.LogError("[Conversation] Таймаут ожидания ответа AI!");
            npcBubble.Hide();
            yield return ShowNpcReactionCo("...Связь пропала. Попробуй позже.");
            FinishTurn();
            yield break;
        }

        npcBubble.Hide();

        Debug.Log($"[DEBUG] Получен ответ от AI: Action='{response.action}', Item='{response.item}', Price={response.price}");

        switch (response.action)
        {
            case "confirm_purchase":
                yield return ShowNpcReactionCo(response.answer);

                bool purchaseDone = false;
                bool popupClosed = false;

                // Вызываем Show, передавая колбэк, который установит флаги
                PurchasePopupUI.Instance.Show(response.item, response.price, success =>
                {
                    purchaseDone = success;
                    popupClosed = true; // Помечаем, что попап обработал действие
                });

                // Ждем, пока popupClosed станет true (игрок нажал кнопку)
                // Таймаут защиты от зависания, если игрок закроет игру
                float waitTimer = 0f;
                while (!popupClosed && waitTimer < 60f)
                {
                    yield return null;
                    waitTimer += Time.deltaTime;
                }

                if (purchaseDone)
                {
                    NotificationManager.Instance.Show($"{ItemDatabase.Get(response.item).displayName} +1");
                    yield return ShowNpcReactionCo("Приятного аппетита, парень.");
                }
                else
                {
                    yield return ShowNpcReactionCo("Как хочешь. Если передумаешь — я тут.");
                }
                break;

            case "give_item":               
                InventoryManager.Instance.AddItem(response.item, 1);
                NotificationManager.Instance.Show($"{ItemDatabase.Get(response.item).displayName} +1");
                yield return ShowNpcReactionCo(response.answer);
                break;

            default:                
                yield return ShowNpcReactionCo(response.answer);
                break;
        }

        FinishTurn();
    }

    /// <summary>
    /// Хелпер для завершения хода: снимаем блокировку и возвращаем поле ввода.
    /// </summary>
    private void FinishTurn()
    {
        busy = false;
        if (inConversation) ShowInput();
    }

    /// <summary>
    /// Публичный метод для показа реакции NPC (используется торговлей/попапами).
    /// </summary>
    public void ShowNpcReaction(string text) => StartCoroutine(ShowNpcReactionCo(text));

    private IEnumerator ShowNpcReactionCo(string text)
    {
        if (currentNPC == null || currentNPC.bubble == null) yield break;

        var b = currentNPC.bubble;
        bool typed = false;
        b.onTyped = () => typed = true;
        b.Show(text);
        yield return new WaitUntil(() => typed);
        yield return new WaitForSeconds(1.5f); // Время чтения
        b.Hide();
    }
}