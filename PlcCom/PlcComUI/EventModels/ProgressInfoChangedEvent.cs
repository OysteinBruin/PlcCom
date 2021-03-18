using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.EventModels
{
    public class ProgressInfoChangedEvent
    {
        public string Content { get; set; }

        public int ProgressMax { get; set; }
        public int Progress { get; set; }
        public bool CloseDialog { get; set; } = false;



        public ProgressInfoChangedEvent(string content, int value = 0, int progressMax = 0)
        {
            Content = content;
            Progress = value;
            ProgressMax = progressMax;

        }

        public ProgressInfoChangedEvent(bool closeDialog)
        {
            CloseDialog = closeDialog;
        }
    }
}
