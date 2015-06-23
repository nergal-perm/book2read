/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 23.06.2015
 * Time: 13:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using book2read.LibraryHandlers;

namespace book2read.Test
{
	[TestFixture]
	public class EvernoteOfflineTest {
		[Test]
		public void shouldBeEmptyUponCreation() {
			var target = new EvernoteOfflineLibrary();
			Assert.True(target.getBookCount() == 0, "Why are there any books?");
		}
	}
}
