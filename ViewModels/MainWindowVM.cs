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

		public void Apply()
		{

		}

		private Camera camera = new Camera();

		public void GetSource(ref Image image)
		{
			Picture picture = camera.Capture().Apply(null);

			using (BitmapPointer ptr = picture.Pointer)
				image.Source = Imaging.CreateBitmapSourceFromHBitmap(
					ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
				);

			//if (picture == picture)
			//	;
		}

		private Picture picture;
	}
}
