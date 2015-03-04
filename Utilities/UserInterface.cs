﻿/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 02.03.2015
 * Time: 15:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using book2read.Commands;

namespace book2read.Utilities {
	public struct BookMetaData {
		public string dbRow;
		public FileInfo file;
	}
	
	/// <summary>
	/// Description of UserInterface.
	/// </summary>
	public static class UserInterface {
		public static void showHomeScreen() {
			clearConsole();
			welcomeUser();
			databaseStatus();
			librariesAvailability();
			readingList();
		}
		
		static void clearConsole() {
			Console.Clear();
		}
		
		static void welcomeUser() {
			Console.WriteLine("Привет, читатель!");
			Console.WriteLine();
		}

		static void databaseStatus() {
			Console.Write("Твоя база данных по книгам обновлялась ");
			if (FileSystemService.Instance.isLibraryFileTooOld()) {
				ColoredConsoleWrite(ConsoleColor.Red, "больше");
			} else {
				ColoredConsoleWrite(ConsoleColor.Yellow, "меньше");
			}
			Console.WriteLine(" месяца назад.");
			Console.Write("В ней сейчас содержится ");
			ColoredConsoleWrite(ConsoleColor.Cyan, FileSystemService.Instance.getBookCount().ToString());
			Console.WriteLine(" записей о книгах.");
			Console.WriteLine();
		}

		static void librariesAvailability() {
			if (FileSystemService.Instance.isLibraryFound()) {
				Console.Write("Твоя локальная библиотека найдена в папке "); 
				ColoredConsoleWrite(ConsoleColor.Yellow, FileSystemService.Instance.LibraryPath.FullName + Environment.NewLine);
			} else {
				ColoredConsoleWrite(ConsoleColor.Red, "Твоя локальная библиотека не найдена." + Environment.NewLine);
			}

			if (FileSystemService.Instance.isWebLibraryAvailable()) {
				ColoredConsoleWrite(ConsoleColor.Yellow, "Доступ к сетевой библиотеке предоставлен." + Environment.NewLine);
			} else {
				ColoredConsoleWrite(ConsoleColor.Red, "Твоя сетевая библиотека недоступна." + Environment.NewLine);
			}
			Console.WriteLine();
		}

		static void readingList() {
			Console.Write("В твоем списке чтения сейчас ");
			ColoredConsoleWrite(ConsoleColor.Cyan, FileSystemService.Instance.getCurrentQueue().Length.ToString());
			Console.WriteLine(" книг:");
			int i = 0;
			foreach (var element in FileSystemService.Instance.getCurrentQueue()) {
				Console.WriteLine(++i + "\t" + element);
			}
			Console.WriteLine();
		}

		static void ColoredConsoleWrite(ConsoleColor color, string text) {
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(text);
			Console.ForegroundColor = originalColor;
		}
		
		/// <summary>
		/// Запрашивает у пользователя текст и аргументы комманды
		/// </summary>
		/// <returns>string[]: [0]-текст, [1]-аргумент</returns>
		public static BaseCommand getUserCommand() {
			Console.WriteLine("Введите команду:");
			return CommandFactory.getCommand(Console.ReadLine().Split(" ".ToCharArray()));
		}

		/// <summary>
		/// Запрашивает у пользователя дополнительную информацию о заданной книге
		/// </summary>
		/// <param name="_bookIndex">Номер книги в списке на чтение</param>
		/// <returns>BookMetaData: строка для записи в дневник и ссылка на файл</returns>
		public static BookMetaData getMetadataForFile(int _bookIndex) {
			var bookInfo = new BookMetaData();
			bookInfo.file = FileSystemService.Instance.getBookFromQueue(_bookIndex);
			Console.WriteLine("Выбран файл: " + bookInfo.file.Name);
			Console.Write("Введите название книги: ");
			string title = ToTitleCase(Console.ReadLine());
			Console.Write("Введите автора книги: ");
			string author = ToTitleCase(Console.ReadLine());
			Console.Write("Введите количество страниц: ");
			int pageCount;
			int.TryParse(Console.ReadLine(), out pageCount);
			Console.Write("Введите рейтинг по 10-балльной шкале: ");
			int rating;
			int.TryParse(Console.ReadLine(), out rating);			
			Console.Write("Введите доп.информацию [*^+@]: ");
			string code = Console.ReadLine();
			bookInfo.dbRow = string.Format("{0:yyyyMMdd} - " + title + ", автор " + author + " [" + pageCount + ":" + rating + "]" + code, DateTime.Today);
			return bookInfo;
		}

		/// <summary>
		/// Запрашивает у пользователя подтверждение операции "прочитать книгу"
		/// </summary>
		/// <param name="bookInfo">Данные о книге, которую планируется "прочитать"</param>
		/// <returns></returns>
		public static bool confirmReadOperation(BookMetaData bookInfo) {
			return confirmOperation("В архиве будет создана следующая запись:", 
				bookInfo.dbRow,
				"Подтвердите операцию (нажмите [Enter]): ",
				ConsoleKey.Enter);
		}

		/// <summary>
		/// Запрашивает у пользователя подтверждение операции "удалить книгу"
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool confirmDeleteFromQueueOperation(string name) {
			return confirmOperation("Следующий файл будет удален из списка чтения:", 
				name,
				"Подтвердите операцию (нажмите [Enter]): ",
				ConsoleKey.Enter);
		}

