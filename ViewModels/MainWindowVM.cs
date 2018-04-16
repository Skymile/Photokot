﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using Models;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ViewModels
{
	public class MainWindowVM
	{
		public MainWindowVM(string filename)
		{
			this.picture = new Picture(filename);
		}

		public void Apply()
		{

		}

		public void GetSource(ref Image image)
		{
			IntPtr ptr = picture._Bitmap.GetHbitmap();

			image.Source = Imaging.CreateBitmapSourceFromHBitmap(
				ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
			);

			// todo releasing
		}

		private Picture picture;
	}
}