using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using Models;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ViewModels
{
	public class MainWindowVM
	{
		public MainWindowVM(string filename = null)
		{
			if (filename != null)
				this.picture = new Picture(filename);
		}

		public void Apply(in int x)
		{

		}

		private Camera camera = new Camera();

		public void GetSource(ref Image image)
		{
			using (BitmapPointer ptr = camera.Capture().Apply(null).Pointer)
				image.Source = Imaging.CreateBitmapSourceFromHBitmap(
					ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
				);
		}

		private Picture picture;
	}
}
