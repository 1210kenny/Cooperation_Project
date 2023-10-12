﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading;
using System;
using System.IO;

public class keywordscene3 : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private KeywordRecognitionModel keywordModel;

    // Start is called before the first frame update
    void Start()
    {

        string scriptDirectory = System.IO.Path.GetDirectoryName(Application.dataPath);
        string keywordModelPath = "Assets\\AssetsKeywordModels\\121889bb-3f27-4782-a3dc-45854ac20224.table";
        keywordModelPath = ConvertWindowsToMacOSPath(keywordModelPath);
        Debug.Log(keywordModelPath);
        keywordModel = KeywordRecognitionModel.FromFile(keywordModelPath);
        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        keywordRecognizer = new KeywordRecognizer(audioConfig);
        StartKeywordRecognition();
    }
    static string ConvertWindowsToMacOSPath(string windowsPath)
    {
        if (Path.DirectorySeparatorChar == '/')
        {
            string fullPath = Path.GetFullPath(windowsPath);
            string macOSPath = fullPath.Replace('\\', Path.DirectorySeparatorChar);
            return macOSPath;
        }
        else return windowsPath;
    }
    // Update is called once per frame
    private async void StartKeywordRecognition()
    {
        try
        {
            KeywordRecognitionResult result = await keywordRecognizer.RecognizeOnceAsync(keywordModel);

            if (result.Reason == ResultReason.RecognizedKeyword)
            {
                PlayerPrefs.SetInt("CharacterSelected", 0);
                PlayerPrefs.Save();
                SceneManager.LoadScene(1);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("����r�ѧO���~�G" + ex.Message);
        }
    }

}