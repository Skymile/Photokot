namespace Models
{
	public static class OperationMatrix
	{
		public static readonly int[] Identity = new[] { 0 };

		public static int[] Default(int width, int height, int stride, int bytesPerPixel)
		{
			int[] offsets = new int[width * height];
			for (int i = 0; i < height; i++)
			{
				int yval = stride * (i - height / 2);
				for (int j = 0; j < width; j++)
					offsets[i * width + j] = yval + bytesPerPixel * (j - width / 2);
			}
			return offsets;
		}
	}
}
