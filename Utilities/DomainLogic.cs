/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 02.03.2015
 * Time: 15:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;


namespace book2read.Utilities {
	public struct Stats {
		public int months;
		public int bookCount;
		public int pagesCount;
		public int newBooks;
		public int likedBooks;
		public int cumulativeRating;
		public DateTime startDate;
	}
	
	/// <summary>
	/// Description of DomainLogic.
	/// </summary>
	public static class DomainLogic {
		/// <summary>
		/// Записывает информацию о прочитанной книге в "читательский дневник", при
		/// необходимости сохраняет файл с книгой в архив, затем удаляет книгу из локальной
		/// библиотеки (при наличии возможности) и из текущего списка книг "в чтении".
		/// </summary>
		/// <param name="bookInfo">Структура: строка для записи в дневник и ссылка на файл книги</param>
		public static void registerReadBook(BookMetaData bookInfo) {
			var fs = FileSystemService.Instance;
			
			fs.appendBookInfoToReadLog(bookInfo);

			if (bookInfo.dbRow.Contains("*")) {
				fs.archiveBook(bookInfo.file);
			}

			fs.removeFromLibrary(bookInfo.file);
			fs.removeFromQueue(bookInfo.file);
		}

		/* Хочется увидеть:
 *    - среднее количество произведений в месяц
 *    - средний объем одного произведения
 *    - сколько страниц в среднем читаю за месяц
 *    - каков процент новых книг
 * 	  - каков процент понравившихся книг
 *    - средний рейтинг прочитанных книг
 */
		
		public static void getReadingStats(string _startDate, int _months) {
			string[] readLog = FileSystemService.Instance.getWholeReadingLog();
			CultureInfo provider = CultureInfo.InvariantCulture;					
			
			DateTime endDate;
			DateTime startDate = DateTime.ParseExact(_startDate + "01", "yyyyMMdd", provider);
			
			endDate = _months == 0 ? DateTime.Today.AddDays(1) : startDate.AddMonths(_months);
			
			var stats = new Stats();
			stats.months = ((endDate - startDate).Days) / 30;
			
			
			foreach (var line in readLog) {
				var lineDate = DateTime.ParseExact(line.Substring(0, 8), "yyyyMMdd", provider);

				if (lineDate >= startDate && lineDate < endDate) {
					stats.bookCount++;
					string[] splitLine = line.Split("[]".ToCharArray());
					stats.pagesCount += int.Parse(splitLine[1].Split(":".ToCharArray())[0]);
					stats.cumulativeRating += int.Parse(splitLine[1].Split(":".ToCharArray())[1]);
					if (splitLine[2].Contains("*")) {
						stats.likedBooks++;
					}
					if (!splitLine[2].Contains("^")) {
						stats.newBooks++;
					}
				}
			}	
			
			UserInterface.showStatistics(stats);

		}
	}
}
