using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Effect
	{
		public Effect(ApplyFunction apply, int width, int height)
		{
			this.Width = width;
			this.Height = height;
			this._Apply = apply ?? throw new ArgumentNullException(nameof(apply));
		}

		public readonly int Width;
		public readonly int Height;

		public void SetSize(int bytesPerPixel, int stride, int[] operationMatrix)
		{
			this.BytesPerPixel = bytesPerPixel;
			this.Stride = stride;
			this.OperationMatrix = operationMatrix;
		}

		public void Apply(
			IntPtr readPointer, IntPtr writePointer, params object[] other
		) => _Apply(readPointer, writePointer, BytesPerPixel, Stride, OperationMatrix, other);

		public delegate void ApplyFunction(
			IntPtr readPointer, 
			IntPtr writePointer, 
			int xOffset, 
			int yOffset, 
			int[] operationMatrix, 
			params object[] other
		);

		private int BytesPerPixel;
		private int Stride;
		private int[] OperationMatrix;

		private ApplyFunction _Apply;
	}
}
