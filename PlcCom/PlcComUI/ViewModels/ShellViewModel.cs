using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Dragablz;
using PlcComLibrary.Config;
using System;
using System.Threading.Tasks;
using PlcComUI.EventModels;
using PlcComUI.Views;
using System.Media;
using System.ComponentModel;
using PlcComLibrary.PlcCom;

namespace PlcComUI.ViewModels
{
	public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<MessageEvent>
    {
		private IEventAggregator _events;
        private IPlcComManager _plcComManager;
        private bool _modalViewIsActive = false;
        private bool _showDrawer = false;

        public ShellViewModel(IEventAggregator events, IPlcComManager plcComManager)
		{
			_events = events;
            _plcComManager = plcComManager;
            _plcComManager.ConfigManager.ConfigsLoadingProgressChanged += OnConfigLoadingProgressChanged;
            foreach (var plc in _plcComManager.PlcServiceList)
            {
                plc.ComStateChanged += OnPlcComStateChanged;
            }
            Items.Add(IoC.Get<PlcComViewModel>());
            Items.Add(IoC.Get<PaletteSelectorViewModel>());
            _events.Subscribe(this);
		}

        

        protected override void OnInitialize()
		{
			base.OnInitialize();

            var windowManager = new WindowManager();
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.DoWork += InitializeApplication;
                bw.RunWorkerCompleted += InitializationCompleted;
                bw.RunWorkerAsync();
                
                windowManager.ShowDialog(new SplashViewModel(_events));
            }
        }

        private void InitializeApplication(object sender, DoWorkEventArgs e)
        {
            _plcComManager.LoadConfigs();
        }

        private void InitializationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _events.PublishOnUIThread(new SplashStatusChangedEvent(true));
            ActivateHomeView();
        }

        private void OnConfigLoadingProgressChanged(object sender, EventArgs args)
        {
            ConfigsProgressEventArgs configArgs = (ConfigsProgressEventArgs)args;
            string splashContent = $"Loading configs {configArgs.ProgressInput} of {configArgs.ProgressTotal}";
            _events.PublishOnUIThread(new SplashStatusChangedEvent(splashContent, configArgs.ProgressInput, configArgs.ProgressTotal));
        }

        public void ActivateHomeView()
        {
            ActivateItem(Items[0]);
        }

        //public void ActivatePaletteSelectorView()
        //{
        //    ActivateItem(Items[1]);
        //}

        public void ActivateSettingsView()
        {
            ActivateItem(Items[1]);
        }

        public async void Handle(MessageEvent message)
        {
            if (!_modalViewIsActive)
            {
                _modalViewIsActive = true;
                await ShowMessageDialog(message);
            }
        }

        //public bool ShowDrawer
        //{
        //    get => _showDrawer; 
        //    set 
        //    { 
        //        _showDrawer = value;
        //        NotifyOfPropertyChange(() => ShowDrawer);
        //    }
        //}

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

        //public List<>

        private void OnPlcComStateChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
