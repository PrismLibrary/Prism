using System.Collections.Generic;
using System.Diagnostics;
using Android.Speech.Tts;
using Java.Lang;
using Sandbox.Droid.Services;
using Sandbox.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(TextToSpeechService_Android))]

namespace Sandbox.Droid.Services
{
    public class TextToSpeechService_Android : Object, ITextToSpeechService, TextToSpeech.IOnInitListener
    {
        TextToSpeech _speaker;
        string _toSpeak;

        public void Speak(string text)
        {
            var c = Forms.Context;
            _toSpeak = text;
            if (_speaker == null)
            {
                _speaker = new TextToSpeech(c, this);
            }
            else
            {
                var p = new Dictionary<string, string>();
                _speaker.Speak(_toSpeak, QueueMode.Flush, p);
                Debug.WriteLine("spoke " + _toSpeak);
            }
        }

        #region IOnInitListener implementation

        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                Debug.WriteLine("speaker init");
                var p = new Dictionary<string, string>();
                _speaker.Speak(_toSpeak, QueueMode.Flush, p);
            }
            else
            {
                Debug.WriteLine("was quiet");
            }
        }

        #endregion
    }
}