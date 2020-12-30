using AppSettings;
using Autofac;
using Caliburn.Micro;
using Dragablz;
using PlcComUI.Properties;
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

namespace PlcComUI
{
	public class Bootstrapper : BootstrapperBase
	{
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        

        protected override void Configure()
        {

            _container.Instance(_container);

            _container.Singleton<IWindowManager, WindowManager>()
                      .Singleton<IEventAggregator, EventAggregator>()
                      .Singleton<IUtilities, Utilities>()
                      .Singleton<IDatablockParser, DatablockParser>()
                      .Singleton<IJsonConfigFileParser, JsonConfigFileParser>()
                      .Singleton<IConfigManager, ConfigManager>()
                      .Singleton<IPlcComManager, PlcComManager>()
                      .Singleton<IInterTabClient, InterTabClient>()
                      .Singleton<IInterLayoutClient, InterLayoutClient>();
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

            if (Settings.Default.SettingsMain == null)
            {
                Settings.Default.SettingsMain = new SettingsMain();
            }

            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            Settings.Default.Save();
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





        //private Autofac.IContainer container;
        //public Bootstrapper() : base( true )
        //{
        //	Initialize();
        //}

        //      protected override void Configure()
        //      {
        //          var builder = new ContainerBuilder();
        //          var executingAssembly = Assembly.GetExecutingAssembly();
        //          builder.RegisterAssemblyTypes( executingAssembly )
        //              .AsSelf()
        //              .AsImplementedInterfaces();
        //          builder.RegisterAssemblyTypes( AssemblySource.Instance.ToArray() )
        //              .Where( type => type.Name.EndsWith( "ViewModel", StringComparison.Ordinal ) )
        //              .Where( type => !( string.IsNullOrWhiteSpace( type.Namespace ) ) && type.Namespace.EndsWith( nameof( ViewModels ), StringComparison.Ordinal ) )
        //              .Where( type => type.GetInterface( typeof( INotifyPropertyChanged ).Name ) != null )
        //              .AsSelf()
        //              .InstancePerDependency();
        //          builder.RegisterAssemblyTypes( AssemblySource.Instance.ToArray() )
        //              .Where( type => type.Name.EndsWith( "View", StringComparison.Ordinal ) )
        //              .Where( type => !( string.IsNullOrWhiteSpace( type.Namespace ) ) && type.Namespace.EndsWith( "Views", StringComparison.Ordinal ) )
        //              .AsSelf()
        //              .InstancePerDependency();
        //          builder.RegisterType<WindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
        //          builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();

        //          builder.RegisterType<Models.InterTabClient>().As<Dragablz.IInterTabClient>().InstancePerLifetimeScope();
        //          builder.RegisterType<Models.InterLayoutClient>().As<Dragablz.IInterLayoutClient>().InstancePerLifetimeScope();
        //          container = builder.Build();
        //      }

        //      protected override void OnStartup( object sender, StartupEventArgs e )
        //      {
        //          DisplayRootViewFor<ShellViewModel>();
        //      }

        //      protected override object GetInstance( Type service, string key )
        //      {
        //          if ( string.IsNullOrWhiteSpace( key ) )
        //          {
        //              if ( container.IsRegistered( service ) )
        //                  return container.Resolve( service );
        //          }
        //          else
        //          {
        //              if ( container.IsRegisteredWithKey( key, service ) )
        //                  return container.ResolveKeyed( key, service );
        //          }
        //          throw new Exception( $"Could not locate any instances of contract {key ?? service.Name}." );
        //      }

        //      protected override IEnumerable<object> GetAllInstances( Type service )
        //      {
        //          return container.Resolve( typeof( IEnumerable<> ).MakeGenericType( service ) ) as IEnumerable<object>;
        //      }

        //      protected override void BuildUp( object instance )
        //      {
        //          container.InjectProperties( instance );
        //      }

        //      protected override void OnUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
        //      {
        //          Console.WriteLine( $"Unhandled exception: {e.Exception.ToString()}" );
        //      }
    }
}
