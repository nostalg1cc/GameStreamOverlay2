using System;
using System.Windows;
using System.Windows.Input;

namespace VideoOverlay
{
    public partial class SettingsWindow : Window
    {
        private Settings _settings;
        private ModifierKeys _tempModifiers;
        private Key _tempKey;
        private KeyboardHook _hotKeyHook;
        private bool _isCapturingHotkey = false;

        public bool SettingsChanged { get; private set; } = false;

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
            
            // Initialize UI with current settings
            HotkeyTextBox.Text = _settings.GetHotkeyString();
            AlwaysOnTopCheckBox.IsChecked = _settings.AlwaysOnTop;
            StartMinimizedCheckBox.IsChecked = _settings.StartMinimized;
        }

        private void ChangeHotkey_Click(object sender, RoutedEventArgs e)
        {
            // Show hotkey input dialog
            HotkeyInputDialog.Visibility = Visibility.Visible;
            NewHotkeyTextBox.Text = "Press keys...";
            
            // Initialize temporary values
            _tempModifiers = ModifierKeys.None;
            _tempKey = Key.None;
            
            // Start listening for keyboard input
            _isCapturingHotkey = true;
            
            // Set up keyboard hook for capturing hotkey
            if (_hotKeyHook == null)
            {
                _hotKeyHook = new KeyboardHook();
                PreviewKeyDown += SettingsWindow_PreviewKeyDown;
            }
        }

        private void SettingsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!_isCapturingHotkey)
                return;

            e.Handled = true;

            // Capture modifiers
            _tempModifiers = Keyboard.Modifiers;

            // Ignore modifier keys when pressed alone
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LWin || e.Key == Key.RWin)
            {
                return;
            }

            // Capture main key
            _tempKey = e.Key;

            // Update the text box
            UpdateHotkeyText();

            // Stop capturing
            _isCapturingHotkey = false;
        }

        private void UpdateHotkeyText()
        {
            string modifiers = string.Empty;

            if ((_tempModifiers & ModifierKeys.Control) != 0)
                modifiers += "Ctrl + ";
            if ((_tempModifiers & ModifierKeys.Alt) != 0)
                modifiers += "Alt + ";
            if ((_tempModifiers & ModifierKeys.Shift) != 0)
                modifiers += "Shift + ";
            if ((_tempModifiers & ModifierKeys.Windows) != 0)
                modifiers += "Win + ";

            NewHotkeyTextBox.Text = modifiers + _tempKey.ToString();
        }

        private void HotkeyOK_Click(object sender, RoutedEventArgs e)
        {
            if (_tempKey != Key.None)
            {
                // Update the main textbox
                HotkeyTextBox.Text = NewHotkeyTextBox.Text;
                
                // Hide the dialog
                HotkeyInputDialog.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Please select a valid key combination.", "Invalid Hotkey", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void HotkeyCancel_Click(object sender, RoutedEventArgs e)
        {
            // Hide the dialog without saving
            HotkeyInputDialog.Visibility = Visibility.Collapsed;
            _isCapturingHotkey = false;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Save settings
            if (_tempKey != Key.None)
            {
                _settings.ToggleOverlayModifiers = _tempModifiers;
                _settings.ToggleOverlayKey = _tempKey;
                SettingsChanged = true;
            }
            
            _settings.AlwaysOnTop = AlwaysOnTopCheckBox.IsChecked ?? false;
            _settings.StartMinimized = StartMinimizedCheckBox.IsChecked ?? false;
            
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _hotKeyHook?.Dispose();
            base.OnClosed(e);
        }
    }
}
