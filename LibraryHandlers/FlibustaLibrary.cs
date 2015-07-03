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
		private IFileSystemWrapper _fsw;
		
		public FlibustaLibrary(IFileSystemWrapper fsw) {
			_fsw = fsw;
		}

		#region ILibraryHandler implementation

		public int getDaysAfterLastUpdate() {
			return _fsw.getAge();
		}

		public int getBookCount() {
			return _fsw.getLinesCount();
		}
		
		public void updateLibrary() {
			// TODO: Заменить реализацию на реальное скачивание и распаковку каталога
			//_fsw.updateFile();
			
			using (var tmp = new TempFile()) {
				Catalog.DownloadFromUrl(@"http://flibustahezeous3.onion/catalog/catalog.zip", tmp);
				
				/* Download catalog
			 	* Unzip catalog
			 	* Clean up catalog
			 	*/
			}  
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
	
		static class Catalog {
			public static void DownloadFromUrl(string url, TempFile tmp) {
				using (WebClient webClient = new WebClient()) {
					webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((object sender, DownloadProgressChangedEventArgs e) => Console.WriteLine("Downloaded:" + e.ProgressPercentage.ToString()));
 
					webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
					                                                                                        	if(e.Error != null) {
					                                                                                        		Console.WriteLine(e.Error.Message);
					                                                                                        	}
					                                                                                        	
					if (e.Error == null && !e.Cancelled) {
							Console.WriteLine("Download completed!");
							Console.WriteLine("File path: {0}", tmp.Path);
							Console.ReadLine();
						}
					});
					webClient.DownloadFileAsync(new Uri(url), tmp.Path);
				}				
			}
		}
	}
}
