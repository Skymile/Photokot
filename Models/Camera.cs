using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Camera
	{
		public Camera()
		{
			capture = new Emgu.CV.VideoCapture();
		}

		public Picture Capture()
		{
			return new Picture(capture.QueryFrame().Bitmap);
		}

		private Emgu.CV.VideoCapture capture;
	}
}
