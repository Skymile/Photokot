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
		public MainWindowVM(string filename = null) =>
			this.picture = filename != null ? new Picture(filename) : new Picture("apple.png");

		public void Apply(int width, int height) => 
			this.picture = camera?.Capture().Apply(
				EffectLibrary.Pixelize(width, height), 
				null
				//, new[] { ConvolutionMatrix.BoxFilter(width, height) }
			);

		private readonly Camera camera = new Camera();

		public void GetSource(ref Image image)
		{
			using (BitmapPointer ptr = picture.Pointer)
				image.Source = Imaging.CreateBitmapSourceFromHBitmap(
					ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
				);
		}

		private Picture picture;
	}
}
