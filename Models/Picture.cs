using System;
using System.Collections.Generic;
using System.Drawing;
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

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr intPtr);

		public Bitmap _Bitmap;
	}
}
