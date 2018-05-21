using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public unsafe static class EffectLibrary
	{
		public readonly static Effect BlackWhite =
			new Effect(
				(read, write, x, y, o, _) =>
				{
					byte* r = (byte*)read.ToPointer();
					byte* w = (byte*)write.ToPointer();

					w[0] = w[1] = w[2] = (byte)((r[0] + r[1] + r[2]) / 3);
				}, 1, 1);
	}
}
