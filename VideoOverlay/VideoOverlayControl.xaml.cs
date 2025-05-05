using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        
        #region Enhanced Dragging and Resizing
        
        private Point _dragHandleStartPoint;
        private bool _isDragHandleDragging = false;

        private void DragHandle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isInteractable && e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragHandleDragging = true;
                _dragHandleStartPoint = e.GetPosition(_parentWindow);
                ((UIElement)sender).CaptureMouse();
                e.Handled = true;
            }
        }

        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragHandleDragging && _parentWindow != null)
            {
                Point currentPosition = e.GetPosition(_parentWindow);
                Vector diff = currentPosition - _dragHandleStartPoint;

                // Move the entire control
                double newLeft = Canvas.GetLeft(this) + diff.X;
                double newTop = Canvas.GetTop(this) + diff.Y;

                // Ensure the overlay stays within screen bounds
                newLeft = Math.Max(0, Math.Min(_parentWindow.ActualWidth - ActualWidth, newLeft));
                newTop = Math.Max(0, Math.Min(_parentWindow.ActualHeight - ActualHeight, newTop));

                Canvas.SetLeft(this, newLeft);
                Canvas.SetTop(this, newTop);

                _dragHandleStartPoint = currentPosition;
                e.Handled = true;
            }
        }

        private void DragHandle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragHandleDragging)
            {
                _isDragHandleDragging = false;
                ((UIElement)sender).ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void ResizeThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (_isInteractable && _parentWindow != null)
            {
                // Calculate new width and height
                double newWidth = Math.Max(200, Width + e.HorizontalChange);
                double newHeight = Math.Max(150, Height + e.VerticalChange);
                
                // Check if we're within screen bounds
                double currentLeft = Canvas.GetLeft(this);
                double currentTop = Canvas.GetTop(this);
                
                if (currentLeft + newWidth > _parentWindow.ActualWidth)
                    newWidth = _parentWindow.ActualWidth - currentLeft;
                
                if (currentTop + newHeight > _parentWindow.ActualHeight)
                    newHeight = _parentWindow.ActualHeight - currentTop;

                // Apply new dimensions while maintaining aspect ratio
                if (e.OriginalSource == BottomRightResize)
                {
                    // Free resize (no aspect ratio constraint)
                    Width = newWidth;
                    Height = newHeight;
                }
                
                e.Handled = true;
            }
        }
        
        #endregion

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
                
                // Create options with extensions enabled
                var options = new CoreWebView2EnvironmentOptions();
                options.AdditionalBrowserArguments = "--enable-features=msWebView2EnableDeveloperMode --enable-extensions";
                
                // Create environment with extensions enabled
                var env = await CoreWebView2Environment.CreateAsync(
                    null, userDataFolder, options);
                
                await _webView.EnsureCoreWebView2Async(env);
                
                // Enable Chrome extensions
                await EnableChromeExtensions();

                // Configure WebView2
                _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                
                // Subscribe to navigation events
                _webView.NavigationCompleted += WebView_NavigationCompleted;

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

        private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // Wait a moment for video content to load
                await Task.Delay(1000);
                
                // Apply styles to extract and display only the video content
                await ExtractVideoContent();
            }
        }
        
        public async Task ExtractVideoContent()
        {
            if (_webView?.CoreWebView2 != null)
            {
                // JavaScript to identify and extract the video element
                string videoExtractionScript = @"
                    // Hide everything except the video
                    document.querySelectorAll('body > *').forEach(element => {
                        if (!element.querySelector('video') && !element.contains(document.querySelector('video'))) {
                            element.style.display = 'none';
                        }
                    });
                    
                    // Find the main video element
                    const video = document.querySelector('video');
                    
                    if (video) {
                        // Get the container element that holds the video
                        let container = video;
                        let parent = video.parentElement;
                        
                        // Find the closest parent container that controls the video display
                        for (let i = 0; i < 5 && parent; i++) {
                            container = parent;
                            // Check if we've reached a good container (e.g., YouTube's player)
                            if (parent.id === 'player' || 
                                parent.className.includes('player') || 
                                parent.className.includes('video')) {
                                break;
                            }
                            parent = parent.parentElement;
                        }
                        
                        // Make the container fill the viewport
                        container.style.position = 'fixed';
                        container.style.top = '0';
                        container.style.left = '0';
                        container.style.width = '100%';
                        container.style.height = '100%';
                        container.style.zIndex = '9999';
                        
                        // Ensure the video fills the container
                        video.style.width = '100%';
                        video.style.height = '100%';
                        video.style.objectFit = 'contain';
                        
                        document.body.style.margin = '0';
                        document.body.style.padding = '0';
                        document.body.style.overflow = 'hidden';
                        
                        // Remove any unnecessary UI elements (play bars that might be site-specific)
                        // YouTube-specific adjustments
                        if (window.location.href.includes('youtube.com')) {
                            // Hide YouTube's control bar when not hovering
                            const style = document.createElement('style');
                            style.textContent = `
                                .ytp-chrome-bottom { opacity: 0; transition: opacity 0.3s; }
                                .ytp-chrome-bottom:hover { opacity: 1; }
                            `;
                            document.head.appendChild(style);
                        }
                        
                        return true;
                    }
                    return false;
                ";
                
                // Execute the script to extract the video
                string result = await _webView.ExecuteScriptAsync(videoExtractionScript);
                
                // If video extraction failed, show a message
                if (result == "false")
                {
                    // No video found - perhaps try again or show a message
                    MessageBox.Show("Could not find a video on this page. Please ensure the video is playing before pinning.", 
                        "No Video Found", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        
        private async Task EnableChromeExtensions()
        {
            if (_webView?.CoreWebView2 != null)
            {
                try
                {
                    // Enable browser developer tools for extension management
                    _webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                    
                    // Add a button to the control bar for accessing extensions
                    var extensionsButton = new Button
                    {
                        Content = "Extensions",
                        Margin = new Thickness(5, 0, 5, 0),
                        Padding = new Thickness(5),
                        ToolTip = "Manage Extensions"
                    };
                    
                    extensionsButton.Click += (s, e) => 
                    {
                        _webView.CoreWebView2.Navigate("edge://extensions/");
                    };
                    
                    // Let's use a simpler approach - find the first stack panel by walking through the control hierarchy
                    StackPanel leftPanel = null;
                    
                    // Get the control bar's grid directly
                    if (ControlBar != null && ControlBar is Border controlBorder && 
                        controlBorder.Child is Grid controlBarGrid)
                    {
                        // Get the left stack panel from the grid
                        foreach (var child in controlBarGrid.Children)
                        {
                            if (child is StackPanel panel && panel.Orientation == Orientation.Horizontal)
                            {
                                leftPanel = panel;
                                break;
                            }
                        }
                    }
                    
                    if (leftPanel != null)
                    {
                        leftPanel.Children.Insert(0, extensionsButton);
                    }
                    
                    // Create an "Ad Blocker" button for quick access to popular ad blockers
                    var adBlockerButton = new Button
                    {
                        Content = "Get Ad Blocker",
                        Margin = new Thickness(5, 0, 5, 0),
                        Padding = new Thickness(5),
                        ToolTip = "Install a popular ad blocker extension"
                    };
                    
                    adBlockerButton.Click += (s, e) => 
                    {
                        // This will open the Chrome Web Store page for uBlock Origin
                        _webView.CoreWebView2.Navigate("https://chrome.google.com/webstore/detail/ublock-origin/cjpalhdlnbpafiamejdnhcphjbkeiagm");
                    };
                    
                    // Add to the same panel
                    if (leftPanel != null)
                    {
                        leftPanel.Children.Insert(1, adBlockerButton);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting up extension support: {ex.Message}", 
                        "Extension Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            
            await Task.CompletedTask;
        }
        
        public void Dispose()
        {
            _webView?.Dispose();
        }
        
        // Helper method to find visual children of a specific type
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
            yield break;
        }
    }
}
