using System;
using System.Runtime.InteropServices;

namespace Models
{
	internal static class NativeMethods
	{
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteObject([In] IntPtr handle);
	}
}
