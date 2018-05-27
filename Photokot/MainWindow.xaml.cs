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
			vm.Apply((int)BlockWidthSlider.Value, (int)BlockHeightSlider.Value);
			vm.GetSource(ref MainImage);
			tempLock = false;
		}

		private void BlockSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (vm != null && !tempLock)
			{
				vm.Apply((int)BlockWidthSlider.Value, (int)BlockHeightSlider.Value);
				vm.GetSource(ref MainImage);
			}
		}

		private bool tempLock = true;

		private MainWindowVM vm;
	}
}
