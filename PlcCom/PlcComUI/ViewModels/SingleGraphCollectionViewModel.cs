using Caliburn.Micro;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Screen = Caliburn.Micro.Screen;

namespace PlcComUI.ViewModels
{
    public class SingleGraphCollectionViewModel : Screen
    {
		private readonly Random _random = new Random();

		public SingleGraphCollectionViewModel()
		{
			DisplayName = "Single Graph Collection";
			//SelectedGraphCount = 4;

			Controller1 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
			Controller1.Range.MinimumY = 0;
			Controller1.Range.MaximumY = 1080;
			Controller1.Range.MaximumX = TimeSpan.FromSeconds(20);
			Controller1.Range.AutoY = true;

			Controller1.DataSeriesCollection.Add(new WpfGraphDataSeries()
			{
				Name = "Var 1",
				Stroke = Colors.Red,
			});

			Controller2 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
			Controller2.Range.MinimumY = 0;
			Controller2.Range.MaximumY = 1080;
			Controller2.Range.MaximumX = TimeSpan.FromSeconds(20);
			Controller2.Range.AutoY = true;

			Controller2.DataSeriesCollection.Add(new WpfGraphDataSeries()
			{
				Name = "Var 2",
				Stroke = Colors.Yellow,
			});

			Controller3 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
			Controller3.Range.MinimumY = 0;
			Controller3.Range.MaximumY = 1080;
			Controller3.Range.MaximumX = TimeSpan.FromSeconds(20);
			Controller3.Range.AutoY = true;

			Controller3.DataSeriesCollection.Add(new WpfGraphDataSeries()
			{
				Name = "Var 3",
				Stroke = Colors.CornflowerBlue,
			});

			Controller4 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
			Controller4.Range.MinimumY = 0;
			Controller4.Range.MaximumY = 1080;
			Controller4.Range.MaximumX = TimeSpan.FromSeconds(20);
			Controller4.Range.AutoY = true;

			Controller4.DataSeriesCollection.Add(new WpfGraphDataSeries()
			{
				Name = "Var 4",
				Stroke = Colors.DarkOliveGreen,
			});


			Stopwatch watch = new Stopwatch();
			watch.Start();

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					var x = watch.Elapsed;

					var y1 = Cursor.Position.Y; // Math.Sin(x.TotalSeconds) + _random.Next(0,5) / 32000 * 1 * Math.Sin(x.TotalSeconds / 0.3843); //Cursor.Position.Y;
					Controller1.PushData(x, y1);

					var y2 = Math.Cos(x.TotalSeconds)  / 20000 * 0.5 * Math.Cos(x.TotalSeconds / 0.4364) + (_random.NextDouble() ); //Cursor.Position.Y;
					Controller2.PushData(x, y2);

					var y3 = Cursor.Position.Y + (_random.NextDouble() * _random.Next(10,40));
					Controller3.PushData(x, y3);

					var y4 =  Math.Cos(x.TotalSeconds) + _random.Next(0, 5) / 32000 * 1 * Math.Cos(x.TotalSeconds / 0.1843); //Cursor.Position.Y;
					Controller4.PushData(x, y4);


					Thread.Sleep(30);
				}
			});
		}

		public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller1 { get; set; }
		public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller2 { get; set; }
		public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller3 { get; set; }
		public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller4 { get; set; }

		//public ObservableCollection<ObservableCollection<SingleGraphViewModel>> GraphMatrix
		//{
		//	get => _graphMatrix;
		//	set 
		//	{
		//		_graphMatrix = value;
		//	}
		//}

		//private int _gridWidth = 4;
		//public int GridWidth
		//{
		//	get { return _gridWidth; }
		//	set
		//	{
		//		if (Equals(_gridWidth, value))
		//			return;

		//		_gridWidth = value;
		//		NotifyOfPropertyChange(() => GridWidth);
		//	}
		//}

		//private int _gridHeight = 2;
		//public int GridHeight
		//{
		//	get { return _gridHeight; }
		//	set
		//	{
		//		if (Equals(_gridHeight, value))
		//			return;

		//		_gridHeight = value;
		//		NotifyOfPropertyChange(() => GridHeight);
		//	}
		//}
		//public int SelectedGraphCount
		//{
		//	get => _selectedGraphCount;
		//          set 
		//	{
		//		if (Equals(_selectedGraphCount, value))
		//			return;

		//		_selectedGraphCount = value;
		//		GraphMatrix = CreateMatrix();
		//		NotifyOfPropertyChange(() => SelectedGraphCount);
		//	}
		//      }


		//      private ObservableCollection<ObservableCollection<SingleGraphViewModel>> CreateMatrix()
		//{
		//	var graphMatrix = new ObservableCollection<ObservableCollection<SingleGraphViewModel>>();


		//	if (SelectedGraphCount == 1)
		//	{
		//		GridHeight = 1;
		//		GridWidth = 1;

		//	}
		//	else if (SelectedGraphCount == 2)
		//	{
		//		GridHeight = 1;
		//		GridWidth = 2;
		//	}
		//	else if (SelectedGraphCount == 4)
		//	{
		//		GridHeight = 2;
		//		GridWidth = 2;
		//	}
		//	else if (SelectedGraphCount == 8)
		//	{
		//		GridHeight = 2;
		//		GridWidth = 4;
		//	}

		//	for (var posRow = 0; posRow < GridHeight; posRow++)
		//	{
		//		var row = new ObservableCollection<SingleGraphViewModel>();
		//		for (var posCol = 0; posCol < GridWidth; posCol++)
		//		{
		//			var model = new SingleGraphViewModel();
		//			row.Add(model);
		//		}
		//		graphMatrix.Add(row);
		//	}
		//	return graphMatrix;
		//}
	}
}
