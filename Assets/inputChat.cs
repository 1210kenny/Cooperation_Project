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
using System.Linq;

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

    //上次回傳訊息
    private string last_callback = "fjweiofwoanow;iefnoiwefnowfnowe";
    //AI暫停播放關鍵字
    private const string callAI = "暫停播放";
    //AI語音播放器
    private text_to_voice Speaker;
    //設備關鍵字
    private const string callEquipment = "操作設備";
    //設備模式
    private bool equipmentMode = false;
    //觸發指令操作 (預設為進入設備模式 第一句話) 待指令集加入後改由偵測到指令集指令後觸發
    private bool firstEquipment = true;
    //語音辨識工具
    private Microsoft.CognitiveServices.Speech.SpeechConfig configuration;
    //建立TextAsset
    public TextAsset TxtFile;
    //用來存放文本內容
    private string Mytxt;       
    
    [SerializeField] public List<ApiKeyData> ApiKey = new List<ApiKeyData>();

    void Start()
    {
        //讀取鑰匙
        try
        {
            var inputString = File.ReadAllText("Keys.json");
            ApiKey = JsonUtility.FromJson<Serialization<ApiKeyData>>(inputString).ToList();
        }
        //無鑰匙 回報
        catch (Exception e)
        {
            print("No have Key file.");
        }

        //AI語音播放器
        Speaker = new text_to_voice(ApiKey[0].key, ApiKey[0].region);
        configuration = SpeechConfig.FromSubscription(ApiKey[1].key, ApiKey[1].region);
        chatGPT.setApiKey(ApiKey[2].key);

        configuration.SpeechRecognitionLanguage = "zh-TW";
        //Button btn = yourButton.GetComponent<Button>();
        //btn.onClick.AddListener(TaskOnClick);

        // 初始化 SpeechRecognizer 物件
        //InitializeSpeechRecognizer();

        //讀取文本
        Mytxt = ((TextAsset)Resources.Load("instruction")).text;
        //測試
        //print(Mytxt);

        //chatGPT(聊天) 預設角色
        chatGPT.m_DataList.Add(new SendData("system", "我是生活幫手，可以回答任何問題；同時也是一個可以控制設備AI，在接收命令時，只表示願意執行即可，等待後續輸入再根據(裝置狀態)做回應，若(裝置狀態)是失敗的，請根據狀態描述提示用戶可能的錯誤原因。"));
        //chatGPT(設備) 預設角色
        chatGPT.e_DataList.Add(new SendData("system", "我是指令資料庫，請根據輸入文句選擇下列相對應的編號指令：\n1.開關電燈1號\n2.開關電燈二號\n3.播放、暫停音樂\n4.提供目前播放中的音樂叫什麼名字\n5.提供上一首播放的音樂叫什麼名字"));

        //讀取現有對話紀錄
        try
        {
            var inputString = File.ReadAllText("MyFile.json");
            if (!String.IsNullOrEmpty(inputString))
                chatGPT.m_DataList = JsonUtility.FromJson<Serialization<SendData>>(inputString).ToList();
        }
        //無對話紀錄 則創建空紀錄檔案
        catch (Exception e)
        {
            File.Create("MyFile.json");
        }

        //讀取現有設備操作紀錄
        try
        {
            var inputString = File.ReadAllText("EquipmentLog.json");
            if(!String.IsNullOrEmpty(inputString))
                chatGPT.e_DataList = JsonUtility.FromJson<Serialization<SendData>>(inputString).ToList();
        }
        //無設備操作 則創建空紀錄檔案
        catch (Exception e)
        {
            File.Create("EquipmentLog.json");
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
            //newMessage = "操作設備，請幫我開啟電燈。";
            //呼叫字串比較，不是由AI回答，並且提到"操作設備"則進入設備模式
            if (!equipmentMode)
            {
                //任務進入設備模式 (之後會由分析用戶任務導向的方式控制 目前仍由偵測關鍵詞進入)
                var check_EquipmentMode_call = ClassSim.MatchKeywordSim(callEquipment, newMessage);
                if (check_EquipmentMode_call >= 0.4)
                {
                    equipmentMode = true;
                    firstEquipment = true;
                }
            }
        }
        //強制結束播放（不用等回傳到）ChatGPT的時間
        var check_num1 = ClassSim.MatchKeywordSim(callAI, newMessage);
        if(check_num1 >= 0.5){
            Rec = 0;
            //停止現有的AI對話與音輸出
            Speaker.Mute();
            return;
        }
        //呼叫字串比較，只要大於一定值就直接無視
        var check_num = ClassSim.MatchKeywordSim(last_callback, newMessage);
        if(check_num >= 0.5){
            Rec = 0;
            return;
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
        if (!equipmentMode || (equipmentMode && firstEquipment)) {
            var vChatWindow = chatWindow.transform.localPosition;
            var itemGround = Instantiate(chatItem, vChatWindow, Quaternion.identity);
            itemGround.transform.parent = chatWindow.transform;
            itemGround.text = " I :　" + _msg;
        }
        else if (equipmentMode && !firstEquipment)
        {
            equipmentMode = false;
        }
        //POST GPT訊息 (並添加訊息備註 提示chatGPT回答規範)
        if (equipmentMode && firstEquipment)
        {
            _msg += "(簡短回覆收到命令即可，勿確切告知執行與否)";
            firstEquipment = false;
        }
        //
        StartCoroutine(TurnToLastLine());
        //POST GPT訊息
        StartCoroutine(chatGPT.GetPostData(_msg, CallBack));
    }

    //GPT訊息 發送動作 (設備模式)
    public void toSendData_E(
        string _msg    //文字消息
    ){
        print(_msg);
        //
        //不與對話行交互 (無須印出對話)
        //
        StartCoroutine(TurnToLastLine());
        //POST GPT訊息 (並添加訊息備註 提示chatGPT回答規範)
        _msg += "(請根據指令資料庫選擇指令)";
        StartCoroutine(chatGPT.GetPostData_E(_msg, CallBack_E));
    }

    //GPT訊息 回傳動作
    private void CallBack(string _callback, string emotion)
    {
        //取得回傳訊息
        _callback = _callback.Trim();
        print("M: " + _callback);
        print(emotion);
        last_callback = _callback;

        if(equipmentMode)
            toSendData_E(message);
        //淨空傳遞訊息
        message = string.Empty;

        //建構對話條
        var vChatWindow = chatWindow.transform.localPosition;
        var itemGround = Instantiate(chatItem, vChatWindow, Quaternion.identity);
        itemGround.transform.parent = chatWindow.transform;
        itemGround.text = " chatGPT :　" + _callback;
        //AI語音播放
        Speaker.speak(_callback);
        //
        StartCoroutine(TurnToLastLine());

        
        //存取現有對話紀錄
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

    //GPT訊息 回傳動作 (設備操作使用)
    private void CallBack_E(string _callback)
    {
        //取得回傳訊息
        _callback = _callback.Trim();
        print("E: "+_callback);
        StartCoroutine(TurnToLastLine());
        //觸發指令操作
        //
        //實際裝置控制指令
        //
        //根據裝置狀態讓chatGPT(聊天)回應
        String equipmentState = "(裝置狀態)裝置已操作成功，裝置目前狀態:啟用";
        //String equipmentState = "(裝置狀態)裝置無法連接";
        //String equipmentState = "(裝置狀態)查無此裝置";
        //String equipmentState = "(裝置狀態)操作失敗，原因:未知";
        toSendData(equipmentState);

        //讀取現有對話紀錄
        var outputString = JsonUtility.ToJson(new Serialization<SendData>(chatGPT.e_DataList));
        try
        {
            File.WriteAllText("EquipmentLog.json", outputString);
        }
        //無對話紀錄 則創建空紀錄檔案
        catch (Exception e)
        {
            File.Create("EquipmentLog.json");
            File.WriteAllText("EquipmentLog.json", outputString);
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
                
                //一般聊天
                if (!string.IsNullOrEmpty(message))
                {
                    //停止現有的AI對話與音輸出
                    Speaker.Mute();
                    //輸入框顯示本次輸入的訊息
                    chatInput.text = message;
                    //chatGPT請求
                    toSendData(message);
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

// API金鑰 資料結構
[Serializable]
public class ApiKeyData
{
    public string key; // 使用模型
    public string region; // 對話紀錄
}

//字串比較演算法
public class ClassSim
{
    public static double MatchKeywordSim(string keyword, string matchkeyword)
    {
        List<char> keywordList = keyword.ToCharArray().ToList();
        List<char> matchkeywordList = matchkeyword.ToCharArray().ToList();
        List<char> unionKeyword = keywordList.Union(matchkeywordList).ToList<char>();
        List<int> arrA = new List<int>();
        List<int> arrB = new List<int>();
        foreach (var str in unionKeyword)
        {
            arrA.Add(keywordList.Where(x => x == str).Count());
            arrB.Add(matchkeywordList.Where(x => x == str).Count());
        }
        double num = 0;
        double numA=0;
        double numB=0;
        for (int i = 0; i < unionKeyword.Count; i++)
        { 
            num+=arrA[i]*arrB[i];
            numA+=Math.Pow(arrA[i], 2);
            numB+=Math.Pow(arrB[i], 2);
        }
        double cos = num / (Math.Sqrt(numA) * Math.Sqrt(numB));
        return cos;
    }
}
