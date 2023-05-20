using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class text_to_voice
{
    // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
    static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
    static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

    public static Microsoft.CognitiveServices.Speech.SpeechConfig config_;
    public static SpeechSynthesizer synthesizer;


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
    }

    public void speak(string text)
    {
        readString(text);
    }

    async public static void readString(string text)
    {
        //
        // For more samples please visit https://github.com/Azure-Samples/cognitive-services-speech-sdk 
        // 

        // Creates an instance of a speech config with specified subscription key and service region.

        // Note: the voice setting will not overwrite the voice element in input SSML.
        //while(true){
        // use the default speaker as audio output.
        using (var result = await synthesizer.SpeakTextAsync(text))
        {

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine($"Speech synthesized for text [{text}]");
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
}


