using Caliburn.Micro;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
	public class TabContentViewModel : Screen
	{
		public TabContentViewModel(string header)
		{
			DisplayName = header;
			ContentText = $"Datablock: {header}";
		}

		public string ContentText { get; }

        public List<SignalDisplayModel> Signals { get; set; } = new List<SignalDisplayModel>();

        public int Number { get; set; }
        public string NumberStr
        {
            get => $"DB{Number}";
        }      
    }
}
