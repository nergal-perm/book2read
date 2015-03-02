/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 26.02.2015
 * Time: 14:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;

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
		public DirectoryInfo LibraryPath { get { return _libraryPath; } }
		
		DirectoryInfo _toReadPath;
		public DirectoryInfo ToReadPath { get { return _toReadPath; } }
		
		DirectoryInfo _bookDbPath;
		public DirectoryInfo BookDbPath { get { return _bookDbPath; } }
		
		FileInfo _haveRead;
		public FileInfo HaveReadFile { get { return _haveRead; } }
		
		FileInfo _toReadLocal;
		public FileInfo ToReadLocalFile { get { return _toReadLocal; } }
		
		FileInfo _toReadWeb;
		public FileInfo ToReadWebFile { get { return _toReadWeb; } }
		
		FileInfo _report;
		public FileInfo ReportFile { get { return _report; } }
		
		FileInfo _haveReadWebIds;
		public FileInfo HaveReadWebIds { get { return _haveReadWebIds; } }

		bool _isWebLibraryAvailable;
		DriveService _webService;
		public DriveService WebService { get { return _webService; } }
		
		public void initialize() {
			findPaths();
			checkAndCreateFiles();
			try {
				authorizeInWebLibrary();
				_isWebLibraryAvailable = true;
			} catch (Exception e) {
				_isWebLibraryAvailable = false;
			}
			
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
			_toReadLocal.Refresh();
			return (DateTime.Now - _toReadLocal.LastWriteTime).Days > 30;
		}

		public bool isLibraryFound() {
			return _libraryPath.Exists;
		}
		
		public long getBookCount() {
			return LocalBoksCount + WebBooksCount;
		}
		
		public int LocalBoksCount { get { return System.IO.File.ReadLines(_toReadLocal.FullName).Count(); } }
		public int WebBooksCount { get { return System.IO.File.ReadLines(_toReadWeb.FullName).Count(); } }
		
		public bool isWebLibraryAvailable() {
			return _isWebLibraryAvailable;
		}
		
		void findPaths() {
			_libraryPath = getLibraryPath() ?? new DirectoryInfo(@"D:\Library\");
			
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

		DirectoryInfo getDropboxFolder() {
			var appDataPath = Environment.GetFolderPath(
				                  Environment.SpecialFolder.ApplicationData);
			var dbPath = Path.Combine(appDataPath, "Dropbox\\host.db");

			if (!System.IO.File.Exists(dbPath))
				return null;

			var lines = System.IO.File.ReadAllLines(dbPath);
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
		
		void checkAndCreateFiles() {
			_haveRead = new FileInfo(_bookDbPath.FullName + "HaveRead.txt");
			if (!_haveRead.Exists) {
				_haveRead.Create().Close();
				LoggingService.Instance.RecordMessage("Создан файл \"HaveRead.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"HaveRead.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}

			_report = new FileInfo(_bookDbPath.FullName + "Report.txt");
			if (!_report.Exists) {
				_report.Create().Close();
				LoggingService.Instance.RecordMessage("Создан файл \"Report.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"Report.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}
			
			_toReadLocal = new FileInfo(_bookDbPath.FullName + "ToReadLocal.txt");
			if (!_toReadLocal.Exists) {
				_toReadLocal.Create().Close();
				LoggingService.Instance.RecordMessage("Создан файл \"ToReadLocal.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"ToReadLocal.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}

			_toReadWeb = new FileInfo(_bookDbPath.FullName + "ToReadWeb.txt");
			if (!_toReadWeb.Exists) {
				_toReadWeb.Create().Close();
				LoggingService.Instance.RecordMessage("Создан файл \"ToReadWeb.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"ToReadWeb.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}
			
			_haveReadWebIds = new FileInfo(_bookDbPath.FullName + "WebIds.txt");
			if (!_haveReadWebIds.Exists) {
				_haveReadWebIds.Create().Close();
				LoggingService.Instance.RecordMessage("Создан файл \"WebIds.txt\"", LoggingService.MessageType.System);
			} else {
				LoggingService.Instance.RecordMessage("Файл \"WebIds.txt\" найден в папке " + _bookDbPath.FullName, LoggingService.MessageType.Info);
			}			
		}
		
		
		void authorizeInWebLibrary() {
			Console.WriteLine("Авторизация в онлайн-библиотеке...");
			Console.WriteLine("==========================");

			const String serviceAccountEmail = "1068110686945-0tr1r5qqleo8a34slqq17k5cb44nd2l2@developer.gserviceaccount.com";
			
			var certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);

			var credential = new ServiceAccountCredential(
				                 new ServiceAccountCredential.Initializer(serviceAccountEmail) {
					Scopes = new[] {
						DriveService.Scope.Drive,
						DriveService.Scope.DriveFile,
						DriveService.Scope.DriveReadonly,
						DriveService.Scope.DriveMetadataReadonly
					}
				}.FromCertificate(certificate));

			// Create the service.
			_webService = new DriveService(new BaseClientService.Initializer() {
				HttpClientInitializer = credential,
				ApplicationName = "Web Library Crawler",
			});
			

			Console.Clear();
		}

		public void updateLibrary() {
			if (!isLibraryFound()) {
				Console.WriteLine("Локальная библиотека не найдена, список локальных книг не будет обновлен.");
			} else {
				updateLocalLibrary();
			}
			
			if (!isWebLibraryAvailable()) {
				Console.WriteLine("Сетевая библиотека недоступна, список книг в облаке не будет обновлен.");
			} else {
				updateWebLibrary();
			}
		}

		void updateLocalLibrary() {
			var timeStart = DateTime.Now;
			Console.Write("Получаю список файлов локальной библиотеки... ");
			var allFiles = _libraryPath.GetFiles("*.*", SearchOption.AllDirectories).ToList(); //.Select(p => p.Name).ToArray();
			var files = new List<FileInfo>();
			Console.WriteLine("готово!");
			int i = 0;
			foreach (var file in allFiles) {
				if (file.Name.ToUpper().Contains(".PDF") || file.Name.ToUpper().Contains(".ZIP") || file.Name.ToUpper().Contains(".RAR") ||
				    file.Name.ToUpper().Contains(".FB2") || file.Name.ToUpper().Contains(".DJV") || file.Name.ToUpper().Contains(".DOC") ||
				    file.Name.ToUpper().Contains(".TXT")) {
					files.Add(file);
				}
				Program.ShowPercentProgress("Обновляю библиотеку...", i, allFiles.Count);
				i++;
			}
			System.IO.File.WriteAllLines(_toReadLocal.FullName, files.Select(p => p.Name).ToArray(), Encoding.UTF8);
			
		}

		void updateWebLibrary() {
			Console.Write("Получаю список файлов сетевой библиотеки... ");
			FilesResource.ListRequest request = _webService.Files.List();
			request.Q = "mimeType != 'application/vnd.google-apps.folder'";
			request.MaxResults = 1000;
			var result = new List<Google.Apis.Drive.v2.Data.File>();

			int i = 0;
			do {
				try {
					FileList allFiles = request.Execute();
			        result.AddRange(allFiles.Items);
        			request.PageToken = allFiles.NextPageToken;
        			i++;
      			} catch (Exception e) {
        			Console.WriteLine("An error occurred: " + e.Message);
        			request.PageToken = null;
      			}
    		} while (!String.IsNullOrEmpty(request.PageToken));
			Console.WriteLine("готово!");

			i = 0;
			var files = new List<Google.Apis.Drive.v2.Data.File>();
			var ids = System.IO.File.ReadAllLines(_haveReadWebIds.FullName).ToList();
			foreach (var file in result) {
				if (!ids.Contains(file.Id) && (file.Title.ToUpper().Contains(".PDF") || file.Title.ToUpper().Contains(".ZIP") ||
				    file.Title.ToUpper().Contains(".RAR") || file.Title.ToUpper().Contains(".FB2") || 
				    file.Title.ToUpper().Contains(".DJV") || file.Title.ToUpper().Contains(".DOC") ||
				    file.Title.ToUpper().Contains(".TXT"))) {
					files.Add(file);
				}
				Program.ShowPercentProgress("Обновляю библиотеку...", i, result.Count);
				i++;
			}
			System.IO.File.WriteAllLines(_toReadWeb.FullName, files.Select(p => p.Id + ": " + p.OriginalFilename).ToArray(), Encoding.UTF8);
			Console.Read();
		}

		public string[] getCurrentQueue() {
			string[] files = _toReadPath.GetFiles().Select(p => p.Name).ToArray();
			Array.Sort(files);
			return files;
			
		}

		public void archiveBook(FileInfo curFile) {
			//TODO: Not yet implemented
		}

		public void removeBook(FileInfo curFile) {
			foreach (var file in _libraryPath.GetFiles(curFile.Name, SearchOption.AllDirectories)) {
				file.Delete();
			}
			curFile.Delete();
		}
	}
}