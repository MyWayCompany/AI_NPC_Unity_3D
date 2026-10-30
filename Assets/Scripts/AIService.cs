using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class AIService : MonoBehaviour
{
    public static AIService Instance { get; private set; }

    [Header("Настройки сервера")]
    public string baseUrl = "http://127.0.0.1:8000";
    public float timeoutSeconds = 90f;

    public event Action<string> OnItemGiven;
    public event Action<string, int> OnPurchaseOffered;

    public event Action OnRequestStarted;
    public event Action OnRequestFinished;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[AIService] Инициализирован. URL: " + baseUrl);
        }
    }

    public void AskGoran(string playerMessage, Action<AIResponse> onResponse)
    {
        Debug.Log($"[AIService] Вызов AskGoran. Сообщение: '{playerMessage}'");
        StartCoroutine(DialogueRoutine(playerMessage, onResponse));
    }

    private IEnumerator DialogueRoutine(string message, Action<AIResponse> onResponse)
    {
        OnRequestStarted?.Invoke();

      
        var requestBody = new AIRequest { text = message };       
        string jsonBody = JsonUtility.ToJson(requestBody);
        Debug.Log($"[AIService] Отправка POST на {baseUrl}/ask. Body: {jsonBody}");       

        using (var request = new UnityWebRequest(baseUrl + "/ask", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = (int)timeoutSeconds;
            
            yield return request.SendWebRequest();
           
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[AIService] Ошибка сети/HTTP: {request.error}\nКод: {(long)request.responseCode}");
                
                onResponse?.Invoke(new AIResponse
                {
                    answer = "...Сервер недоступен...",
                    action = "",
                    item = "",
                    price = 0
                });
                OnRequestFinished?.Invoke();
                yield break;
            }
          
            string responseText = request.downloadHandler.text;
            Debug.Log($"[AIService] Получен сырой ответ: {responseText}");

            try
            {
                var response = JsonUtility.FromJson<AIResponse>(responseText);
                
                Debug.Log($"[AIService] Распарсено: Answer='{response.answer}', Action='{response.action}', Item='{response.item}', Price={response.price}");
                
                switch (response.action)
                {
                    case "give_item":
                        OnItemGiven?.Invoke(response.item);
                        break;
                    case "confirm_purchase":
                        OnPurchaseOffered?.Invoke(response.item, response.price);
                        break;
                }

                onResponse?.Invoke(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AIService] Ошибка парсинга JSON: {e.Message}\nТекст был: {responseText}");
                onResponse?.Invoke(new AIResponse { answer = "...Ошибка данных...", action = "", item = "", price = 0 });
            }
        }

        OnRequestFinished?.Invoke();
    }

    public void ConfirmPurchase(string item, int price)
    {
        Debug.Log($"[AIService] Подтверждение покупки: {item} за {price}");
        StartCoroutine(PostJson("/confirm_purchase", new { item, price }, _ => { }));
    }


    private IEnumerator PostJson(string path, object bodyData, Action<TradeResponse> onComplete)
    {
        using (var req = new UnityWebRequest(baseUrl + path, "POST"))
        {
            string json = JsonUtility.ToJson(bodyData);
            byte[] raw = Encoding.UTF8.GetBytes(json);

            req.uploadHandler = new UploadHandlerRaw(raw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = (int)timeoutSeconds;

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var resp = JsonUtility.FromJson<TradeResponse>(req.downloadHandler.text);
                onComplete(resp);
            }
            else
            {
                Debug.LogError($"POST Error ({path}): {req.error}");
                onComplete(null);
            }
        }
    }
}

[System.Serializable]
public class AIResponse
{
    public string answer;
    public string action;
    public string item;
    public int price;
}

[System.Serializable]
public class TradeResponse
{
    public bool accepted;
    public string reaction;
    public string error;
}

[Serializable]
public class  AIRequest 
{
    public string text;
}