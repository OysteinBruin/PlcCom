using Caliburn.Micro;
using PlcComUI.EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class SplashViewModel : Screen, IHandle<SplashStatusChangedEvent>
    {
        private IEventAggregator _events;
        private string _content;

        public SplashViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
        }

        public string Content 
        { 
            get => _content; 
            set
            {
                if (_content != value)
                {
                    _content = value;
                    NotifyOfPropertyChange(() => Content);
                }
            }
        }

        private int _sliderMaximum;

        public int SliderMaximum
        {
            get => _sliderMaximum; 
            set 
            { 
                _sliderMaximum = value;
                NotifyOfPropertyChange(() => SliderMaximum);
            }
        }


        private int _sliderValue;

        public int SliderValue
        {
            get => _sliderValue;
            set 
            { 
                _sliderValue = value;
                NotifyOfPropertyChange(() => SliderValue);
            }
        }


        public void Handle(SplashStatusChangedEvent message)
        {
            if (message.CloseDialog)
            {
                TryClose();
            }
            else 
            {
                Content = message.Content;
                SliderValue = message.Progress;

                if (message.ProgressMax != SliderMaximum)
                {
                    SliderMaximum = message.ProgressMax;
                }
            }
        }
    }
}
