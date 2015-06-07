using AVFoundation;
using Sandbox.iOS.Services;
using Sandbox.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(TextToSpeechService_iOS))]

namespace Sandbox.iOS.Services
{
    public class TextToSpeechService_iOS : ITextToSpeechService
    {
        public void Speak(string text)
        {
            var speechSynthesizer = new AVSpeechSynthesizer (); 

 			var speechUtterance = new AVSpeechUtterance (text) { 
 				Rate = AVSpeechUtterance.MaximumSpeechRate/4, 
 				Voice = AVSpeechSynthesisVoice.FromLanguage ("en-US"), 
 				Volume = 0.5f, 
 				PitchMultiplier = 1.0f 
 			}; 

 			speechSynthesizer.SpeakUtterance (speechUtterance); 
        }
    }
}