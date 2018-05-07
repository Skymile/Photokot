using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Picture
	{
		public Picture(string filename)
		{
			this._Bitmap = new Bitmap(filename);
		}

		internal Picture(Bitmap bitmap)
		{
			this._Bitmap = bitmap;
		}

		public unsafe Picture Apply(Effect effect)
		{
			effect = new Effect(
				(readPtr, writePtr, x, y) =>
				{
					byte* r = (byte*)readPtr.ToPointer();
					byte* w = (byte*)writePtr.ToPointer();

					w[0] = w[1] = w[2] = (byte)((r[0] + r[1] + r[2]) / 3);

				}, 3, 3
			);

			BitmapData readData = _Bitmap.LockBits(
				new Rectangle(Point.Empty, _Bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb
			);

			Bitmap writeBitmap = new Bitmap(_Bitmap.Width, _Bitmap.Height);

			BitmapData writeData = writeBitmap.LockBits(
				new Rectangle(Point.Empty, _Bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb
			);
			
			IntPtr read = readData.Scan0;
			IntPtr write = writeData.Scan0;

			for (int i = 0; i < _Bitmap.Height; i++)
			{
				for (int j = 0; j < _Bitmap.Width; j++)
				{
					int offset = i * readData.Stride + j * 3;

					effect.Apply(read + offset, write + offset, 3, readData.Stride);
				}
			}

			_Bitmap.UnlockBits(readData);
			writeBitmap.UnlockBits(writeData);
			return new Picture(writeBitmap);
		}

		public BitmapPointer Pointer => new BitmapPointer(this);

		public Bitmap _Bitmap;
	}
}
