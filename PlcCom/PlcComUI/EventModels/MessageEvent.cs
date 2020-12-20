using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.EventModels
{
    public class MessageEvent
    {
        public enum Level
        {
            Info,
            Warn,
            Error,
            Fatal
        }

        public Level MessageLevel { get; }

        public string HeaderText { get; }

        public string ContentText { get; }

        public MessageEvent(string headerMessage, string contentMessage, Level level)
        {
            HeaderText = headerMessage;
            ContentText = contentMessage;
            MessageLevel = level;
        }
    }
}
