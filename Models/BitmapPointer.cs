using System;
using System.Runtime.InteropServices;

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
			if (!disposedValue)
			{
				NativeMethods.DeleteObject(ptr);
				disposedValue = true;
			}
		}
		
		~BitmapPointer() => Dispose(false);

		private IntPtr ptr;
		private bool disposedValue = false;
	}
}
