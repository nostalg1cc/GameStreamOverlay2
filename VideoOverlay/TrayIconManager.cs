using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace VideoOverlay
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;

        public event EventHandler ExitRequested;
        public event EventHandler SettingsRequested;
        public event EventHandler ToggleOverlayRequested;

        public void Initialize()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Toggle Overlay", null, (s, e) => ToggleOverlayRequested?.Invoke(this, EventArgs.Empty));
            _contextMenu.Items.Add("Settings", null, (s, e) => SettingsRequested?.Invoke(this, EventArgs.Empty));
            _contextMenu.Items.Add("-"); // Separator
            _contextMenu.Items.Add("Exit", null, (s, e) => ExitRequested?.Invoke(this, EventArgs.Empty));

            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Default icon as fallback
                Visible = true,
                Text = "Video Overlay",
                ContextMenuStrip = _contextMenu
            };

            _notifyIcon.DoubleClick += (s, e) => ToggleOverlayRequested?.Invoke(this, EventArgs.Empty);

            // Try to load custom icon if available
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app-icon.ico");
                if (File.Exists(iconPath))
                {
                    _notifyIcon.Icon = new Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load tray icon: {ex.Message}");
                // Use default icon
            }
        }

        public void Dispose()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }
}
