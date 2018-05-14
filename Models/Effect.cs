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
			if (apply == null)
				throw new ArgumentNullException(nameof(apply));

			this.Width = width;
			this.Height = height;
			this.Apply = apply;
		}

		public readonly int Width;
		public readonly int Height;

		public delegate void ApplyFunction(IntPtr readPointer, IntPtr writePointer, int xOffset, int yOffset);

		public ApplyFunction Apply;
	}
}
