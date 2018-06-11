using System;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

using Drawing = System.Drawing;
using Models;

namespace ViewModels
{
	public class MainWindowVM
	{
		public MainWindowVM(string filename = null) =>
			this.picture = filename != null ? new Picture(filename) : new Picture("taj.jpg");

		public void Apply(int width, int height, int slider) => 
			this.picture = camera?.Capture().Apply(
				EffectLibrary.MinRGB()
			//	EffectLibrary.Pixelize(new Drawing.Size(width, height)), null, null
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
