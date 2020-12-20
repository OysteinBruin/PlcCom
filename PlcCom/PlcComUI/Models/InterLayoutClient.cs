using Caliburn.Micro;
using Dragablz;
using PlcComUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlcComUI.Models
{
    public class InterLayoutClient : DefaultInterLayoutClient, IInterLayoutClient
    {
        private readonly Func<TearedViewModel> _factory;
        public InterLayoutClient( Func<TearedViewModel> factory )
        {
            _factory = factory;
        }

        INewTabHost<UIElement> IInterLayoutClient.GetNewHost( object partition, TabablzControl source )
        {
            var tabHost = base.GetNewHost( partition, source );
            var vm = _factory();
            ViewModelBinder.Bind( vm, tabHost.Container, null );
            return tabHost;
        }
    }
}
