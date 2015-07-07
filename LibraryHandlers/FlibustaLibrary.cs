/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 29.06.2015
 * Time: 14:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using System.Net;
using book2read.Utilities;
using book2read.Model;

namespace book2read.LibraryHandlers {
	/// <summary>
	/// Класс предназначен для хранения информации о каталоге книг Флибусты
	/// Поскольку данные каталога хранятся в локальном файле, класс "завязан" на 
	/// дисковые операции - нужен объект-обертка для FileSystem
	/// </summary>
	public class FlibustaLibrary : ILibraryHandler {
		private IFileSystemWrapper _fileSystem;
		
		public FlibustaLibrary(IFileSystemWrapper fsw) {
			_fileSystem = fsw;
		}

		#region ILibraryHandler implementation

		public int getDaysAfterLastUpdate() {
			return _fileSystem.getAge();
		}

		public int getBookCount() {
			return _fileSystem.getLinesCount();
		}
		
		public void updateLibrary() {
			_fileSystem.updateFile();

		}

		public bool isAvailable() {
			return _fileSystem.fileExists();
		}

		public BookInfo[] getRandomBooks(int quantity) {
			throw new NotImplementedException();
		}

		public void deleteFromLibrary(BookInfo bookToDelete) {
			throw new NotImplementedException();
		}

		public void markAsRead(BookInfo readBook) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
