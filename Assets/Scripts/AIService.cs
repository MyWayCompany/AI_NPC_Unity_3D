using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string content;

    public ChatMessage(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}

[System.Serializable]
public class ChatRequest
{
    public string model;
    public List<ChatMessage> messages;
    public float temperature;
    public int max_tokens;    
}

[System.Serializable]
public class Choice
{
    public ChatMessage message;
}

[System.Serializable]
public class ChatResponse
{
    public Choice[] choices;
}


public class AIService : MonoBehaviour
{
    public static AIService Instance;

    [Header("Íàñòðîéêè API")]
    public string apiUrl = "https://api.openai.com/v1/chat/completions";
    public string apiKey = "ÂÑÒÀÂÜ_ÊËÞ×";
    public string model = "gpt-4o-mini";
    public float temperature = 0.8f;
    public int maxTokens = 120;

    private void Awake()
    {
        Instance = this;
    }
}
