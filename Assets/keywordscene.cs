using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading;
using System;

public class keywordscene : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private KeywordRecognitionModel keywordModel;

    // Start is called before the first frame update
    void Start()
    {

        string scriptDirectory = System.IO.Path.GetDirectoryName(Application.dataPath);
        string keywordModelPath = "Assets\\AssetsKeywordModels\\0ac84afd-526e-4375-b32d-8c28db473034.table";
        Debug.Log(keywordModelPath);
        keywordModel = KeywordRecognitionModel.FromFile(keywordModelPath);
        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        keywordRecognizer = new KeywordRecognizer(audioConfig);
        StartKeywordRecognition();
    }

    // Update is called once per frame
    private async void StartKeywordRecognition()
    {
        try
        {
            KeywordRecognitionResult result = await keywordRecognizer.RecognizeOnceAsync(keywordModel);

            if (result.Reason == ResultReason.RecognizedKeyword)
            {
                // 辨識到關鍵字，這裡可以添加切換場景的邏輯
                SceneManager.LoadScene(1);
            }
        }
        catch (Exception ex)
        {
            // 處理識別過程中可能發生的任何異常
            Debug.LogError("關鍵字識別錯誤：" + ex.Message);
        }
    }

}