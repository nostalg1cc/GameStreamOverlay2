using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace VideoOverlay
{
    public partial class MainWindow : Window
    {
        private WebView2 _webView;
        private Settings _settings;
        private bool _closeRequested = false;

        public event EventHandler<WebView2> WebViewInitialized;
        public bool IsOverlayActive { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            _settings = Settings.Load();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize WebView2
            _webView = new WebView2();
            ContentGrid.Children.Add(_webView);

            try
            {
                var userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "VideoOverlay");
                
                Directory.CreateDirectory(userDataFolder);
                
                var env = await CoreWebView2Environment.CreateAsync(
                    null, userDataFolder, new CoreWebView2EnvironmentOptions());
                
                await _webView.EnsureCoreWebView2Async(env);

                // Configure WebView2
                _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                
                // Populate URL textbox with default or saved URL
                UrlTextBox.Text = _settings.LastUrl ?? "https://www.youtube.com/";
                
                // Navigate to the default URL
                if (!string.IsNullOrEmpty(UrlTextBox.Text))
                {
                    _webView.CoreWebView2.Navigate(UrlTextBox.Text);
                }
                
                WebViewInitialized?.Invoke(this, _webView);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUrl();
        }

        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadUrl();
            }
        }

        private void LoadUrl()
        {
            if (_webView?.CoreWebView2 != null && !string.IsNullOrWhiteSpace(UrlTextBox.Text))
            {
                string url = UrlTextBox.Text;
                
                // Add https:// if not present and not a local file
                if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("file://"))
                {
                    url = "https://" + url;
                    UrlTextBox.Text = url;
                }
                
                _webView.CoreWebView2.Navigate(url);
                _settings.LastUrl = url;
                _settings.Save();
            }
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            var overlayManager = (Application.Current as App)?._overlayManager;
            overlayManager?.PinCurrentVideo();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.ShowDialog();
            
            if (settingsWindow.SettingsChanged)
            {
                var app = Application.Current as App;
                app._keyboardHook.UnregisterHotKey();
                app._keyboardHook.RegisterHotKey(_settings.ToggleOverlayModifiers, _settings.ToggleOverlayKey);
                _settings.Save();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_closeRequested)
            {
                e.Cancel = true;
                Hide();
            }
        }

        public void RequestClose()
        {
            _closeRequested = true;
            Close();
        }

        public void ShowOverlay()
        {
            IsOverlayActive = true;
            Show();
            Activate();
        }

        public void HideOverlay()
        {
            IsOverlayActive = false;
            Hide();
        }
    }
}
