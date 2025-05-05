using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VideoOverlay
{
    public class OverlayManager : IDisposable
    {
        private MainWindow _mainWindow;
        private Window _overlayWindow;
        private Canvas _overlayCanvas;
        private List<VideoOverlayControl> _pinnedVideos = new List<VideoOverlayControl>();
        private Settings _settings;
        private WindowsServices _windowsServices;

        public OverlayManager(Settings settings)
        {
            _settings = settings;
            _windowsServices = new WindowsServices();
            
            // Initialize the main window
            _mainWindow = new MainWindow();
            _mainWindow.WebViewInitialized += MainWindow_WebViewInitialized;
            
            // Initialize the overlay window
            InitializeOverlayWindow();
        }

        private void MainWindow_WebViewInitialized(object sender, WebView2 e)
        {
            // Any additional setup after WebView is initialized
        }

        private void InitializeOverlayWindow()
        {
            _overlayWindow = new Window
            {
                Title = "Video Overlay",
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = null,
                Topmost = true,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                Width = SystemParameters.PrimaryScreenWidth,
                Height = SystemParameters.PrimaryScreenHeight,
                WindowState = WindowState.Maximized
            };
            
            // Set up window to pass through mouse events when inactive
            _windowsServices.MakeWindowClickThrough(_overlayWindow);
            
            // Initialize the canvas for pinned videos
            _overlayCanvas = new Canvas();
            _overlayWindow.Content = _overlayCanvas;
            
            // Hide by default
            _overlayWindow.Hide();
        }

        public void ToggleOverlay()
        {
            if (_mainWindow.IsOverlayActive)
            {
                HideMainOverlay();
            }
            else
            {
                ShowMainOverlay();
            }
        }

        private void ShowMainOverlay()
        {
            _mainWindow.ShowOverlay();
            
            // Make overlay window clickable when main window is shown
            _windowsServices.MakeWindowClickable(_overlayWindow);
            
            // Make pinned videos interactable
            foreach (var video in _pinnedVideos)
            {
                video.SetInteractable(true);
            }
        }

        private void HideMainOverlay()
        {
            _mainWindow.HideOverlay();
            
            // Make overlay window click-through when main window is hidden
            _windowsServices.MakeWindowClickThrough(_overlayWindow);
            
            // Make pinned videos non-interactable
            foreach (var video in _pinnedVideos)
            {
                video.SetInteractable(false);
            }
        }

        public void PinCurrentVideo()
        {
            string url = _settings.LastUrl;
            
            if (string.IsNullOrEmpty(url))
                return;
                
            // Create a new video overlay control
            double width = 400;
            double height = 225;
            var videoOverlay = new VideoOverlayControl(url, width, height);
            
            // Position in the center of the screen by default
            double left = (SystemParameters.PrimaryScreenWidth - width) / 2;
            double top = (SystemParameters.PrimaryScreenHeight - height) / 2;
            
            Canvas.SetLeft(videoOverlay, left);
            Canvas.SetTop(videoOverlay, top);
            
            // Set interactability state
            videoOverlay.SetInteractable(_mainWindow.IsOverlayActive);
            
            // Handle closing request
            videoOverlay.CloseRequested += (s, e) => RemoveVideoOverlay(videoOverlay);
            
            // Add to canvas and tracking list
            _overlayCanvas.Children.Add(videoOverlay);
            _pinnedVideos.Add(videoOverlay);
            
            // Ensure overlay window is visible
            if (!_overlayWindow.IsVisible)
            {
                _overlayWindow.Show();
            }
        }

        private void RemoveVideoOverlay(VideoOverlayControl videoOverlay)
        {
            _overlayCanvas.Children.Remove(videoOverlay);
            _pinnedVideos.Remove(videoOverlay);
            videoOverlay.Dispose();
            
            // Hide overlay window if no pinned videos left
            if (_pinnedVideos.Count == 0)
            {
                _overlayWindow.Hide();
            }
        }

        public void Dispose()
        {
            // Clean up resources
            foreach (var video in _pinnedVideos.ToArray())
            {
                RemoveVideoOverlay(video);
            }
            
            _overlayWindow?.Close();
            _mainWindow?.RequestClose();
        }
    }
}
