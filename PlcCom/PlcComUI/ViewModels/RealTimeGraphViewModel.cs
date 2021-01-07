using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Forms;
using Caliburn.Micro;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using Screen = Caliburn.Micro.Screen;

namespace PlcComUI.ViewModels
{
    public class RealTimeGraphViewModel : Screen
    {
        public RealTimeGraphViewModel(string header)
        {
            DisplayName = header;
            ContentText = "Real Time Graph";

            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1080;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(60);
            Controller.Range.AutoY = true;

            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });


            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var y = Cursor.Position.Y;

                    List<DoubleDataPoint> yy = new List<DoubleDataPoint>()
                    {
                        y,
                        y + 20,
                        y + 40,
                        y + 60,
                        y + 80,
                    };

                    var x = watch.Elapsed;

                    Controller.PushData(x, y);

                    Thread.Sleep(30);
                }
            });
        }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }
        public string ContentText { get; }
    }
}
