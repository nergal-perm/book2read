/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 25.02.2015
 * Time: 14:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Drive.v2;
using book2read.Utilities;
using Google.Apis.Auth;

namespace book2read {
	class Program {

		
		public static void Main(string[] args) {   
			
			bool exitCommand = false;
			while (!exitCommand) {
				Console.Clear();
				initAndWelcome();	
				Console.WriteLine("Введите команду:");
				string[] command = Console.ReadLine().Split(" ".ToCharArray());
				switch (command[0]) {
					case "exit":
						exitCommand = true;
						break;
					case "read":
						// mark book as read and move it to archive
						archiveBook(command);
						break;
					case "delete":
						deleteBook(command);
						break;
					case "update":
						FileSystemService.Instance.updateLibrary();
						break;
					case "get":
						getBooksToRead(command);
						break;
					case "clear":
						break;
					default:
						// show help screen
						Console.WriteLine("Here goes some help");
						break;
				}
			}
			
			// После получения команды exit организовать выход из консоли
			return;
		}

		static void SampleCode() {
			// Get n-th line number from file
			const string FileName = @"D:\Temp\lines1-10.txt";
			string line = File.ReadLines(FileName).Skip(5).First();
			long line_count = File.ReadLines(FileName).Count();
			Console.WriteLine("Found {1} lines, here is line 6: {0}", line, line_count);
			
			// Show progress percent count
			for (int i = 0; i < 100; i++) {
				ShowPercentProgress("Processing...", i, 100);   
				System.Threading.Thread.Sleep(100);   
			}   
		}
		
		public static void ShowPercentProgress(string message, int currElementIndex, int totalElementCount) {
			if (currElementIndex < 0 || currElementIndex >= totalElementCount) {
				throw new InvalidOperationException("currElement out of range");
			}
			int percent = (100 * (currElementIndex + 1)) / totalElementCount;
			Console.Write("\r{0}{1}% complete", message, percent);
			if (currElementIndex == totalElementCount - 1) {
				Console.WriteLine(Environment.NewLine);
			}
		}

		static void initAndWelcome() {
			FileSystemService f = FileSystemService.Instance;
			Console.WriteLine("Привет, читатель!");
			Console.WriteLine();
			Console.WriteLine("Твоя база данных по книгам обновлялась " +
			(f.isLibraryFileTooOld() ? "больше" : "меньше") +
			" месяца назад.");
			Console.WriteLine("В ней сейчас содержится " + f.getBookCount() + " записей о книгах.");
			Console.WriteLine();
			Console.WriteLine("Твоя локальная библиотека " +
			(f.isLibraryFound() ? "найдена в папке " + f.LibraryPath.FullName : "не найдена."));
			Console.WriteLine("Сетевая библиотека " + (f.isWebLibraryAvailable() ? "доступна." : "недоступна"));
			Console.WriteLine();
			Console.WriteLine("В твоем списке чтения сейчас " + f.getCurrentQueue().Count() + " книг:");
			int i = 0;
			foreach (var element in f.getCurrentQueue()) {
				Console.WriteLine(++i + "\t" + element);
			}
			Console.WriteLine();
			
		}

		static void getBooksToRead(string[] command) {
			// если неверное число аргументов, выходим обратно
			if (command.Length != 2) {
				return;
			}
			var f = FileSystemService.Instance;
			int index;
			int.TryParse(command[1], out index);
			if (index <= 0) {
				return;
			}
			
			int local = f.isLibraryFound() ? f.LocalBoksCount : 0;
			int web = f.isWebLibraryAvailable() ? f.WebBooksCount : 0;
		
			if (local + web == 0) {
				return;
			}
			
			var bookNums = getRandomSequence(index, local + web);
			
			foreach (var element in bookNums) {
				if (element < local) {
					// choose book from local library
					string fileName = File.ReadLines(f.ToReadLocalFile.FullName).Skip(element).First();
					FileInfo fileToRead = f.LibraryPath.GetFiles(fileName, SearchOption.AllDirectories)[0];
					fileToRead.CopyTo(f.ToReadPath.FullName + fileName);
				} else {
					string fileId = System.IO.File.ReadLines(f.ToReadWebFile.FullName).Skip(element - local).First().Split(":".ToCharArray())[0];
					Google.Apis.Drive.v2.Data.File file = f.WebService.Files.Get(fileId).Execute();
					downloadFile(f.WebService, file, f.ToReadPath + file.Title);
				}
			}
		}
		
		
        public static Boolean downloadFile(DriveService _service, Google.Apis.Drive.v2.Data.File _fileResource, string _saveTo)
        {

            if (!String.IsNullOrEmpty(_fileResource.DownloadUrl))
            {
                try
                {
                    var x = _service.HttpClient.GetByteArrayAsync(_fileResource.DownloadUrl );
                    byte[] arrBytes = x.Result;
                    System.IO.File.WriteAllBytes(_saveTo, arrBytes);
                    return true;                  
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return false;
                }
            }
            else
            {
                // The file doesn't have any content stored on Drive.
                return false;
            }
        }
		
