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
