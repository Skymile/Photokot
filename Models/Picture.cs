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
		public Picture(string filename = null)
		{
			if (filename != null)
				this._Bitmap = new Bitmap(filename);
		}

		internal Picture(Bitmap bitmap)
		{
			this._Bitmap = bitmap;
		}

		private BitmapData FullLock(ImageLockMode mode, PixelFormat format) =>
			_Bitmap.LockBits(new Rectangle(Point.Empty, _Bitmap.Size), mode, format);

		private void Unlock(BitmapData data) => _Bitmap.UnlockBits(data);

		public unsafe List<Point> GetDifferences(Picture picture)
		{
			BitmapData oldData = FullLock(ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			BitmapData newData = picture.FullLock(ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			List<Point> points = new List<Point>();

			byte* left = (byte*)oldData.Scan0.ToPointer();
			byte* right = (byte*)newData.Scan0.ToPointer();

			for (int i = 0; i < oldData.Height; i++)
				for (int j = 0; j < oldData.Width; j++)
				{
					int offset = i * oldData.Stride + j * 3;
					if (left[offset] != right[offset] ||
						left[offset + 1] != right[offset + 1] ||
						left[offset + 2] != right[offset + 2])
						points.Add(new Point(j, i));
				}

			Unlock(oldData);
			picture.Unlock(newData);
			return points;
		}

		public unsafe static bool operator ==(Picture l, Picture r)
		{
			if (l.Equals(null) || r.Equals(null) || l._Bitmap.Size != r._Bitmap.Size)
				return false;

			BitmapData leftData = l.FullLock(ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			BitmapData rightData = r.FullLock(ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			byte* left = (byte*)leftData.Scan0.ToPointer();
			byte* right = (byte*)rightData.Scan0.ToPointer();

			bool returnValue = true;

			int chunk = leftData.Height / 4;

			Task[] tasks = new Task[4];
			for (int k = 0; k < tasks.Length; k++)
			{
				int t = k;
			
				tasks[k] = Task.Run(() =>
				{
					for (int i = chunk * t; i < chunk * t + chunk; i++)
						for (int j = 0; j < leftData.Stride; j++)
						{
							int offset = i * leftData.Stride + j;

							if (left[offset] != right[offset])
								returnValue = false;
						}
				});
			}
			Task.WaitAll(tasks);
			l.Unlock(leftData);
			r.Unlock(rightData);
			return returnValue;
		}

		public static bool operator !=(Picture l, Picture r) => !(l == r);

		public unsafe Picture Apply(Effect effect)
		{
			BitmapData readData = _Bitmap.LockBits(
				new Rectangle(Point.Empty, _Bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb
			);

			Bitmap writeBitmap = new Bitmap(_Bitmap.Width, _Bitmap.Height);

			BitmapData writeData = writeBitmap.LockBits(
				new Rectangle(Point.Empty, _Bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb
			);

			IntPtr read = readData.Scan0;
			IntPtr write = writeData.Scan0;

			effect.SetSize(3, readData.Stride, null);

			for (int i = 0; i < _Bitmap.Height; i++)
			{
				for (int j = 0; j < _Bitmap.Width; j++)
				{
					int offset = i * readData.Stride + j * 3;

					effect.Apply(read + offset, write + offset);
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
