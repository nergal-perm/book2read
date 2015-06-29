/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 29.06.2015
 * Time: 14:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

using book2read.Utilities;
using book2read.Model;

namespace book2read.LibraryHandlers
{
	/// <summary>
	/// Класс предназначен для хранения информации о каталоге книг Флибусты
	/// Поскольку данные каталога хранятся в локальном файле, класс "завязан" на 
	/// дисковые операции - нужен объект-обертка для FileSystem
	/// </summary>
	public class FlibustaLibrary : ILibraryHandler {
		private IFileSystemWrapper _fsw;
		
		public FlibustaLibrary(IFileSystemWrapper fsw) {
			_fsw = fsw;
		}

		#region ILibraryHandler implementation

		public int getDaysAfterLastUpdate() {
			if (_fsw.fileExists()) {
				return _fsw.getAge();
			}
			throw new FileNotFoundException("Файл каталога не найден");
		}

		public int getBookCount() {
			if (_fsw.fileExists()) {
				return _fsw.getLinesCount();
			}
			throw new FileNotFoundException("Файл каталога не найден");
		}		
		
		public void updateLibrary() {
			_fsw.updateFile();
		}

		public bool isAvailable() {
			return _fsw.fileExists();
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
