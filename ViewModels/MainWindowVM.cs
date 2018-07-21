using System;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

using Drawing = System.Drawing;
using Models;

namespace ViewModels
{
	public sealed class MainWindowVM : IDisposable
	{
		public MainWindowVM(string filename = null) =>
			this.Picture = filename != null ? new Picture(filename) : new Picture("taj.jpg");

		public void Apply(int width, int height, int slider)
		{
			this.Picture = camera?.Capture();

			var tmp = this.Picture.Apply(
			    (EffectLibrary.MinRGB(), null, null, null),
			    (EffectLibrary.Pixelize(new Drawing.Size(width, height)), null, null, null),
			    (EffectLibrary.Sobel(), null, null, new[] {
			        ConvolutionMatrix.SobelHorizontal, ConvolutionMatrix.SobelVertical
			    })
			);

			//this.picture = tmp.Apply(
			//	EffectLibrary.HalfBlend(slider / 32.0), null, null, null, picture
			//);
			this.Picture = new Picture(tmp.Data, tmp.Width, tmp.Height);
		}

		private readonly Camera camera = new Camera();

		public void GetSource(ref Image image)
		{
			using (BitmapPointer ptr = this.Picture.Pointer)
				image.Source = Imaging.CreateBitmapSourceFromHBitmap(
					ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
				);
		}

		public void Dispose()
		{
			this.Picture.Dispose();
			GC.SuppressFinalize(this);
		}

		public Picture Picture { get; set; }
	}
}
