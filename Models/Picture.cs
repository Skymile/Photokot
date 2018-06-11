using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

		public Picture(int width, int height) : this(new Bitmap(width, height))
		{ }

		internal Picture(Bitmap bitmap) => this._Bitmap = bitmap;

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

		public override bool Equals(object obj) => 
			obj is Picture picture &&
			EqualityComparer<BitmapPointer>.Default.Equals(this.Pointer, picture.Pointer) &&
			EqualityComparer<Bitmap>.Default.Equals(this._Bitmap, picture._Bitmap);

		public override int GetHashCode()
		{
			var hashCode = 1485453679;
			hashCode = hashCode * -1521134295 + EqualityComparer<BitmapPointer>.Default.GetHashCode(this.Pointer);
			hashCode = hashCode * -1521134295 + EqualityComparer<Bitmap>.Default.GetHashCode(this._Bitmap);
			return hashCode;
		}

		public Picture Apply(
			(Effect effect, int[] readBlock, int[] writeBlock, object[] parameters) first,
			params (Effect effect, int[] readBlock, int[] writeBlock, object[] parameters)[] multiEffect
			)
		{
			Picture picture = Apply(first.effect, first.readBlock, first.writeBlock, first.parameters);
			foreach (var (effect, readBlock, writeBlock, parameters) in multiEffect)
				picture = picture.Apply(effect, readBlock, writeBlock, parameters);
			return picture;
		}

		public unsafe Picture Apply(
			Effect effect, 
			int[] readBlock        = null, 
			int[] writeBlock       = null, 
			object[] parameters    = null, 
			Picture writtenPicture = null
		)
		{
			BitmapData readData = this.FullLock(ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			Picture newPicture = writtenPicture ?? new Picture(_Bitmap.Width, _Bitmap.Height);
			BitmapData writeData = newPicture.FullLock(ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			IntPtr read = readData.Scan0, 
			       write = writeData.Scan0;

			effect.SetSize(
				readBlock ?? OperationMatrix.Default(effect.ReadBlock.Width, effect.ReadBlock.Height, readData.Stride, 3),
				writeBlock ?? OperationMatrix.Default(effect.WriteBlock.Width, effect.WriteBlock.Height, readData.Stride, 3)
			);

			Size max = effect.MaxSize;

			int innerCondition = _Bitmap.Width - max.Width / 2;

			for (int i = max.Height / 2; i < _Bitmap.Height - max.Height / 2; i += effect.WriteBlock.Height)
			{
				int offset = i * readData.Stride;
				for (int j = max.Width / 2; j < innerCondition; j += effect.WriteBlock.Width)
					effect.Apply(read + offset + j * 3, write + offset + j * 3, parameters);
			}

			this.Unlock(readData);
			newPicture.Unlock(writeData);
			return newPicture;
		}

		public BitmapPointer Pointer => new BitmapPointer(this);

		public Bitmap _Bitmap;
	}
}
