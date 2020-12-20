using System;

// https://stackoverflow.com/questions/8148473/convention-in-caliburn-micro-for-radio-buttons
// https://www.broculos.net/2014/03/wpf-how-to-save-settings-using-custom.html?m=1

namespace AppSettings
{
    [Serializable]
    public class SettingsMain
    {
        public MainWindowSettings MainWindow { get; set; }

        public SettingsMain()
        {
            MainWindow = new MainWindowSettings();
            MainWindow.Width = 1000;
            MainWindow.Height = 800;
        }
    }
}
