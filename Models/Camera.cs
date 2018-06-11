using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
	public class Camera
	{
		public Camera()
		{
			// capture = new Emgu.CV.VideoCapture();
		}

		public Picture Capture() => new Picture("taj.jpg");

		// private readonly Emgu.CV.VideoCapture capture;
	}
}
