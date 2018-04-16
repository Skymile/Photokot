using System;
using System.Collections.Generic;
using System.Linq;
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
