using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class BitmapPointer : IDisposable
	{
		public BitmapPointer(Picture picture)
		{
			this.ptr = picture._Bitmap.GetHbitmap();
		}

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
				if (!DeleteObject(ptr))
					throw new SystemException(nameof(ptr));
				disposedValue = true;
			}
		}

		~BitmapPointer() {
		  Dispose(false);
		}

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr intPtr);

		private IntPtr ptr;
		private bool disposedValue = false;
	}
}
