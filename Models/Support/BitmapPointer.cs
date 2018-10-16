using System;

namespace Models
{
	public class BitmapPointer : IDisposable
	{
		public BitmapPointer(Picture picture) =>
			this.ptr = picture.Handle;

		public static implicit operator IntPtr(BitmapPointer bitmapPointer) =>
			bitmapPointer.ptr;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				NativeMethods.DeleteObject(this.ptr);
				this.disposedValue = true;
			}
		}

		~BitmapPointer() => Dispose(false);

		private IntPtr ptr;
		private bool disposedValue = false;
	}
}
