using System;
using System.Drawing;

namespace Models
{
	public class Effect
	{
		public Effect(ApplySimpler apply, Size? readBlock = null, Size? writeBlock = null) :
			this(Wrap(apply), readBlock, writeBlock)
		{ }

		public Effect(ApplyFunction apply, Size? readBlock = null, Size? writeBlock = null)
		{
			this._Apply     = apply      ?? throw new ArgumentNullException(nameof(apply));
			this.ReadBlock  = readBlock  ?? new Size(1, 1);
			this.WriteBlock = writeBlock ?? new Size(1, 1);
		}

		public readonly Size ReadBlock;
		public readonly Size WriteBlock;

		public void SetSize(int[] readMatrix, int[] writeMatrix = null)
		{
			this.readMatrix = readMatrix ?? OperationMatrix.Identity;
			this.writeMatrix = writeMatrix ?? OperationMatrix.Identity;
		}

		public Size MaxSize => 
			new Size(
				ReadBlock.Width  > WriteBlock.Width  ? ReadBlock.Width  : WriteBlock.Width,
				ReadBlock.Height > WriteBlock.Height ? ReadBlock.Height : WriteBlock.Height
			);

		public void Apply(
			IntPtr readPointer, IntPtr writePointer, params object[] other
		) => _Apply(readPointer, writePointer, readMatrix, writeMatrix, other);

		public delegate void ApplyFunction(
			IntPtr readPointer, 
			IntPtr writePointer,
			int[] readMatrix,
			int[] writeMatrix,
			params object[] other
		);

		public delegate void ApplySimpler(
			IntPtr readPointer,
			IntPtr writePointer
		);

		private static ApplyFunction Wrap(ApplySimpler apply) =>
			(readPointer, writePointer, readMatrix, writeMatrix, other) => apply(readPointer, writePointer);

		private int[] readMatrix;
		private int[] writeMatrix;

		private readonly ApplyFunction _Apply;
	}
}
