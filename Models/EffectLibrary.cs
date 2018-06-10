using System;
using System.Collections.Generic;
using System.Drawing;

namespace Models
{
	public unsafe static class EffectLibrary
	{
		public static Effect BlackWhite() =>
			new Effect(
				(read, write, ___, __, _) =>
				{
					byte* r = (byte*)read.ToPointer();
					byte* w = (byte*)write.ToPointer();

					w[0] = w[1] = w[2] = (byte)((r[0] + r[1] + r[2]) / 3);
				}, new Size(1, 1), new Size(1, 1)
			);

		public static Effect Pixelize(Size readWriteBlock) =>
			new Effect((read, write, rBlock, wBlock, _) =>
				{
					byte* r = (byte*)read.ToPointer();
					byte* w = (byte*)write.ToPointer();

					foreach (var o in rBlock)
						w[o] = w[o + 1] = w[o + 2] =
							(byte)((r[o] + r[o + 1] + r[o + 2]) / 3);

					for (int i = 0; i < 3; i++)
					{
						int sum = 0;
						foreach (var o in rBlock)
							sum += r[o + i];
						sum /= rBlock.Length;
						byte converted = (byte)sum;
						foreach (var o in wBlock)
							w[o + i] = converted;
					}

				}, readWriteBlock, readWriteBlock);

		public static Effect Convolution(Size readBlock) =>
			new Effect((read, write, rBlock, wBlock, parameters) =>
			{
				int[] matrix = (int[])parameters[0];

				byte* r = (byte*)read.ToPointer();
				byte* w = (byte*)write.ToPointer();

				for (int j = 0; j < 3; j++)
				{
					int sum = 0;
					for (int i = 0; i < rBlock.Length; i++)
						sum += r[rBlock[i] + j] * matrix[i];
					sum /= rBlock.Length;

					w[j] = sum > 255 ? Byte.MaxValue : sum < 0 ? Byte.MinValue : (byte)sum;
				}

			}, readBlock, new Size(1, 1));

		public static Effect Median(Size readBlock) =>
			new Effect((read, write, rBlock, wBlock, _) =>
			{
				byte* r = (byte*)read.ToPointer();
				byte* w = (byte*)write.ToPointer();

				List<byte> pixels = new List<byte>();
				for (int i = 0; i < 9; i++)
					pixels.Add(0);

				for (int j = 0; j < 3; j++)
				{
					for (int i = 0; i < rBlock.Length; i++)
						pixels[i] = r[rBlock[i] + j];
					pixels.Sort();
					w[j] = pixels[pixels.Count / 2];
				}
			}, readBlock, new Size(1, 1));
	}
}
