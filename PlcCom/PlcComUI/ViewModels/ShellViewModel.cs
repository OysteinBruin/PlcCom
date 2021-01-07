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

                //_plcComManager.ConfigManager.



                /*
                protected override void OnInitialize()
		        {
			        base.OnInitialize();
                    var windowManager = new WindowManager();

                    using (BackgroundWorker bw = new BackgroundWorker())
                    {
                        bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                        bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                        bw.WorkerReportsProgress = true;
            
                        this.button1.Click += new EventHandler(button1_Click);
                    }
                }

                private void button1_Click(object sender, EventArgs e)
                {
                    if (!this.bw.IsBusy)
                    {
                        this.bw.RunWorkerAsync();
                        this.button1.Enabled = false;
                    }
                }

                private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
                {
                    this.label1.Text = "The answer is: " + e.Result.ToString();
                    this.button1.Enabled = true;
                }

                private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
                {
                    this.label1.Text = e.ProgressPercentage.ToString() + "% complete";
                }

                private void bw_DoWork(object sender, DoWorkEventArgs e)
                {
                    BackgroundWorker worker = (BackgroundWorker)sender;
                    for (int i = 0; i < 100; ++i)
                    {
                        // report your progres
                        worker.ReportProgress(i);

                        // pretend like this a really complex calculation going on eating up CPU time
                        System.Threading.Thread.Sleep(100);
                    }
                    e.Result = "42";
                }

                 */
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

        public void ActivatePaletteSelectorView()
        {
            ActivateItem(Items[1]);
        }

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
