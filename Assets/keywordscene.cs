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
                // ���Ѩ�����r�A�o�̥i�H�K�[�����������޿�
                SceneManager.LoadScene(1);
            }
        }
        catch (Exception ex)
        {
            // �B�z�ѧO�L�{���i��o�ͪ����󲧱`
            Debug.LogError("����r�ѧO���~�G" + ex.Message);
        }
    }

}