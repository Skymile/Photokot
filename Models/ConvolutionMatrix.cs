using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public static class ConvolutionMatrix
	{
		public static readonly int[] Identity = new[] { 1 };

		public static int[] BoxFilter(int width, int height)
		{
			var matrix = new int[width * height];
			for (int i = 0; i < matrix.Length; i++)
				matrix[i] = 1;
			return matrix;
		}

		public static readonly int[] SobelHorizontal = new[]
		{
			-1, -2, -1,
			 0,  0,  0,
			 1,  2,  1
		};

		public static readonly int[] SobelVertical = new[]
		{
			-1, 0, 1,
			-2, 0, 2,
			-1, 0, 1
		};
	}
}
