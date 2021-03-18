using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class ConfigFilesViewModel : Screen
    {
        private string _directoryPath;

        public ConfigFilesViewModel()
        {
            DirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public string DirectoryPath
        {
            get => _directoryPath;
            set
            {
                if (Equals(_directoryPath, value))
                    return;

                _directoryPath = value;
                Properties.Settings.Default.SettingsMain.Config.ConfigFilesPath = _directoryPath;
                NotifyOfPropertyChange(() => DirectoryPath);
            }
        }

        public void SetConfigDirectoryPath()
        {
            // https://stackoverflow.com/questions/1922204/open-directory-dialog
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    DirectoryPath = dialog.FileName;
                }
            }
        }
    }
}
