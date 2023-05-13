using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json.Linq;
using static ChatGPT;
using System.IO;
using TreeEditor;

public class inputChat : MonoBehaviour
{
    //對話窗口
    public GameObject chatWindow;
    //對話條
    public Text chatItem;

    //輸入送出按鍵
    //public Button yourButton;
    //用戶 輸入框
    public InputField chatInput;
    //API Key 輸入框
    //public InputField OpenAI_Key;
    //chatGPT對話 物件
    public ChatGPT chatGPT;

    //線程鎖
    private object threadLocker = new object();
    //語音辨識等待值
    private bool waitingForReco;
    //語音辨識狀態 0:未開始辨識 1：輸入辨識中 2：辨識終止
    private int Rec = 0;

    //SpeechRecognizer 物件
    private SpeechRecognizer recognizer;
    //輸入語音訊息
    private string message;

    //AI語音播放器
    private text_to_voice Speaker = new text_to_voice();
    //語音辨識工具
    private Microsoft.CognitiveServices.Speech.SpeechConfig configuration = SpeechConfig.FromSubscription("", "");

    void Start()
    {
        configuration.SpeechRecognitionLanguage = "zh-TW";
        //Button btn = yourButton.GetComponent<Button>();
        //btn.onClick.AddListener(TaskOnClick);

        // 初始化 SpeechRecognizer 物件
        //InitializeSpeechRecognizer();

        //預設角色
        chatGPT.m_DataList.Add(new SendData("system", "我是強尼，是你的生活幫手，可以幫你回答任何問題。"));


        //讀取現有對話紀錄
        try
        {
            var inputString = File.ReadAllText("MyFile.json");
            chatGPT.m_DataList = JsonUtility.FromJson<Serialization<SendData>>(inputString).ToList();
        }
        //無對話紀錄 則創建空紀錄檔案
        catch (Exception e)
        {
            File.Create("MyFile.json");
        }
    }

    void OnDestroy()
    {
        // 釋放 SpeechRecognizer 物件資源
        if (recognizer != null)
        {
            recognizer.Dispose();
            recognizer = null;
        }
    }

    private async void InitializeSpeechRecognizer()
    {

        lock (threadLocker)
        {
            //等待語音辨識(啟用)
            waitingForReco = true;
            //語音辨識開始
            Rec = 1;
        }

        // 初始化 SpeechRecognizer 物件
        recognizer = new SpeechRecognizer(configuration);

        //語音辨識結果紀錄
        string newMessage = string.Empty;

        //語音辨識回傳
        var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

        //確認語音辨識
        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            newMessage = result.Text;
        }

        lock (threadLocker)
        {
            //傳輸輸入訊息
            message = newMessage;
            //等待語音辨識(停用)
            waitingForReco = false;
            //語音辨識完成
            Rec = 2;
        }

        // 設定回傳的結果類型為 RecognizedSpeech
        /*
        recognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                recognizedText = e.Result.Text;
                //chatInput.text = recognizedText;
                //recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }
        };
        */

        // 開始語音辨識
        // await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

        //保证至少一个任务完成（等待到结束时间执行后再结束）
        //Task.WaitAny(new[] { stopRecognition.Task });
    }

    //當發送鍵被按下
    void TaskOnClick()
    {
        //設置ApiKey 已內建
        //chatGPT.setApiKey(OpenAI_Key.text);
        print(chatGPT.getApiKey());

        //判斷輸入框是否為空
        //if (!string.IsNullOrEmpty(chatInput.text))
        //    toSendData();
    }

    //GPT訊息 發送動作
    public void toSendData(string _msg)
    {

        //取得輸入訊息
        //string _msg = chatInput.text;
        print(_msg);
        //清空輸入框
        //chatInput.text = "";
        //建構對話條
        var vChatWindow = chatWindow.transform.localPosition;
        var itemGround = Instantiate(chatItem, vChatWindow, Quaternion.identity);
        itemGround.transform.parent = chatWindow.transform;
        itemGround.text = " I :　" + _msg;
        //
        StartCoroutine(TurnToLastLine());
        //POST GPT訊息
        StartCoroutine(chatGPT.GetPostData(_msg, CallBack));
    }

    //GPT訊息 回傳動作
    private void CallBack(string _callback)
    {
        //取得回傳訊息
        _callback = _callback.Trim();
        print(_callback);
        //建構對話條
        var vChatWindow = chatWindow.transform.localPosition;
        var itemGround = Instantiate(chatItem, vChatWindow, Quaternion.identity);
        itemGround.transform.parent = chatWindow.transform;
        itemGround.text = " chatGPT :　" + _callback;
        //AI語音播放
        Speaker.speak(_callback);
        //
        StartCoroutine(TurnToLastLine());

        //讀取現有對話紀錄
        var outputString = JsonUtility.ToJson(new Serialization<SendData>(chatGPT.m_DataList));
        try
        {
            File.WriteAllText("MyFile.json", outputString);
        }
        //無對話紀錄 則創建空紀錄檔案
        catch (Exception e)
        {
            File.Create("MyFile.json");
            File.WriteAllText("MyFile.json", outputString);
        }
    }

    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //控制語音辨識線程
        lock (threadLocker)
        {
            //語音辨識完成
            if (Rec == 2)
            {
                Rec = 0;
                if (!string.IsNullOrEmpty(message))
                {
                    //停止現有的AI對話與音輸出
                    Speaker.Mute();
                    //輸入框顯示本次輸入的訊息
                    chatInput.text = message;
                    //chatGPT請求
                    toSendData(message);
                    //淨空傳遞訊息
                    message = string.Empty;
                }
            }
            //啟用語音辨識
            else if (Rec == 0)
            {
                InitializeSpeechRecognizer();
            }
        }
    }
}

//list轉存JSON
[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}