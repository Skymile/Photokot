using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Models
{
	public unsafe class Picture : IDisposable
	{
		public Picture(byte[] data, int width, int height)
		{
			//Contract.Requires<DataMisalignedException>(data.Length % width == 0);
			//Contract.Requires<DataMisalignedException>(data.Length % height == 0);

			int bpp = data.Length / width / height << 3;
			PixelFormat format = GetFormatFromBpp(bpp) ?? throw new DataMisalignedException(nameof(bpp));
			this.bitmap = new Bitmap(width, height, format);
			this.Data = data;
		}

		public Picture(string filename) => 
			this.bitmap = new Bitmap(filename);

		public Picture(int width, int height) : this(new Bitmap(width, height))
		{ }

		internal Picture(Bitmap bitmap) => this.bitmap = bitmap;

		public byte[] Data
		{
			get
			{
				var data = Lock(PixelFormat.Format24bppRgb, ImageLockMode.ReadOnly);
				int length = data.Stride * data.Height;
				byte[] bytes = new byte[length];

				Marshal.Copy(data.Scan0, bytes, 0, length);

				Unlock(data);
				return bytes;
			}

			set
			{
				var data = Lock(PixelFormat.Format24bppRgb, ImageLockMode.WriteOnly);

				Marshal.Copy(value, 0, data.Scan0, data.Stride * data.Height);

				Unlock(data);
			}
		}

		public BitmapData Lock(PixelFormat format, ImageLockMode mode = ImageLockMode.ReadWrite, Rectangle? rectangle = null) =>
			this.bitmap.LockBits(rectangle ?? new Rectangle(Point.Empty, this.bitmap.Size), mode, format);

		public Picture Unlock(BitmapData data)
		{
			this.bitmap.UnlockBits(data);
			return this;
		}

		public List<Point> GetDifferences(Picture picture)
		{
			BitmapData oldData = Lock(PixelFormat.Format24bppRgb, ImageLockMode.ReadOnly);
			BitmapData newData = picture.Lock(PixelFormat.Format24bppRgb, ImageLockMode.ReadOnly);

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

		public static bool operator ==(Picture l, Picture r)
		{
			if (l.Equals(null) || r.Equals(null) || l.bitmap.Size != r.bitmap.Size)
				return false;

			BitmapData leftData = l.Lock (PixelFormat.Format24bppRgb, ImageLockMode.ReadOnly);
			BitmapData rightData = r.Lock(PixelFormat.Format24bppRgb, ImageLockMode.ReadOnly);

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
			EqualityComparer<Bitmap>.Default.Equals(this.bitmap, picture.bitmap);

		public override int GetHashCode()
		{
			var hashCode = 1485453679;
			hashCode = hashCode * -1521134295 + EqualityComparer<BitmapPointer>.Default.GetHashCode(this.Pointer);
			hashCode = hashCode * -1521134295 + EqualityComparer<Bitmap>.Default.GetHashCode(this.bitmap);
			return hashCode;
		}

		public Picture Apply(
			(IEffect effect, int[] readBlock, int[] writeBlock, object[] parameters) first,
			params (IEffect effect, int[] readBlock, int[] writeBlock, object[] parameters)[] multiEffect
			)
		{
			Picture picture = Apply(first.effect, first.readBlock, first.writeBlock, first.parameters);
			foreach (var (effect, readBlock, writeBlock, parameters) in multiEffect)
				picture = picture.Apply(effect, readBlock, writeBlock, parameters);
			return picture;
		}

		public unsafe Picture Apply(
			IEffect effect, 
			int[] readBlock        = null, 
			int[] writeBlock       = null, 
			object[] parameters    = null, 
			Picture writtenPicture = null
		)
		{
			BitmapData readData = Lock(PixelFormat.Format24bppRgb, ImageLockMode.ReadOnly);

			Picture newPicture = writtenPicture ?? new Picture(bitmap.Width, bitmap.Height);
			BitmapData writeData = newPicture.Lock(PixelFormat.Format24bppRgb, ImageLockMode.ReadWrite);

			IntPtr read = readData.Scan0, 
			       write = writeData.Scan0;

			effect.SetSize(
				readBlock ?? OperationMatrix.Default(effect.ReadBlock.Width, effect.ReadBlock.Height, readData.Stride, 3),
				writeBlock ?? OperationMatrix.Default(effect.WriteBlock.Width, effect.WriteBlock.Height, readData.Stride, 3)
			);

			Size max = effect.MaxSize;

			int innerCondition = bitmap.Width - max.Width / 2;

			for (int i = max.Height / 2; i < bitmap.Height - max.Height / 2; i += effect.WriteBlock.Height)
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

		public IntPtr Handle => 
			handle = GetHandle(null);
		
		public IntPtr GetHandle(Color? color = null) =>
			handle = this.bitmap.GetHbitmap(color ?? Color.Black);

		public int Width => bitmap.Width;

		public int Height => bitmap.Height;

		internal PixelFormat? GetFormatFromBpp(int bpp) => 
			bppToFormat.TryGetValue(bpp, out var value) ? (PixelFormat?)value : null;

		internal static Dictionary<int, PixelFormat> bppToFormat = new Dictionary<int, PixelFormat>()
		{
			{  8, PixelFormat.Format8bppIndexed },
			{ 24, PixelFormat.Format24bppRgb    },
			{ 32, PixelFormat.Format32bppArgb   },
			{ 48, PixelFormat.Format48bppRgb    },
			{ 64, PixelFormat.Format64bppArgb   },
		};

		private readonly Bitmap bitmap;
		private IntPtr handle;

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
					this.bitmap.Dispose();

				if (handle != null)
					NativeMethods.DeleteObject(handle);
				disposedValue = true;
			}
		}

		~Picture() => Dispose(false);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
