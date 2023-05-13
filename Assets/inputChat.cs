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


public class inputChat : MonoBehaviour
{
    //對話窗口
    public GameObject chatWindow;
    //對話條
    public Text chatItem;

    //輸入送出按鍵
    public Button yourButton;
    //用戶 輸入框
    public InputField chatInput;
    //API Key 輸入框
    public InputField OpenAI_Key;
    //chatGPT對話 物件
    public ChatGPT chatGPT;
    private SpeechRecognizer recognizer;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

        // 初始化 SpeechRecognizer 物件
        InitializeSpeechRecognizer();
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
        var configuration = SpeechConfig.FromSubscription("", "");
        configuration.SpeechRecognitionLanguage = "zh-TW";

        // 初始化 SpeechRecognizer 物件
        recognizer = new SpeechRecognizer(configuration);

        // 設定回傳的結果類型為 RecognizedSpeech
        recognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                string recognizedText = e.Result.Text;
                chatInput.text = recognizedText;
            }
        };

        // 開始語音辨識
        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
    }

    //當發送鍵被按下
    void TaskOnClick()
    {
        //設置ApiKey 已內建
        //chatGPT.setApiKey(OpenAI_Key.text);
        print(chatGPT.getApiKey());

        //判斷輸入框是否為空
        if (!string.IsNullOrEmpty(chatInput.text))
            toSendData();
    }

    //GPT訊息 發送動作
    public void toSendData()
    {

        //取得輸入訊息
        string _msg = chatInput.text;
        print(_msg);
        //清空輸入框
        chatInput.text = "";
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
        //
        text_to_voice.readString(_callback);
        //
        StartCoroutine(TurnToLastLine());
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
    }

}