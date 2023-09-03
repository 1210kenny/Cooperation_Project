using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Threading;

public class text_to_voice : MonoBehaviour
{
    // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
    static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
    static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

    public static Microsoft.CognitiveServices.Speech.SpeechConfig config_;
    public static SpeechSynthesizer synthesizer;
    public string speak_style = "assistant";
    public bool isSpeaking = false;
    [SerializeField]
    private AnimationControl animationControl;
    public text_to_voice(string key, string region)
    {
        config_ = SpeechConfig.FromSubscription(key, region);
        config_.SpeechSynthesisVoiceName = "zh-CN-XiaoxiaoNeural";
        synthesizer = new SpeechSynthesizer(config_);
    }

    public void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Console.WriteLine($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }

    public void Mute()
    {
        synthesizer.StopSpeakingAsync();
        isSpeaking = false;
        File.WriteAllText(@"Assets\Python\speechRecognition\mode.txt", "1");
    }



    // 非同步方法用於執行語音合成
    async public void readString(string text, string style, Action onCompleted)
    {
        var ssml = $"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='zh-CN'>" +
            $"<voice name='zh-CN-XiaoxiaoNeural' style='{style}'>" +
            $"{text}" +
            "</voice></speak>";

        using (var result = await synthesizer.SpeakSsmlAsync(ssml))
        {
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine($"Speech synthesized for text [{text}]");

                // 調用 onCompleted 回調（如果有提供的話）以表示合成完成
                onCompleted?.Invoke();
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
        }
    }
    public void ChangeEmotion(string emotion)
    {
        switch (emotion)
        {
            case "1":
                speak_style = "affectionate";
                break;
            case "2":
                speak_style = "angry";
                break;
            case "3":
                speak_style = "assistant";
                break;
            case "4":
                speak_style = "calm";
                break;
            case "5":
                speak_style = "chat";
                break;
            case "6":
                speak_style = "cheerful";
                break;
            case "7":
                speak_style = "customerService";
                break;
            case "8":
                speak_style = "disgruntled";
                break;
            case "9":
                speak_style = "fearful";
                break;
            case "10":
                speak_style = "friendly";
                break;
            case "11":
                speak_style = "gentle";
                break;
            case "12":
                speak_style = "lyrical";
                break;
            case "13":
                speak_style = "newscast";
                break;
            case "14":
                speak_style = "poetryReading";
                break;
            case "15":
                speak_style = "sad";
                break;
            case "16":
                speak_style = "serious";
                break;
            default:
                speak_style = "assistant";
                break;
        }
    }
}