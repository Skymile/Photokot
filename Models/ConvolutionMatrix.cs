using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public static class ConvolutionMatrix
	{
		public static int[] BoxFilter(int width, int height)
		{
			var matrix = new int[width * height];
			for (int i = 0; i < matrix.Length; i++)
				matrix[i] = 1;
			return matrix;
		}
	}
}
