/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 23.06.2015
 * Time: 13:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using book2read.LibraryHandlers;

namespace book2read.Test
{
	[TestFixture]
	public class LocalLibraryTest {
		[Test]
		public void shouldBeEmptyUponCreation() {
			var target = new LocalLibrary();
			Assert.AreEqual(0, target.getBookCount(), "Why are there any books?");
		}
		
		[Test]
		public void shouldNotBeEmptyAfterUpdate() {
			var target = new LocalLibrary(@"C:\Temp\Library");
			target.updateLibrary();
			Assert.AreNotEqual(0, target.getBookCount(), "Why there aren't any books?");
		}
		
		[Test]
		public void shouldBeAvailable() {
			var target = new LocalLibrary(@"C:\Temp\Library");
			Assert.IsTrue(target.isAvailable());
		}
	}
}
