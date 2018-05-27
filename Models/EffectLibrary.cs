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

		public static Effect Pixelize(int width, int height) =>
			new Effect((read, write, x, y, offsets, _) =>
				{
					byte* r = (byte*)read.ToPointer();
					byte* w = (byte*)write.ToPointer();

					foreach (var o in offsets)
						w[o] = w[o + 1] = w[o + 2] =
							(byte)((r[o] + r[o + 1] + r[o + 2]) / 3);
					
					for (int i = 0; i < 3; i++)
					{
						int sum = 0;
						foreach (var o in offsets)
							sum += r[o + i];
						sum /= offsets.Length;
						byte converted = (byte)sum;
						foreach (var o in offsets)
							w[o + i] = converted;
					}

				}, width, height);

		public static Effect Convolution(int width, int height) =>
			new Effect((read, write, x, y, offsets, parameters) =>
			{
				int[] matrix = (int[])parameters[0];

				byte* r = (byte*)read.ToPointer();
				byte* w = (byte*)write.ToPointer();

				for (int j = 0; j < 3; j++)
				{
					int sum = 0;
					for (int i = 0; i < offsets.Length; i++)
						sum += r[offsets[i]+j] * matrix[i];
					sum /= offsets.Length;

					w[j] = sum > 255 ? Byte.MaxValue : sum < 0 ? Byte.MinValue : (byte)sum;
				}

			}, width, height);
	}
}
