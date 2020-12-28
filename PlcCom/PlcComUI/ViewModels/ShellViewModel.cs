using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Dragablz;
using PlcComLibrary;
using System;
using System.Threading.Tasks;
using PlcComUI.EventModels;
using PlcComUI.Views;
using System.Media;

namespace PlcComUI.ViewModels
{
	public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<MessageEvent>
    {
		private TabablzViewModel _tabablzViewModel;
		private PaletteSelectorViewModel _paletteSelectorVM;
		private IEventAggregator _events;
		private IConfigManager _configManager;
        private bool _modalViewIsActive = false;
        private bool _showDrawer = false;

        public ShellViewModel(IEventAggregator events, IConfigManager configManager)
		{
			_events = events;
			_configManager = configManager;

            _events.Subscribe(this);
			_configManager.LoadConfigs();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_tabablzViewModel = IoC.Get<TabablzViewModel>();
			ActivateItem(_tabablzViewModel);
		}

        public void ActivateHomeView()
        {
            _tabablzViewModel = IoC.Get<TabablzViewModel>();
            ActivateItem(_tabablzViewModel);
            ShowDrawer = false;
        }

        public void ActivatePaletteSelectorView()
        {
            _paletteSelectorVM = IoC.Get<PaletteSelectorViewModel>();
            ActivateItem(_paletteSelectorVM);
            ShowDrawer = false;
        }

        public void ActivateSettingsView()
        {
            _paletteSelectorVM = IoC.Get<PaletteSelectorViewModel>();
            ActivateItem(_paletteSelectorVM);
        }

        public async void Handle(MessageEvent message)
        {
            if (!_modalViewIsActive)
            {
                _modalViewIsActive = true;
                await ShowMessageDialog(message);
            }
        }

        public bool ShowDrawer
        {
            get => _showDrawer; 
            set 
            { 
                _showDrawer = value;
                NotifyOfPropertyChange(() => ShowDrawer);
            }
        }

        private async Task ShowMessageDialog(MessageEvent ev)
        {
            var view = new ErrorMessageView
            {
                DataContext = new ErrorDialogViewModel()
            };

            view.HeaderText.Text = ev.HeaderText;
            view.ContentText.Text = ev.ContentText;

            if (ev.MessageLevel > MessageEvent.Level.Info)
            {
                SystemSounds.Hand.Play();
            }

            await DialogHost.Show(view, "MainDialogHost");

            _modalViewIsActive = false;
            // https://stackoverflow.com/questions/49965223/how-to-open-a-material-design-dialog-from-the-code-xaml-cs
        }

        // TODO: Implement can close check
        public override void CanClose(Action<bool> callback)
        {
            //base.CanClose(callback);

            //callback(false);
            callback(true);
        }



        // TODO: add Squirrel installer
        //private async Task CheckForUpdates()
        //{
        //    string devPath = @"C:\dev\C#\Releases\PlcUnitTest";//@"C:\dev\C#\Releases\PlcUnitTest";
        //    string prodPath = @"T:\Plc UnitTest\Releases";//@"T:\Plc UnitTest\Releases";

        //    if (Directory.Exists(devPath))
        //    {
        //        Console.WriteLine($"ShellViewModel.Ctor - CheckForUpdates() - from prod dir {devPath}");
        //        using (var manager = new UpdateManager(devPath))
        //        {
        //            await manager.UpdateApp();
        //        }
        //    }
        //    else if (Directory.Exists(prodPath))
        //    {
        //        Console.WriteLine($"ShellViewModel.Ctor - CheckForUpdates() - from prod dir {prodPath}");
        //        using (var manager = new UpdateManager(prodPath))
        //        {
        //            await manager.UpdateApp();
        //        }
        //    }
        //}
    }
}
