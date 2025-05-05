using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace VideoOverlay
{
    public class KeyPressedEventArgs : EventArgs
    {
        public ModifierKeys Modifier { get; private set; }
        public Key Key { get; private set; }

        public KeyPressedEventArgs(ModifierKeys modifier, Key key)
        {
            Modifier = modifier;
            Key = key;
        }
    }

    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;
        private ModifierKeys _registeredModifiers;
        private Key _registeredKey;

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            _proc = HookCallback;
            _hookId = SetHook(_proc);
        }

        public void RegisterHotKey(ModifierKeys modifier, Key key)
        {
            _registeredModifiers = modifier;
            _registeredKey = key;
        }

        public void UnregisterHotKey()
        {
            _registeredModifiers = ModifierKeys.None;
            _registeredKey = Key.None;
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                
                // Check if the pressed key matches the registered hotkey
                bool modifiersMatch = CheckModifiers();
                
                if (modifiersMatch && (Key)vkCode == _registeredKey)
                {
                    KeyPressed?.Invoke(this, new KeyPressedEventArgs(_registeredModifiers, _registeredKey));
                }
            }
            
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private bool CheckModifiers()
        {
            bool ctrl = (_registeredModifiers & ModifierKeys.Control) != 0;
            bool alt = (_registeredModifiers & ModifierKeys.Alt) != 0;
            bool shift = (_registeredModifiers & ModifierKeys.Shift) != 0;
            bool win = (_registeredModifiers & ModifierKeys.Windows) != 0;

            bool ctrlPressed = (GetAsyncKeyState((int)Key.LeftCtrl) & 0x8000) != 0 || 
                              (GetAsyncKeyState((int)Key.RightCtrl) & 0x8000) != 0;
                              
            bool altPressed = (GetAsyncKeyState((int)Key.LeftAlt) & 0x8000) != 0 || 
                             (GetAsyncKeyState((int)Key.RightAlt) & 0x8000) != 0;
                             
            bool shiftPressed = (GetAsyncKeyState((int)Key.LeftShift) & 0x8000) != 0 || 
                               (GetAsyncKeyState((int)Key.RightShift) & 0x8000) != 0;
                               
            bool winPressed = (GetAsyncKeyState((int)Key.LWin) & 0x8000) != 0 || 
                             (GetAsyncKeyState((int)Key.RWin) & 0x8000) != 0;

            return (ctrl == ctrlPressed) && (alt == altPressed) && (shift == shiftPressed) && (win == winPressed);
        }

        public void Dispose()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }
    }
}
