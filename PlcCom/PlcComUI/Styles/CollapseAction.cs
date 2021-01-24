
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

// Copied from  https://blog.pixelingene.com/2010/08/simple-triggeraction-for-docking-using-gridsplitters/

namespace PlcComUI.Styles
{
	public class CollapseAction : TriggerAction<Button>
	{
		public Dock Direction { get; set; }
		protected override void Invoke(object parameter)
		{
			// First find the nearest splitter
			var splitter = FindVisual<GridSplitter>(AssociatedObject);

			if (splitter != null)
			{
				var grid = FindVisual<Grid>(splitter); // Find nearest Grid
				if (grid != null)
				{
					ApplyDock(grid);
				}
			}
		}

		private void ApplyDock(Grid grid)
		{
			var cDef1 = grid.ColumnDefinitions.FirstOrDefault();
			var cDef2 = grid.ColumnDefinitions.LastOrDefault();
			var rDef1 = grid.RowDefinitions.FirstOrDefault();
			var rDef2 = grid.RowDefinitions.LastOrDefault();
			switch (Direction)
			{
				case Dock.Left:
					cDef1.Width = new GridLength(0);
					cDef2.Width = new GridLength(1, GridUnitType.Star);
					break;
				case Dock.Right:
					cDef2.Width = new GridLength(7, GridUnitType.Star);
					cDef1.Width = new GridLength(3, GridUnitType.Star);
					break;
				case Dock.Top:
					rDef1.Height = new GridLength(0);
					rDef2.Height = new GridLength(1, GridUnitType.Star);
					break;
				case Dock.Bottom:
					rDef2.Height = new GridLength(0);
					rDef1.Height = new GridLength(1, GridUnitType.Star);
					break;
			}
		}

		private T FindVisual<T>(FrameworkElement relElt) where T : FrameworkElement
		{
			var parent = VisualTreeHelper.GetParent(relElt);

			while (parent != null && !(parent is T))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}

			return parent as T;
		}
	}
}