using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class PictureTests
	{
		[TestMethod]
		public void EqualityTest()
		{
			Models.Picture pictureOne = new Models.Picture("apple.png");
			Models.Picture pictureTwo = new Models.Picture("apple.png");
			Models.Picture pictureThree = new Models.Picture("apple2.png");

			Assert.IsTrue(pictureOne == pictureTwo);
		}
	}
}
