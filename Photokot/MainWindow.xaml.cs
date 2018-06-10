using System;
using System.Diagnostics;
using System.Windows;

using ViewModels;

namespace Photokot
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			vm = new MainWindowVM();
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();
			vm.Apply((int)BlockWidthSlider.Value, (int)BlockHeightSlider.Value, (int)BlockSlider.Value);
			sw.Stop();
			StatusLabel.Content = ElapsedTime(sw.Elapsed);
			vm.GetSource(ref MainImage);
			tempLock = false;
		}

		private void BlockSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (vm != null && !tempLock)
			{
				Stopwatch sw = Stopwatch.StartNew();
				vm.Apply((int)BlockWidthSlider.Value, (int)BlockHeightSlider.Value, (int)BlockSlider.Value);
				sw.Stop();
				vm.GetSource(ref MainImage);
				StatusLabel.Content = ElapsedTime(sw.Elapsed);
			}
		}

		private string ElapsedTime(TimeSpan time) => $"{time.Milliseconds} ms, {time.Ticks} ticks";

		private bool tempLock = true;

		private MainWindowVM vm;
	}
}
