using PlcComUI.Properties;
using System.Windows.Data;

namespace PlcComUI.UserSettings
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = Properties.Settings.Default.SettingsMain;
            this.Mode = BindingMode.TwoWay;

        }
    }
}