		static List<int> getRandomSequence(int index, int maxValue) {
			var r = new Random();
			var randomList = new List<int>();
			
			for (int i = 0; i < index; i++) {
				int MyNumber = r.Next(0, maxValue - 1);
				if (!randomList.Contains(MyNumber)) {
					randomList.Add(MyNumber);	
				}
			}
			return randomList;
		}

		static void deleteBook(string[] command) {
			// если неверное число аргументов, выходим обратно
			if (command.Length != 2) {
				return;
			}
			var f = FileSystemService.Instance;
			int index;
			int.TryParse(command[1], out index);
			index--;
			if (index < 0 || index >= f.getCurrentQueue().Length) {
				return;
			}
			var curFile = f.ToReadPath.GetFiles(f.getCurrentQueue()[index])[0];

			Console.WriteLine("Выбранный файл будет удален:");
			ColoredConsoleWrite(ConsoleColor.Red, curFile.Name + Environment.NewLine);
			Console.Write("Подтвердите операцию (нажмите [Enter]): ");
			if (Console.ReadKey().Key == ConsoleKey.Enter) {
				f.removeBook(curFile);
				Console.WriteLine();
			} 			
		}
		
		static void archiveBook(string[] command) {
			// если неверное число аргументов, выходим обратно
			if (command.Length != 2) {
				return;
			}
			var f = FileSystemService.Instance;
			int index;
			int.TryParse(command[1], out index);
			index--;
			if (index < 0 || index >= f.getCurrentQueue().Length) {
				return;
			}
			
			// Запрашиваем информацию о текущем файле
			var curFile = f.ToReadPath.GetFiles(f.getCurrentQueue()[index])[0];
			Console.WriteLine("Выбран файл: " + curFile.Name);
			Console.Write("Введите название книги: ");
			string title = Console.ReadLine();
			Console.Write("Введите автора книги: ");
			string author = Console.ReadLine();
			Console.Write("Введите доп.информацию [*^+@]: ");
			string info = Console.ReadLine();
			bool needToArchive = info.Contains(@"*");
			Console.WriteLine("В архиве будет создана следующая запись:");
			string bookRecord = string.Format("{0:yyyyMMdd} - " + title + ", автор " + author + " [" + info + "]", DateTime.Today);
			ColoredConsoleWrite(ConsoleColor.Red, bookRecord + Environment.NewLine);
			Console.Write("Подтвердите операцию (нажмите [Enter]): ");
			if (Console.ReadKey().Key == ConsoleKey.Enter) {
				System.IO.File.AppendAllText(f.HaveReadFile.FullName, bookRecord + Environment.NewLine);
				// if this is a web-file, store it's Id in a file
				var result = File.ReadAllLines(f.ToReadWebFile.FullName).FirstOrDefault(s => s.Contains(curFile.Name));
				if (result != null) {
					var id = result.Split(":".ToCharArray())[0];
					System.IO.File.AppendAllText(f.HaveReadWebIds.FullName, id + Environment.NewLine);
				}
				
				
				if (needToArchive) {
					f.archiveBook(curFile);
				}
				f.removeBook(curFile);
				Console.WriteLine();
			} 
		}
		
		public static void ColoredConsoleWrite(ConsoleColor color, string text) {
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(text);
			Console.ForegroundColor = originalColor;
		}
	}
	
}