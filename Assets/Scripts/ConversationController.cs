using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System;

public class ConversationController : MonoBehaviour
{
    public static ConversationController Instance { get; private set; }

    [Header("UI ввода")]
    public GameObject inputPanel;
    public TMP_InputField inputField;

    [Header("Пузыри")]
    public SpeechBubble playerBubble;

    private NPCDialogue currentNPC;
    private bool inConversation;

    public bool IsBusy => inConversation;

    private void Awake()
    {
        Instance = this;
        inputPanel.SetActive(false);
    }

    void Update()
    {
        if (!inConversation) return;

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
        currentNPC = null;
        inputPanel.SetActive(false);
        playerBubble.Hide();
    }

    private void ShowInput()
    {
        inputPanel.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
        inputField.Select();
    }

    public void OnSendClicked()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;
        inputPanel.SetActive(false);
        StartCoroutine(PlayerSpeak(message));
    }

    public void OnSubmitString(string text)
    {
        OnSendClicked();
    }

    private IEnumerator PlayerSpeak(string message)
    {
        bool typed = false;
        playerBubble.onTyped = () => typed = true;
        playerBubble.Show(message);

        yield return new WaitUntil(() => typed);
        yield return new WaitForSeconds(1.2f);
        playerBubble.Hide();

        SpeechBubble npcBubble = currentNPC.bubble;
        npcBubble.Show("...", false);

        string response = null; 
        currentNPC.AskAI(message, (answer) => response = answer);

        npcBubble.Hide();
        npcBubble.Show(response);

        yield return new WaitUntil(() => !npcBubble.gameObject.activeSelf);

        if (inConversation)        
            ShowInput();        
    }

}
