using Caliburn.Micro;
using Dragablz;
using PlcComUI.ViewModels;
using PlcComUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlcComUI.Models
{
    public class InterTabClient : IInterTabClient
    {
        private readonly Func<TearedViewModel> _factory;

        public InterTabClient( Func<TearedViewModel> factory )
        {
            _factory = factory;
        }
        public INewTabHost<Window> GetNewHost( IInterTabClient interTabClient, object partition, TabablzControl source )
        {
            var vm = _factory();
            var v = new TearedView();
            
            ViewModelBinder.Bind( vm, v, null );
            v.Tabs.InterTabController = new InterTabController()
            {
                InterTabClient = this
            };
            return new NewTabHost<Window>( v, v.Tabs );
        }

        public TabEmptiedResponse TabEmptiedHandler( TabablzControl tabControl, Window window )
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
