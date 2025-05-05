using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VideoOverlay
{
    public partial class VideoOverlayControl : UserControl
    {
        private WebView2 _webView;
        private Window _parentWindow;
        private string _currentUrl;
        private bool _isInteractable = false;

        public event EventHandler CloseRequested;

        public VideoOverlayControl(string url, double width, double height)
        {
            InitializeComponent();
            
            _currentUrl = url;
            Width = width;
            Height = height;
            
            // Initialize the WebView programmatically
            Loaded += VideoOverlayControl_Loaded;
            
            // Setup mouse behavior
            MouseDown += VideoOverlayControl_MouseDown;
            MouseMove += VideoOverlayControl_MouseMove;
        }

        private Point _dragStartPoint;
        private bool _isDragging = false;

        private void VideoOverlayControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isInteractable && e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                _dragStartPoint = e.GetPosition(_parentWindow);
                CaptureMouse();
            }
        }

        private void VideoOverlayControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _parentWindow != null)
            {
                Point currentPosition = e.GetPosition(_parentWindow);
                Vector diff = currentPosition - _dragStartPoint;

                double newLeft = Canvas.GetLeft(this) + diff.X;
                double newTop = Canvas.GetTop(this) + diff.Y;

                // Ensure the overlay stays within screen bounds
                newLeft = Math.Max(0, Math.Min(_parentWindow.ActualWidth - ActualWidth, newLeft));
                newTop = Math.Max(0, Math.Min(_parentWindow.ActualHeight - ActualHeight, newTop));

                Canvas.SetLeft(this, newLeft);
                Canvas.SetTop(this, newTop);

                _dragStartPoint = currentPosition;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && _isDragging)
            {
                _isDragging = false;
                ReleaseMouseCapture();
            }
            base.OnMouseUp(e);
        }

        private async void VideoOverlayControl_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
            
            try
            {
                _webView = new WebView2();
                WebViewContainer.Child = _webView;

                var userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "VideoOverlay", "PinnedView");
                
                Directory.CreateDirectory(userDataFolder);
                
                var env = await CoreWebView2Environment.CreateAsync(
                    null, userDataFolder, new CoreWebView2EnvironmentOptions());
                
                await _webView.EnsureCoreWebView2Async(env);

                // Configure WebView2
                _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                
                // Navigate to the URL
                if (!string.IsNullOrEmpty(_currentUrl))
                {
                    _webView.CoreWebView2.Navigate(_currentUrl);
                }

                // Set default volume
                SetVolume(1.0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2 for overlay: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;

            if (_webView != null)
            {
                // When not interactable, make WebView ignore mouse events
                _webView.IsHitTestVisible = interactable;
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isInteractable)
            {
                ControlBar.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ControlBar.Visibility = Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void GrowButton_Click(object sender, RoutedEventArgs e)
        {
            Width = Width * 1.2;
            Height = Height * 1.2;
        }

        private void ShrinkButton_Click(object sender, RoutedEventArgs e)
        {
            Width = Width * 0.8;
            Height = Height * 0.8;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(e.NewValue);
        }

        private void SetVolume(double volume)
        {
            if (_webView?.CoreWebView2 != null)
            {
                // Use JavaScript to set the volume on any video elements in the page
                _webView.ExecuteScriptAsync($@"
                    Array.from(document.querySelectorAll('video, audio')).forEach(elem => {{
                        elem.volume = {volume};
                    }});
                ");
            }
        }

        public void Dispose()
        {
            _webView?.Dispose();
        }
    }
}