		public static bool confirmDeleteFromLibraryOperation() {
			return confirmOperation("", "",
				"Удалить его также и из библиотеки? [Enter] = Да",
				ConsoleKey.Enter);			
		}

		public static bool confirmUpdateOperation(TimeSpan timeSpan) {
			return confirmOperation("Библиотека обновлена за:", 
				timeSpan.Seconds + " секунд",
				"Нажмите Enter для продолжения.",
				ConsoleKey.Enter);
		}
		
		public static bool confirmOperation(string description, string argument, string question, ConsoleKey keyToWait) {
			if (description.Length != 0) {
				Console.WriteLine(description);
			}
			if (argument.Length != 0) {
				ColoredConsoleWrite(ConsoleColor.Red, argument + Environment.NewLine);
			}
			if (question.Length != 0) {
				Console.WriteLine(question);
			}
			return (Console.ReadKey().Key == keyToWait);
		}
		
		public static void showMessage(string message) {
			Console.Write(message);
		}

		public static void updateLongOperationStatus(int i) {
			Console.CursorLeft = Console.CursorLeft - 1;
			Console.Write(@"/-\|"[i % 4]);
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
		
		private static string ToTitleCase(string text) {
			TextInfo myTI;
			var r = new Regex(@"\p{IsCyrillic}");
			myTI = r.IsMatch(text) ? new CultureInfo("ru-RU", false).TextInfo : new CultureInfo("en-US", false).TextInfo;
			return myTI.ToTitleCase(text.ToLower());			
		}

		public static void showHelp() {
			Console.Clear();
			Console.WriteLine("Перечень доступных команд:");
			ColoredConsoleWrite(ConsoleColor.Green, "   update\t");
			Console.WriteLine("Обновить данные о книгах в доступных библиотеках.");			
			ColoredConsoleWrite(ConsoleColor.Green, "   get N\t");
			Console.WriteLine("Случайным образом выбрать N книг для чтения.");
			ColoredConsoleWrite(ConsoleColor.Green, "   read N\t");
			Console.WriteLine("Записать информацию о прочитанной книге в дневник,");			
			Console.WriteLine("\t\tудалить файл из библиотеки или переместить его в архив.");
			ColoredConsoleWrite(ConsoleColor.Green, "   delete N\t");
			Console.WriteLine("Удалить книгу из списка и, возможно, из библиотеки, не читая.");
			ColoredConsoleWrite(ConsoleColor.Green, "   clear\t");
			Console.WriteLine("Обновить статусную информацию.");			
			ColoredConsoleWrite(ConsoleColor.Green, "   exit\t\t");
			Console.WriteLine("Выход из приложения.");				
			
			confirmOperation("", "", "Нажмите Enter для продолжения...", ConsoleKey.Enter);
		}

		public static void showStatistics(Stats stats, StatsPeriod sp) {

			string header;
			switch (sp) {
				case StatsPeriod.ThisMonth:
					header = string.Format("Данные за текущий месяц {0:MMMM}:", DateTime.Today);
					break;
				case StatsPeriod.YearToDate:
					header = string.Format("Данные c начала {0:yyyy} года:", DateTime.Today);
					break;
				case StatsPeriod.Total:
					header = string.Format("Данные за всю историю чтения (начиная с {0:MMMMM yyyy} года):", stats.startDate);
					break;
				default:
					return;
			}
			
			ColoredConsoleWrite(ConsoleColor.DarkGray, header + Environment.NewLine);
			//ColoredConsoleWrite(ConsoleColor.DarkGray, "".PadRight(header.Length, "="[0]) + Environment.NewLine);
	
			Console.Write("Прочитано:\t");
			ColoredConsoleWrite(ConsoleColor.Cyan, string.Format("{0} книг\t\t", stats.bookCount));
			Console.Write("Объем:\t\t");
			ColoredConsoleWrite(ConsoleColor.Cyan, string.Format("{0} страниц\n", stats.pagesCount));         

			Console.Write("Новых книг:\t");
			ColoredConsoleWrite(ConsoleColor.Cyan, string.Format("{0} книг\t\t", stats.newBooks));
			Console.Write("Понравилось:\t");
			ColoredConsoleWrite(ConsoleColor.Cyan, string.Format("{0} книг\n", stats.likedBooks));
			
			Console.Write("Средний темп чтения: ");
			ColoredConsoleWrite(ConsoleColor.Green, string.Format("{0:##.00} книг ", (double)stats.bookCount / stats.months));
			Console.Write("или ");
			ColoredConsoleWrite(ConsoleColor.Green, string.Format("{0:##.00} страниц ", (double)stats.pagesCount / stats.months));
			Console.WriteLine("в месяц.");
			Console.Write("Средний рейтинг прочитанных книг: ");
			ColoredConsoleWrite(ConsoleColor.Green, string.Format("{0:##.00}.", (double)stats.cumulativeRating / stats.bookCount) + Environment.NewLine);
			
			Console.WriteLine();
		}

		public static void showStatisticsHeader() {
			Console.Clear();

			string header = string.Format("Отчет по статистике чтения:");
			Console.WriteLine(header);
			Console.WriteLine("".PadRight(header.Length, "="[0]));
			Console.WriteLine();
		}
	}
}