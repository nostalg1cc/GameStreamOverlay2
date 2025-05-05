using System;
using System.Windows;
using System.Windows.Threading;

namespace VideoOverlay
{
    public partial class App : Application
    {
        private TrayIconManager _trayIconManager;
        private KeyboardHook _keyboardHook;
        private OverlayManager _overlayManager;
        private Settings _settings;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Handle unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            // Load settings
            _settings = Settings.Load();

            // Initialize the overlay manager
            _overlayManager = new OverlayManager(_settings);

            // Initialize the keyboard hook
            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyPressed += KeyboardHook_KeyPressed;
            _keyboardHook.RegisterHotKey(_settings.ToggleOverlayModifiers, _settings.ToggleOverlayKey);

            // Initialize the tray icon
            _trayIconManager = new TrayIconManager();
            _trayIconManager.ExitRequested += TrayIconManager_ExitRequested;
            _trayIconManager.SettingsRequested += TrayIconManager_SettingsRequested;
            _trayIconManager.ToggleOverlayRequested += TrayIconManager_ToggleOverlayRequested;
            _trayIconManager.Initialize();
        }

        private void KeyboardHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            _overlayManager.ToggleOverlay();
        }

        private void TrayIconManager_ExitRequested(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void TrayIconManager_SettingsRequested(object sender, EventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.ShowDialog();

            // Update settings if changed
            if (settingsWindow.SettingsChanged)
            {
                _keyboardHook.UnregisterHotKey();
                _keyboardHook.RegisterHotKey(_settings.ToggleOverlayModifiers, _settings.ToggleOverlayKey);
                _settings.Save();
            }
        }

        private void TrayIconManager_ToggleOverlayRequested(object sender, EventArgs e)
        {
            _overlayManager.ToggleOverlay();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIconManager?.Dispose();
            _keyboardHook?.Dispose();
            _overlayManager?.Dispose();
            _settings?.Save();
            
            base.OnExit(e);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"A fatal error occurred: {(e.ExceptionObject as Exception)?.Message}", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
