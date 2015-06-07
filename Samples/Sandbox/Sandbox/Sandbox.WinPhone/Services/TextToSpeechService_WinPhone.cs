using System;
using Windows.Phone.Speech.Synthesis;
using Sandbox.Services;
using Sandbox.WinPhone.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(TextToSpeechService_WinPhone))]

namespace Sandbox.WinPhone.Services
{
    public class TextToSpeechService_WinPhone : ITextToSpeechService
    {
        public async void Speak(string text)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync(text);
        }
    }

}
