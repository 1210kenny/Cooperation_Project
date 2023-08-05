using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPT : MonoBehaviour
{
    // 對話列表(紀錄、設定)
    [SerializeField] public List<SendData> m_DataList = new List<SendData>();
    // 設備操作列表(紀錄、設定)
    [SerializeField] public List<SendData> e_DataList = new List<SendData>();
    // 定义Chat API的URL
    private string m_ApiUrl = "https://api.openai.com/v1/chat/completions";
    // API key
    private string m_OpenAI_Key = "";

    //任務隊列
    public Queue<SendQueue> taskQueue = new Queue<SendQueue>();
    //線程鎖 (ChatGPT)
    public object threadLocker = new object();
    //狀態 0:無執行 1：等待回傳
    public int taskState = 0;

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
        public float temperature; //溫度
    }

    // ChatGPT任務隊列 資料結構
    [Serializable]
    public class SendQueue
    {
        string text;
        int taskClass;

        public SendQueue(string text,int taskClass)
        {
            this.text = text;
            this.taskClass = taskClass;
        }
        public string getText()
        {
            return this.text;
        }
        public int getTaskClass()
        {
            return this.taskClass;
        }
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

    // GPT Post訊息 (任務辨識)
    public IEnumerator GetPostData_T(
        string _postWord,   //輸入訊息
        System.Action<string,string> _callback //異步回傳函式
    )
    {
        print("to T: "+ _postWord);

        List<SendData> t_DataList = new List<SendData>();
        t_DataList.Add(new SendData("system", 
            "我是一個任務分析器，請根據用戶輸入之文句，分析用戶之目的，並選擇下列編號：" +
            "1.聊天或非即時性問題詢問、" +
            "4.傳送郵件給tszmen1104@gmail.com或取得最新郵件內容或新增行事曆和行程、"+
            "2.操作或控制實體設備，郵件和行事曆除外、" +
            "3.詢問有時效性的知識或問題 ;" +
           
            "下列為範例：" +
            "「請問2022年有甚麼大事件發生？」，回答：「3」；" +
            "「請幫我用中文總結最後一個tszmen1104gmail.com發給我的郵件。把總結傳送給tszmen1104gmail.com。」，回答：「4」；" +
            "「請幫我新增行程，8/1晚上去吃晚飯。」，回答：「4」；" +
            "「請幫我打開電燈，並且調整成暖光模式。」，回答：「2」；" +
            "「蘋果派是甚麼？」，回答：「1」；" +
            "如果用戶在句中有明確表達需要使用網路查詢資料則回答：「3」，如 ：「請幫我上網查詢蘋果派的做法。」" +
            "不要回答除編號以外的東西。"));

        //緩存發送的訊息
        t_DataList.Add(new SendData("user", _postWord));

        long responseCode = -1;
        while (responseCode != 200)
        {
            //建構 WebRequest POST
            using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
            {
                //導入PostData
                PostData _postData = new PostData
                {
                    model = "gpt-3.5-turbo",
                    messages = t_DataList,
                    temperature = 1f
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
                responseCode = request.responseCode;
                if (request.responseCode == 200)
                {
                    //取出回傳訊息
                    string _msg = request.downloadHandler.text;
                    MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msg);
                    if (_textback != null && _textback.choices.Count > 0)
                    {

                        string _backMsg = _textback.choices[0].message.content;
                        _callback(_backMsg, _postWord);
                        yield return null;
                    }

                }
            }
            if (responseCode != 200)
            {
                yield return new WaitForSeconds(2f);//停止兩秒
            }
        }
        //單次Post訊息結束
        print("PostEnd");
    }

    // GPT Post訊息 (聊天模式)
    public IEnumerator GetPostData(
        string _postWord,   //輸入訊息
        System.Action<string,string> _callback //異步回傳函式
    )
    {
        print(_postWord);
        //緩存發送的訊息
        m_DataList.Add(new SendData("user", _postWord));

        long responseCode = -1;
        while (responseCode != 200)
        {
            //建構 WebRequest POST
            using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
            {
                //導入PostData
                PostData _postData = new PostData
                {
                    model = "gpt-3.5-turbo",
                    messages = m_DataList,
                    temperature = 1.2f
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
                responseCode = request.responseCode;
                if (request.responseCode == 200)
                {
                    //取出回傳訊息
                    string _msg = request.downloadHandler.text;
                    MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msg);
                    if (_textback != null && _textback.choices.Count > 0)
                    {

                        string _backMsg;
                        try
                        {
                            _backMsg = ChineseProgram.ToTraditionaChinese(_textback.choices[0].message.content);
                        }
                        catch
                        {
                            _backMsg = _textback.choices[0].message.content;
                        }
                        //緩存回傳訊息
                        m_DataList.Add(new SendData("assistant", _backMsg));

                        //返回函式 並做情緒分析
                        yield return inputAnalyze.chatGPT_mood(_backMsg, _callback);

                        //返回函式
                        //_callback(_backMsg, emotion);
                    }

                }
            }
            if (responseCode != 200)
            {
                yield return new WaitForSeconds(2f);//停止兩秒
            }
        }
        //單次Post訊息結束
        print("PostEnd");
    }

    // GPT Post訊息 (設備模式)
    public IEnumerator GetPostData_E(
        string _postWord,   //輸入訊息
        System.Action<string> _callback //異步回傳函式
    )
    {
        print(_postWord);
        //緩存發送的訊息
        e_DataList.Add(new SendData("user", _postWord));


        long responseCode = -1;
        while (responseCode != 200)
        {
            //建構 WebRequest POST
            using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
            {
                //導入PostData
                PostData _postData = new PostData
                {
                    model = "gpt-3.5-turbo",
                    messages = e_DataList
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
                responseCode = request.responseCode;
                if (request.responseCode == 200)
                {
                    //取出回傳訊息
                    string _msg = request.downloadHandler.text;
                    MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msg);
                    if (_textback != null && _textback.choices.Count > 0)
                    {
                        string _backMsg;
                        try
                        {
                            _backMsg = ChineseProgram.ToTraditionaChinese(_textback.choices[0].message.content);
                        }
                        catch
                        {
                            _backMsg = _textback.choices[0].message.content;
                        }
                        //緩存回傳訊息
                        e_DataList.Add(new SendData("assistant", _backMsg));

                        //返回函式 並做情緒分析
                        //yield return inputAnalyze.GetPostData(_backMsg, _callback);

                        _callback(_backMsg);
                    }
                }
            }
            if (responseCode != 200)
            {
                yield return new WaitForSeconds(2f);//停止兩秒
            }
        }
        //單次Post訊息結束
        print("PostEnd");
    }
}
