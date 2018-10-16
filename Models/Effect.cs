using System;
using System.Drawing;

namespace Models
{
	public class Effect : EffectBase
	{
		public Effect(ApplySimpler apply, Size? readBlock = null, Size? writeBlock = null) :
			this(Wrap(apply), readBlock, writeBlock)
		{ }

		public Effect(ApplyFunction apply, Size? readBlock = null, Size? writeBlock = null) :
			base(readBlock ?? new Size(1, 1), writeBlock ?? new Size(1, 1)) =>
			this.apply = apply ?? throw new ArgumentNullException(nameof(apply));

		public override void SetSize(params int[][] matrices)
		{
			this.readMatrix = matrices[0] ?? OperationMatrix.Identity;
			this.writeMatrix = matrices[1] ?? OperationMatrix.Identity;
		}

		public override void Apply(
			IntPtr readPointer, IntPtr writePointer, params object[] other
		) => this.apply(readPointer, writePointer, this.readMatrix, this.writeMatrix, other);

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

		private readonly ApplyFunction apply;
	}
}
