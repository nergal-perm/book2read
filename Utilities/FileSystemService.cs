/*	
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 26.02.2015
 * Time: 14:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace book2read.Utilities {
	/// <summary>
	/// Класс хранит все пути к нужным файлам и папкам, отвечает за
	/// осуществление всех файловых операций чтения / записи. Реализован 
	/// как "одиночка", глобально доступен из любой части приложения.
	/// </summary>
	public sealed class FileSystemService {
		
		#region Реализация "одиночки"
		private static readonly Lazy<FileSystemService> lazy =
			new Lazy<FileSystemService>(() => new FileSystemService());
		
		public static FileSystemService Instance { get { return lazy.Value; } }
		
		private FileSystemService() {
			initialize();
		}
		#endregion
		
		DirectoryInfo _libraryPath;
		public DirectoryInfo LibraryPath { get { return _libraryPath;} }
		
		DirectoryInfo _toReadPath;
		public DirectoryInfo ToReadPath { get { return _toReadPath;} }
		
		DirectoryInfo _bookDbPath;
		public DirectoryInfo BookDbPath { get { return _bookDbPath;} }
		
		FileInfo _haveRead;
		public FileInfo HaveReadFile { get { return _haveRead; } }
		
		FileInfo _toRead;
		public FileInfo ToReadFile { get { return _toRead; } }
		
		FileInfo _report;
		public FileInfo ReportFile { get { return _report; } }

		public void initialize() {
			findPaths();
			checkFiles();
			
			#region Логирование для тестирования работы
			if (_libraryPath == null || !_libraryPath.Exists) {
				LoggingService.Instance.RecordMessage("Локальная библиотека не найдена, приложение завершает работу", LoggingService.MessageType.Error);
			}
			
			LoggingService.Instance.RecordMessage("Library path set as " + _libraryPath.FullName, LoggingService.MessageType.Info);
			LoggingService.Instance.RecordMessage("ToRead path set as " + _toReadPath.FullName, LoggingService.MessageType.Info);
			LoggingService.Instance.RecordMessage("Book database path set as " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			#endregion
		}

		public bool isLibraryFileTooOld() {
			return (DateTime.Now - _toRead.CreationTime).Days > 30;
		}

		public bool isLibraryFound() {
			return _libraryPath.Exists;
		}
		public long getBookCount() {
			return File.ReadLines(_toRead.FullName).Count();
		}
		
		DirectoryInfo getDropboxFolder() {
			var appDataPath = Environment.GetFolderPath(
				                  Environment.SpecialFolder.ApplicationData);
			var dbPath = Path.Combine(appDataPath, "Dropbox\\host.db");

			if (!File.Exists(dbPath))
				return null;

			var lines = File.ReadAllLines(dbPath);
			var dbBase64Text = Convert.FromBase64String(lines[1]);
			var folderPath = Encoding.UTF8.GetString(dbBase64Text);

			return new DirectoryInfo(folderPath);
		}
		
		DirectoryInfo getLibraryPath() {
			foreach (var drive in DriveInfo.GetDrives()) {
				if (drive.IsReady && drive.VolumeLabel.Equals("Big Storage")) {
					return new DirectoryInfo(drive.Name + @"Library\");
				}
			}
			return null;
		}
	
		void findPaths() {
			_libraryPath = getLibraryPath() == null ? new DirectoryInfo(@"D:\Library\") : getLibraryPath();
			
			if (getDropboxFolder() == null) {
				_toReadPath = new DirectoryInfo(@"D:\Temp\ToRead\");
				_bookDbPath = new DirectoryInfo(@"D:\Temp\BookDb\");
			} else {
				_toReadPath = new DirectoryInfo(getDropboxFolder().FullName + @"\ToRead\");
				_bookDbPath = new DirectoryInfo(getDropboxFolder().FullName + @"\BookDb\");
			}			
			
			if (!_toReadPath.Exists) {
				_toReadPath.Create();
			}
			
			if (!_bookDbPath.Exists) {
				_bookDbPath.Create();
			}
			
		}
	
		void checkFiles() {
			_haveRead = new FileInfo(_bookDbPath.FullName + "HaveRead.txt");
			if (!_haveRead.Exists) {
				_haveRead.Create();
				LoggingService.Instance.RecordMessage("Создан файл \"HaveRead.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"HaveRead.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}

			_report = new FileInfo(_bookDbPath.FullName + "Report.txt");
			if (!_report.Exists) {
				_report.Create();
				LoggingService.Instance.RecordMessage("Создан файл \"Report.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"Report.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}
			
			_toRead = new FileInfo(_bookDbPath.FullName + "ToRead.txt");
			if (!_toRead.Exists) {
				_toRead.Create();
				LoggingService.Instance.RecordMessage("Создан файл \"ToRead.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"ToRead.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}			
			
		}
	
	
	}
}