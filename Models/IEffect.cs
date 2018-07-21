using System;
using System.Drawing;

namespace Models
{
	public interface IEffect
	{
		Size ReadBlock { get; }
		Size WriteBlock { get; }
		Size MaxSize { get; }

		void Apply(IntPtr readPointer, IntPtr writePointer, params object[] other);

		void SetSize(params int[][] matrices);
	}
}
