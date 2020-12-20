using Caliburn.Micro;

namespace PlcComUI.ViewModels
{
    public class ErrorDialogViewModel : Screen
    {
        private string _headerText;
        private string _contentText;
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                NotifyOfPropertyChange(() => HeaderText);
            }
        }

        public string ContentText
        {
            get => _contentText;
            set
            {
                _contentText = value;
                NotifyOfPropertyChange(() => ContentText);
            }
        }
    }
}
