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
using PlcComUI.Models;
using PlcComUI.Domain;
using GongSolutions.Wpf.DragDrop;

namespace PlcComUI.ViewModels
{
    public class RealTimeGraphViewModel : Screen, GongSolutions.Wpf.DragDrop.IDropTarget
    {
        
        public RealTimeGraphViewModel()
        {
            ContentText = "Real Time Graph";
            DisplayName = ContentText;
            

            //DropTarget = new GraphDropHandler();
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1080;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(60);
            Controller.Range.AutoY = true;

            //IEventAggregator agg = new EventAggregator();
            //PlcComLibrary.Models.PlcComIndexModel indexModel = new PlcComLibrary.Models.PlcComIndexModel(0, 0, 0);
            //Signals.Add(new ISignalDisplayModel(indexModel, agg));
            //Signals[0].Name = "Signal 1";
            //Signals.Add(new ISignalDisplayModel(indexModel, agg));
            //Signals[1].Name = "Signal 2";
            //Signals.Add(new SignalDisplayModel(indexModel, agg));
            //Signals[2].Name = "Signal 3";

            //Signals2.Add(new SignalDisplayModel(indexModel, agg));
            //Signals2[0].Name = "Signal2 1";
            //Signals2.Add(new SignalDisplayModel(indexModel, agg));
            //Signals2[1].Name = "Signal2 2";
            //Signals2.Add(new SignalDisplayModel(indexModel, agg));
            //Signals2[2].Name = "Signal2 3";

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
        public string ContentText { get; }

        public List<SignalDisplayModel> Signals { get; set; } = new List<SignalDisplayModel>();

        public List<SignalDisplayModel> Signals2 { get; set; } = new List<SignalDisplayModel>();

        void GongSolutions.Wpf.DragDrop.IDropTarget.DragOver(IDropInfo dropInfo)
        {
            SignalDisplayModel sourceItem = dropInfo.Data as SignalDisplayModel;
            SignalDisplayModel targetItem = dropInfo.TargetItem as SignalDisplayModel;

            //if (sourceItem != null && targetItem != null && targetItem.CanAcceptChildren)
            //{
            //    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            //    dropInfo.Effects = DragDropEffects.Copy;
            //}
        }


        void GongSolutions.Wpf.DragDrop.IDropTarget.Drop(IDropInfo dropInfo)
        {

            SignalDisplayModel sourceItem = dropInfo.Data as SignalDisplayModel;
            SignalDisplayModel targetItem = dropInfo.TargetItem as SignalDisplayModel;

        }

    }


    /*
    
     public class MainWindowVM
    {
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }

        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> MultiController { get; set; }

        public MainWindowVM()
        {
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1080;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller.Range.AutoY = true;
            Controller.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            MultiController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            MultiController.Range.MinimumY = 0;
            MultiController.Range.MaximumY = 1080;
            MultiController.Range.MaximumX = TimeSpan.FromSeconds(10);
            MultiController.Range.AutoY = true;

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 1",
                Stroke = Colors.Red,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 2",
                Stroke = Colors.Green,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 3",
                Stroke = Colors.Blue,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 4",
                Stroke = Colors.Yellow,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 5",
                Stroke = Colors.Gray,
            });

           

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var y = System.Windows.Forms.Cursor.Position.Y;

                    List<DoubleDataPoint> yy = new List<DoubleDataPoint>()
                    {
                        y,
                        y + 20,
                        y + 40,
                        y + 60,
                        y + 80,
                    };

                    var x = watch.Elapsed;

                    List<TimeSpanDataPoint> xx = new List<TimeSpanDataPoint>()
                    {
                        x,
                        x,
                        x,
                        x,
                        x
                    };

                    Controller.PushData(x, y);
                    MultiController.PushData(xx, yy);

                    Thread.Sleep(30);
                }
            });
        }
    }
    */
}
