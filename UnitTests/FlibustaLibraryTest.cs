/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 29.06.2015
 * Time: 14:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using NUnit.Framework;

using book2read.LibraryHandlers;
using book2read.Utilities;

namespace book2read.UnitTests {
	[TestFixture]
	public class FlibustaLibraryTest {

		[Test]
		public void shouldBeAbleToUpdate() {
			FakeFSWrapper fsw = new FakeFSWrapper();
			FlibustaLibrary target = new FlibustaLibrary(fsw);
			
			// Файл каталога недоступен, вся библиотека недоступна
			Assert.IsFalse(target.isAvailable());
			
			// Попытки получить свойства библиотеки выбрасывают исключение
			try {
				target.getBookCount();
				Assert.Fail();
			} catch (FileNotFoundException e) {
				// Do Nothing
			}
			
			try {
				target.getDaysAfterLastUpdate();
				Assert.Fail();
			} catch (FileNotFoundException e) {
				// Do Nothing
			}
			
			// После первого обновления должно стать 50 книг и 0 дней
			target.updateLibrary();
			Assert.AreEqual(50, target.getBookCount());
			Assert.AreEqual(0, target.getDaysAfterLastUpdate());

			// Делаем вид, что прочитали за 100 дней 5 книг
			fsw.setAge(100);
			fsw.setLinesCount(45);
			Assert.AreEqual(100, target.getDaysAfterLastUpdate());
			Assert.AreEqual(45, target.getBookCount());
			
			target.updateLibrary();
			// После второго обновления должно стать 45+50=95 книг и возраст = 0 дней
			Assert.AreEqual(95, target.getBookCount());
			Assert.AreEqual(0, target.getDaysAfterLastUpdate());
		}
		
		[Test]
		public void realDownloading() {
			FSWrapper fsw = new FlibustaCatalogWrapper(@"D:\Temp\BookDb\catalog.txt");
			FlibustaLibrary fl = new FlibustaLibrary(fsw);
			
			fl.updateLibrary();
		}
	}
}
