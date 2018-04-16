using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Picture
	{
		//https://github.com/Skymile/Photokot

		public Picture(string filename)
		{
			this._Bitmap = new Bitmap(filename);
		}

		public Bitmap _Bitmap;
	}
}
