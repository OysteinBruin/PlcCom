
using Autofac;
using Caliburn.Micro;
using Dragablz;
using log4net;
using PlcComLibrary;
using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComUI.Models;
using PlcComUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PlcComLibrary.PlcCom;
using AutoMapper;
using PlcComLibrary.Models;
using Settings;
using PlcComLibrary.Models.Signal;
using PlcComLibrary.DbParser;

namespace PlcComUI
{
    public class Bootstrapper : BootstrapperBase
	{
        private SimpleContainer _container = new SimpleContainer();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Bootstrapper));

        public Bootstrapper()
        {
            Initialize();
        }

        private IMapper ConfigureAutoMapper(IEventAggregator events)
        {
            log.Info("ConfigureAutoMapper");
            var config = new MapperConfiguration(cfg => {



                cfg.CreateMap<SignalModel, SignalDisplayModel>(MemberList.Source)
                    .ForSourceMember(x => x.ByteCount, opt => opt.DoNotValidate());
                cfg.CreateMap<BoolSignalModel, BoolSignalDisplayModel>(MemberList.Source)
                    .IncludeBase<SignalModel, SignalDisplayModel>()
                    .ConstructUsing(source => new BoolSignalDisplayModel(events))
                    .ForSourceMember(x => x.ByteCount, opt => opt.DoNotValidate());
                cfg.CreateMap<Int16SignalModel, NumericSignalModel>(MemberList.Source)
                .IncludeBase<SignalModel, SignalDisplayModel>()
                    .ConstructUsing(source => new NumericSignalModel(events))
                    .ForSourceMember(x => x.ByteCount, opt => opt.DoNotValidate());
                cfg.CreateMap<Int32SignalModel, NumericSignalModel>(MemberList.Source)
                .IncludeBase<SignalModel, SignalDisplayModel>()
                    .ConstructUsing(source => new NumericSignalModel(events))
                    .ForSourceMember(x => x.ByteCount, opt => opt.DoNotValidate());
                cfg.CreateMap<FloatSignalModel, NumericSignalModel>(MemberList.Source)
                .IncludeBase<SignalModel, SignalDisplayModel>()
                    .ConstructUsing(source => new NumericSignalModel(events))
                    .ForSourceMember(x => x.ByteCount, opt => opt.DoNotValidate());

                cfg.CreateMap<SignalDisplayModel, SignalModel>();
                cfg.CreateMap<BoolSignalDisplayModel, BoolSignalModel>()
                    .IncludeBase<SignalDisplayModel, SignalModel>();
                cfg.CreateMap<NumericSignalModel, FloatSignalModel>()
                    .IncludeBase<SignalDisplayModel, SignalModel>();



                cfg.CreateMap<IDatablockModel, IDatablockDisplayModel>(MemberList.Source);
                cfg.CreateMap<DatablockModel, DatablockDisplayModel>(MemberList.Source);

                cfg.CreateMap<IDatablockDisplayModel, IDatablockModel>();
                cfg.CreateMap<DatablockDisplayModel, DatablockModel>();

            });

            try
            {
                config.AssertConfigurationIsValid();
            }
            catch (AutoMapperConfigurationException ex)
            {

                log.Error("AssertConfigurationIsValid failed", ex);
                throw;
            }
           

            return config.CreateMapper();
        }

        protected override void Configure()
        {
            _container.Singleton<IWindowManager, WindowManager>()
                      .Singleton<IEventAggregator, EventAggregator>()
                      .Singleton<IUtilities, Utilities>()
                      .Singleton<IDatablockParser, DatablockParser>()
                      .Singleton<IJsonConfigFileParser, JsonConfigFileParser>()
                      .Singleton<IConfigManager, ConfigManager>()
                      .Singleton<IPlcComManager, PlcComManager>()
                      .Singleton<IInterTabClient, InterTabClient>()
                      .Singleton<IInterLayoutClient, InterLayoutClient>();

            _container.Instance(_container);
            //container.Instance<IEventAggregator>();

         
            var eventAggregator = _container.GetInstance<IEventAggregator>();

            //ar eventAggregator = (EventAggregator)_container.GetInstance<EventAggregator>();
            var mapper = _container.Instance(ConfigureAutoMapper(eventAggregator));

           

            
            // .Singleton<IDataAccess, SqliteDataAccess>()

            //_container.PerRequest<IDataAccess, SqliteDataAccess>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            if (Properties.Settings.Default.SettingsMain == null)
            {
                Properties.Settings.Default.SettingsMain = new SettingsMain();
            }
            else
            {
                int mainWinWidth = Properties.Settings.Default.SettingsMain.MainWindow.Width;
                int mainWinHeight = Properties.Settings.Default.SettingsMain.MainWindow.Height;
                int splashWinWidth = Properties.Settings.Default.SettingsMain.SplashWindow.Width;
                int splashWinHeight = Properties.Settings.Default.SettingsMain.SplashWindow.Height;

                if (mainWinWidth > splashWinWidth)
                {
                    Properties.Settings.Default.SettingsMain.SplashWindow.Left = Properties.Settings.Default.SettingsMain.MainWindow.Left + (mainWinWidth / 2) - (splashWinWidth / 2);
                    Properties.Settings.Default.SettingsMain.SplashWindow.Top = Properties.Settings.Default.SettingsMain.MainWindow.Top + (mainWinHeight / 2) - (splashWinHeight / 2);
                }


            }

            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            Properties.Settings.Default.Save();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
