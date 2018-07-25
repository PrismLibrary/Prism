using System;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Prism.Services
{
    public class KeyDownEventArgs : EventArgs
    {
        public KeyDownEventArgs(VirtualKey virtualKey)
        {
            var alt = (Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            var shift = (Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            var control = (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            var windows = ((Window.Current.CoreWindow.GetKeyState(VirtualKey.LeftWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                || ((Window.Current.CoreWindow.GetKeyState(VirtualKey.RightWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down);

            AltKey = alt;
            ControlKey = control;
            ShiftKey = shift;
            WindowsKey = windows;
            VirtualKey = virtualKey;
            Character = ToChar(virtualKey, shift);
        }

        public bool Handled { get; set; } = false;
        public bool AltKey { get; set; }
        public bool ControlKey { get; set; }
        public bool ShiftKey { get; set; }
        public VirtualKey VirtualKey { get; set; }
        public AcceleratorKeyEventArgs EventArgs { get; set; }
        public char? Character { get; set; }
        public bool WindowsKey { get; internal set; }

        public bool OnlyWindows => WindowsKey & !AltKey & !ControlKey & !ShiftKey;
        public bool OnlyAlt => !WindowsKey & AltKey & !ControlKey & !ShiftKey;
        public bool OnlyControl => !WindowsKey & !AltKey & ControlKey & !ShiftKey;
        public bool OnlyShift => !WindowsKey & !AltKey & !ControlKey & ShiftKey;
        public bool Combo => new[] { AltKey, ControlKey, ShiftKey }.Any(x => x) & Character.HasValue;

        public override string ToString()
        {
            return $"KeyboardEventArgs = Handled {Handled}, AltKey {AltKey}, ControlKey {ControlKey}, ShiftKey {ShiftKey}, VirtualKey {VirtualKey}, Character {Character}, WindowsKey {WindowsKey}, OnlyWindows {OnlyWindows}, OnlyAlt {OnlyAlt}, OnlyControl {OnlyControl}, OnlyShift {OnlyShift}";
        }

        private static char? ToChar(VirtualKey key, bool shift)
        {
            // convert virtual key to char
            if (32 == (int)key)
            {
                return ' ';
            }

            VirtualKey search;

            // look for simple letter
            foreach (var letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            {
                if (Enum.TryParse(letter.ToString(), out search) && search.Equals(key))
                {
                    return (shift) ? letter : letter.ToString().ToLower()[0];
                }
            }

            // look for simple number
            foreach (var number in "1234567890")
            {
                if (Enum.TryParse("Number" + number.ToString(), out search) && search.Equals(key))
                {
                    return number;
                }
            }

            // not found
            return null;
        }
    }
}
