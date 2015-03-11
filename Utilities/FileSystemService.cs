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
using EvernoteSDK;

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
			Console.WriteLine("Connecting to Evernote...");
			Console.ReadLine();
			try {
				ENSession.SetSharedSessionDeveloperToken("",
				                                         "");
				if (!ENSession.SharedSession.IsAuthenticated) {
					ENSession.SharedSession.AuthenticateToEvernote();
					Console.WriteLine("Got Note");
					Console.ReadLine();
				}				
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.ReadLine();
			} finally {
				Console.WriteLine("What the hell?");
				Console.ReadLine();
			}
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
			} 
			_report = new FileInfo(_bookDbPath.FullName + "Report.txt");
			if (!_report.Exists) {
				_report.Create().Close();
			} 
			
			_toReadLocal = new FileInfo(_bookDbPath.FullName + "ToReadLocal.txt");
			if (!_toReadLocal.Exists) {
				_toReadLocal.Create().Close();
			} 

			_toReadWeb = new FileInfo(_bookDbPath.FullName + "ToReadWeb.txt");
			if (!_toReadWeb.Exists) {
				_toReadWeb.Create().Close();
			} 
			
			_haveReadWebIds = new FileInfo(_bookDbPath.FullName + "WebIds.txt");
			if (!_haveReadWebIds.Exists) {
				_haveReadWebIds.Create().Close();
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
			var timeStart = DateTime.Now;
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
			UserInterface.confirmUpdateOperation(DateTime.Now - timeStart);
		}

		void updateLocalLibrary() {
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
				UserInterface.ShowPercentProgress("Обновляю библиотеку...", i, allFiles.Count);
				i++;
			}
			System.IO.File.WriteAllLines(_toReadLocal.FullName, files.Select(p => p.Name).ToArray(), Encoding.UTF8);
			
		}

		/// <summary>
		/// Подключается к облачному хранилищу Google Drive и получает списки файлов
		/// пачками по MaxResults штук
		/// </summary>
		/// <returns>Список всех доступных файлов в облачном хранилище</returns>
		private List<Google.Apis.Drive.v2.Data.File> getFileListFromOnlineLibrary() {
			FilesResource.ListRequest request = _webService.Files.List();
			request.Q = "mimeType != 'application/vnd.google-apps.folder'";
			request.MaxResults = 1000;
			var result = new List<Google.Apis.Drive.v2.Data.File>();

			int i = 0;
			do {
				try {
					UserInterface.updateLongOperationStatus(i);
					FileList allFiles = request.Execute();
					result.AddRange(allFiles.Items);
					request.PageToken = allFiles.NextPageToken;
					i++;
				} catch (Exception e) {
					Console.WriteLine(" Ошибка!");
					request.PageToken = null;
					_isWebLibraryAvailable = false;
				}
			} while (!String.IsNullOrEmpty(request.PageToken));		

			return result;
		}
		
		/// <summary>
		/// Получает из всего списка доступных в облаке файлов файлы с книгами
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private List<Google.Apis.Drive.v2.Data.File>filterOnlineFilesList(List<Google.Apis.Drive.v2.Data.File> result) {
			int i = 0;
			var files = new List<Google.Apis.Drive.v2.Data.File>();
			var ids = System.IO.File.ReadAllLines(_haveReadWebIds.FullName).ToList();
			foreach (var file in result) {
				if (!ids.Contains(file.Id) && (file.Title.ToUpper().Contains(".PDF") || file.Title.ToUpper().Contains(".ZIP") ||
				    file.Title.ToUpper().Contains(".RAR") || file.Title.ToUpper().Contains(".FB2") ||
				    file.Title.ToUpper().Contains(".DJV") || file.Title.ToUpper().Contains(".DOC") ||
				    file.Title.ToUpper().Contains(".TXT"))) {
					files.Add(file);
				}
				UserInterface.ShowPercentProgress("Обновляю библиотеку...", i, result.Count);
				i++;
			}
			
			return files;
		}

		/// <summary>
		/// Записывает данные о найденных в облаке книгах в файл БД
		/// </summary>
		/// <param name="list"></param>
		void updateWebLibraryFile(List<Google.Apis.Drive.v2.Data.File> list) {
			System.IO.File.WriteAllLines(_toReadWeb.FullName, list.Select(p => p.Id + ": " + p.OriginalFilename).ToArray(), Encoding.UTF8);
		}
		
		void updateWebLibrary() {
			UserInterface.showMessage("Получаю список файлов сетевой библиотеки... ");
			
			var result = getFileListFromOnlineLibrary();
			if (result.Count == 0) {
				return;
			}
			
			UserInterface.showMessage("готово!" + Environment.NewLine);
			
			updateWebLibraryFile(filterOnlineFilesList(result));
			
			
		}

		public FileInfo[] getCurrentQueue() {
			FileInfo[] files = _toReadPath.GetFiles().ToArray();
			Array.Sort(files, new FileComparer());
			return files;
			
		}

		public void archiveBook(BookMetaData bookInfo) {
			var gdPath = Environment.GetFolderPath(
				Environment.SpecialFolder.UserProfile) + @"\Google Диск\Book Archive\";
			System.IO.File.Copy(bookInfo.file.FullName, gdPath + bookInfo.dbRow.Split("[".ToCharArray())[0].Trim(" ".ToCharArray()) + bookInfo.file.Extension);
		}

		/// <summary>
		/// Удаляет файл из локальной библиотеки
		/// </summary>
		/// <param name="curFile">Файл для удаления</param>
		public void removeFromLibrary(FileInfo curFile) {
			if (!isLibraryFound()) { return; }
			foreach (var file in _libraryPath.GetFiles(curFile.Name, SearchOption.AllDirectories)) {
				file.Delete();
			}			
		}
		
		/// <summary>
		/// Удаляет файл из списка чтения
		/// </summary>
		/// <param name="curFile">Файл для удаления</param>
		public void removeFromQueue(FileInfo curFile) {
			curFile.Delete();			
		}

		/// <summary>
		/// Записывает информацию о книге в читательский дневник. Для "облачных" файлов регистрирует
		/// ID в списке прочитанных, чтобы не предлагать этот файл в будущем
		/// </summary>
		/// <param name="bookInfo">Структура: строка для записи в дневник и ссылка на файл книги</param>
		public void appendBookInfoToReadLog(BookMetaData bookInfo) {
			System.IO.File.AppendAllText(_haveRead.FullName, bookInfo.dbRow + Environment.NewLine);
			appendIdToWebIds(bookInfo.file);
		}
		
		public void appendIdToWebIds(FileInfo file) {
			// если файл был получен из облачной библиотеки, сохраним его ID в список ID прочитанных книг,
			// чтобы не предлагать его к прочтению в будущем
			var result = System.IO.File.ReadAllLines(_toReadWeb.FullName).FirstOrDefault(s => s.Contains(file.Name));
			if (result != null) {
				var id = result.Split(":".ToCharArray())[0];
				System.IO.File.AppendAllText(_haveReadWebIds.FullName, id + Environment.NewLine);				
			}			
		}
		
		/// <summary>
		/// Возвращает указанный по индексу файл из очереди чтения
		/// </summary>
		/// <param name="bookIndex">Индекс возвращаемого файла</param>
		/// <returns>FileInfo: файл с книгой</returns>
		public FileInfo getBookFromQueue(int bookIndex) {
			return getCurrentQueue()[bookIndex];
		}

		public void getBookFromLocalLibrary(int element) {
			string fileName = System.IO.File.ReadLines(_toReadLocal.FullName).Skip(element).First();
			FileInfo fileToRead = _libraryPath.GetFiles(fileName, SearchOption.AllDirectories)[0];
			fileToRead.CopyTo(_toReadPath.FullName + fileName);
		}

		public void getBookFromOnlineLibrary(int element) {
			string fileId = System.IO.File.ReadLines(_toReadWeb.FullName).Skip(element).First().Split(":".ToCharArray())[0];
			Google.Apis.Drive.v2.Data.File file = _webService.Files.Get(fileId).Execute();
			downloadFile(_webService, file, _toReadPath + file.Title);
		}
		
		
		public static Boolean downloadFile(DriveService _service, Google.Apis.Drive.v2.Data.File _fileResource, string _saveTo) {

			if (!String.IsNullOrEmpty(_fileResource.DownloadUrl)) {
				try {
					var x = _service.HttpClient.GetByteArrayAsync(_fileResource.DownloadUrl);
					byte[] arrBytes = x.Result;
					System.IO.File.WriteAllBytes(_saveTo, arrBytes);
					return true;                  
				} catch (Exception e) {
					Console.WriteLine("An error occurred: " + e.Message);
					return false;
				}
			} else {
				// The file doesn't have any content stored on Drive.
				return false;
			}
		}

		public string[] getWholeReadingLog() {
			return System.IO.File.ReadAllLines(_haveRead.FullName);
		}
	}
}