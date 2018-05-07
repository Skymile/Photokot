using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			Models.Picture picture = new Models.Picture();

			Assert.IsTrue(picture.IsEqual(picture));
		}
	}
}
