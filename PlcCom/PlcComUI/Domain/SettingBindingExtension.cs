using PlcComUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PlcComUI.Domain
{
    // https://thomaslevesque.com/2008/11/18/wpf-binding-to-application-settings-using-a-markup-extension/
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
