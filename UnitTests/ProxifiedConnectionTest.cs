/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 06.07.2015
 * Time: 8:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace book2read.UnitTests
{
	[TestFixture]
	public class ProxifiedConnectionTest
	{
		[Test]
		public void TestMethod()
		{
			var target = new ProxifiedConnection();
			Assert.IsTrue(target.DownloadFile(@"http://flibustahezeous3.onion/b/165506/fb2"));
		}
	}
}
