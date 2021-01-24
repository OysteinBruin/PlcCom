using Screen = Caliburn.Micro.Screen;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class SingleGraphViewModel : Screen
    {
        public SingleGraphViewModel()
        {
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1080;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(60);
            Controller.Range.AutoY = true;

            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Var 1",
                Stroke = Colors.Red,
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
    }
}
