using Settings.AppSettings;
using Settings.UserSettings;
using System;

// https://stackoverflow.com/questions/8148473/convention-in-caliburn-micro-for-radio-buttons
// https://www.broculos.net/2014/03/wpf-how-to-save-settings-using-custom.html?m=1

namespace Settings
{
    [Serializable]
    public class SettingsMain
    {
        public MainWindowSettings MainWindow { get; set; }
        public SplashWindowSettings SplashWindow { get; set; }
        public ConfigSettings Config { get; set; }
        public SettingsMain()
        {
            MainWindow = new MainWindowSettings();
            MainWindow.Width = 1000;
            MainWindow.Height = 800;

            SplashWindow = new SplashWindowSettings();
            SplashWindow.Height = 450;
            SplashWindow.Width = 1000;

            Config = new ConfigSettings();
            Config.ConfigFilesPath = AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
