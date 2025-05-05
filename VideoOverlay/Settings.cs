using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;

namespace VideoOverlay
{
    public class Settings
    {
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VideoOverlay",
            "settings.json");

        public ModifierKeys ToggleOverlayModifiers { get; set; } = ModifierKeys.Control | ModifierKeys.Alt;
        public Key ToggleOverlayKey { get; set; } = Key.V;
        public bool AlwaysOnTop { get; set; } = true;
        public string LastUrl { get; set; } = "https://www.youtube.com/";
        public bool StartMinimized { get; set; } = true;

        public static Settings Load()
        {
            try
            {
                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));

                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }

            return new Settings();
        }

        public void Save()
        {
            try
            {
                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));

                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        public string GetHotkeyString()
        {
            string modifiers = string.Empty;

            if ((ToggleOverlayModifiers & ModifierKeys.Control) != 0)
                modifiers += "Ctrl + ";
            if ((ToggleOverlayModifiers & ModifierKeys.Alt) != 0)
                modifiers += "Alt + ";
            if ((ToggleOverlayModifiers & ModifierKeys.Shift) != 0)
                modifiers += "Shift + ";
            if ((ToggleOverlayModifiers & ModifierKeys.Windows) != 0)
                modifiers += "Win + ";

            return modifiers + ToggleOverlayKey.ToString();
        }
    }
}
