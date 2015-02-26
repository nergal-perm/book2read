/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 26.02.2015
 * Time: 14:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;
using book2read.Utilities;

namespace book2read.Tests {
	[TestFixture]
	public class TestFileSystemService {
		private FileSystemService _fss;
		
		[Test]
		public void TestFindFolders() {
			string _libraryPath;
			string _toReadPath;
			string _bookDbPath;
			

			var t = new List<string>(LoggingService.Instance.GetWholeLog());
			#if WORK
			_libraryPath = @"D:\Library\";
			_toReadPath = @"D:\Temp\ToRead\";
			_bookDbPath = @"D:\Temp\BookDb\";
			#else
			_libraryPath = @"C:\Temp";
			_toReadPath = @"C:\Temp";
			_bookDbPath = @"C:\Temp";
			#endif
			
			
			Assert.IsTrue(t.Contains(@"INFO: Library path set as " + _libraryPath));
			Assert.IsTrue(t.Contains(@"INFO: ToRead path set as " + _toReadPath));
			Assert.IsTrue(t.Contains(@"INFO: Book database path set as " + _bookDbPath));
		}
		
		[Test]
		public void TestCheckFiles() {
			var t = new List<string>(LoggingService.Instance.GetWholeLog());
			
			Assert.IsTrue(t.Contains("INFO: Файл \"HaveRead.txt\" найден в папке D:\\Temp\\BookDb\\"), "Файл HaveRead.txt уже существует");
		}
		
		[TestFixtureSetUp]
		public void Init() {
			_fss = FileSystemService.Instance;
		}
		
		[TestFixtureTearDown]
		public void Dispose() {
			Console.Write(String.Join("\r\n", LoggingService.Instance.GetWholeLog()));
			_fss = null;
		}
	}
}
