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

namespace ViewModels
{
	public class MainWindowVM
	{
		public MainWindowVM(string filename = null)
		{
			if (filename != null)
				this.picture = new Picture(filename);
		}

		public void Apply()
		{

		}

		private Camera camera = new Camera();

		public void GetSource(ref Image image)
		{
			IntPtr ptr = camera.Capture()._Bitmap.GetHbitmap();

			image.Source = Imaging.CreateBitmapSourceFromHBitmap(
				ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
			);

			if (!Picture.DeleteObject(ptr))
				throw new SystemException(nameof(ptr));
		}

		private Picture picture;
	}
}
