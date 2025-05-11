using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using GameLogic.GptChats;

public class GeminiAPI : MonoBehaviour, IGptChat
{
    private const string APIKey = "AIzaSyC9bZ9bsaLbMs8Gi55sbfzbqt0RLb2kZr8";
    private const string URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=";

    // private async void Start()
    // {
        // var response = await SendGeminiRequest("Тебе нужно похвалить игрока за прохождение уровня в головоломке (составление слов из кусочков), сделай это одним предложением, будь краток (5-12 слов)");
        // Debug.Log(response);
    //}

    public async UniTask<string> Ask(string promt)
    {
        var requestData = new GeminiRequest
        {
            contents = new[]
            {
                new Content
                {
                    parts = new[]
                    {
                        new Part { text = promt }
                    }
                }
            }
        };

        var jsonBody = JsonUtility.ToJson(requestData);
        using (UnityWebRequest request = new UnityWebRequest(URL + APIKey, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                var response = JsonUtility.FromJson<GeminiResponse>(request.downloadHandler.text);
                if (response?.candidates != null && response.candidates.Length > 0)
                {
                    Candidate candidate = response.candidates[0];
                    if (candidate.finishReason != "STOP")
                        return null;

                    if (candidate.content?.parts?.Length > 0)
                    {
                        var text = candidate.content.parts[0].text;
                        if (string.IsNullOrEmpty(text))
                            return null;

                        return text;
                    }
                }
            }
        }
        return null;

    }

    [System.Serializable]
    public class GeminiRequest
    {
        public Content[] contents;
    }

    [System.Serializable]
    public class Content
    {
        public Part[] parts;
    }

    [System.Serializable]
    public class Part
    {
        public string text;
    }
    
    [System.Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
        public UsageMetadata usageMetadata;
        public string modelVersion;
    }

    [System.Serializable]
    public class Candidate
    {
        public Content content;
        public string finishReason;
        public float avgLogprobs;
    }

    [System.Serializable]
    public class UsageMetadata
    {
        public int promptTokenCount;
        public int candidatesTokenCount;
        public int totalTokenCount;
        public TokenDetails[] promptTokensDetails;
        public TokenDetails[] candidatesTokensDetails;
    }

    [System.Serializable]
    public class TokenDetails
    {
        public string modality;
        public int tokenCount;
    }

}