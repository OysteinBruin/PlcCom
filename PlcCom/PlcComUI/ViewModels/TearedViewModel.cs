using Caliburn.Micro;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class TearedViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public TearedViewModel(
            IInterTabClient interTabClient,
            IInterLayoutClient interLayoutClient )
        {
            InterTabClient = interTabClient;
            InterLayoutClient = interLayoutClient;
        }
        public IInterTabClient InterTabClient { get; }
        public IInterLayoutClient InterLayoutClient { get; }
    }
}
