using System;
using System.Drawing;

namespace Models
{
	public abstract class EffectBase : IEffect
	{
		protected EffectBase(Size readBlock, Size writeBlock)
		{
			this.ReadBlock = readBlock;
			this.WriteBlock = writeBlock;
		}

		public Size MaxSize =>
			new Size(
				this.ReadBlock.Width > this.WriteBlock.Width ? this.ReadBlock.Width : this.WriteBlock.Width,
				this.ReadBlock.Height > this.WriteBlock.Height ? this.ReadBlock.Height : this.WriteBlock.Height
			);

		public Size ReadBlock { get; }
		public Size WriteBlock { get; }

		public abstract void Apply(IntPtr readPointer, IntPtr writePointer, params object[] other);

		public abstract void SetSize(params int[][] matrices);
	}
}
