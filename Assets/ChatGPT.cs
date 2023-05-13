using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPT : MonoBehaviour
{
    // 對話列表(紀錄、設定)
    [SerializeField] public List<SendData> m_DataList = new List<SendData>();
    // 定义Chat API的URL
    private string m_ApiUrl = "https://api.openai.com/v1/chat/completions";
    // API key
    private string m_OpenAI_Key = "";

    //設置API KEY
    public string getApiKey()
    {
        return m_OpenAI_Key;
    }

    //取出API KEY
    public void setApiKey(string m_OpenAI_Key)
    {
        this.m_OpenAI_Key = m_OpenAI_Key;
    }

    // Post 資料結構
    [Serializable]
    public class PostData
    {
        public string model; // 使用模型
        public List<SendData> messages; // 對話紀錄
    }

    // Send 資料結構 
    [Serializable]
    public class SendData
    {
        public string role; // user、system、assistant 不同角色有不同用處
        public string content; // 內文
        public SendData() { }
        public SendData(string _role, string _content)
        {
            role = _role;
            content = _content;
        }
    }

    // GPT返回訊息 資料結構 
    [Serializable]
    private class MessageBack
    {
        public string id;
        public string created;
        public string model;
        public List<MessageBody> choices;
    }


    // GPT返回訊息 本體 資料結構 
    [Serializable]
    private class MessageBody
    {
        public Message message;
        public string finish_reason;
        public string index;
    }

    // GPT返回訊息 內文 資料結構 
    [Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    // GPT Post訊息
    public IEnumerator GetPostData(
        string _postWord,   //輸入訊息
        System.Action<string> _callback //異步回傳函式
    )
    {

        print(_postWord);
        //緩存發送的訊息
        m_DataList.Add(new SendData("user", _postWord));

        //建構 WebRequest POST
        using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
        {
            //導入PostData
            PostData _postData = new PostData
            {
                model = "gpt-3.5-turbo",
                messages = m_DataList
            };

            //轉存格式
            string _jsonText = JsonUtility.ToJson(_postData);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
            //導入request頭部資訊
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", m_OpenAI_Key));

            //發送
            yield return request.SendWebRequest();

            //後台回報 回傳碼 (200為成功)
            print(request.responseCode);
            if (request.responseCode == 200)
            {
                //取出回傳訊息
                string _msg = request.downloadHandler.text;
                MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msg);
                if (_textback != null && _textback.choices.Count > 0)
                {

                    string _backMsg = _textback.choices[0].message.content;
                    //緩存回傳訊息
                    m_DataList.Add(new SendData("assistant", _backMsg));
                    //返回函式
                    _callback(_backMsg);
                }

            }
            //單次Post訊息結束
            print("PostEnd");
        }
    }
}
