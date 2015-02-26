/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 25.02.2015
 * Time: 14:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Linq;
using book2read.Utilities;

namespace book2read
{
	class Program {

		
		public static void Main(string[] args) {   
			
			// Инициализировать приложение, показать пользователю общую информацию
			initAndWelcome();
			// Ожидать ввода команды пользователем, отрабатывать их исполнение
			
			// После получения команды exit организовать выход из консоли
			
			
			if (args.Length > 0) {
				//_arguments = new string[args.Length - 1];
				//System.Array.Copy(args, 1, _arguments, 0, args.Length - 1);			
			} else {
				Console.WriteLine("Не указана команда для выполнения");
				Console.WriteLine("Нажмите любую клавишу для выхода из программы...");
				Console.Read();
				return;
			}
			Console.WriteLine("Current command: " + string.Join(" ", args));
			Console.WriteLine("Нажмите любую клавишу для выхода из программы...");
			Console.Read();
		}

		static void SampleCode() {
			// Compile conditional sample, use WORK / HOME / TEST to mark dev environment
			#if WORK
			Console.WriteLine("This is WORK-DEBUG build");
			#else	
			Console.WriteLine("This is HOME-DEBUG build");
			#endif
			
			
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
		
		static void ShowPercentProgress(string message, int currElementIndex, int totalElementCount)
		{
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
			                  (f.isLibraryFound() ? 
			                   "найдена в папке " + f.LibraryPath.FullName :
			                   "не найдена."));
		}
	}
	
}