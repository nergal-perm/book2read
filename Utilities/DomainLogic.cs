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
	
	public enum StatsPeriod {
		ThisMonth,
		YearToDate,
		Total
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
				fs.archiveBook(bookInfo);
			}

			fs.removeFromLibrary(bookInfo.file);
			fs.removeFromQueue(bookInfo.file);
		}

		public static void getReadingStats() {
			string[] readLog = FileSystemService.Instance.getWholeReadingLog();
			CultureInfo provider = CultureInfo.InvariantCulture;					
			
			/* Собираем статистику по трем периодам:
			 *  - текущий месяц
			 *  - с начала года
			 *  - с начала истории
			 */
			
			var statsMonth = new Stats();
			var statsYTD = new Stats();
			var statsTotal = new Stats();
			var startDate = new DateTime(0L);
			foreach (var line in readLog) {
				var lineDate = DateTime.ParseExact(line.Substring(0, 8), "yyyyMMdd", provider);
				if (startDate.Year == 0001) 
					startDate = lineDate;
				string[] splitLine = line.Split("[]".ToCharArray());
				int linePages = int.Parse(splitLine[1].Split(":".ToCharArray())[0]);
				int lineRating = int.Parse(splitLine[1].Split(":".ToCharArray())[1]);
				bool isNew = !splitLine[2].Contains("^");
				bool isLiked = splitLine[2].Contains("*");
				if (splitLine[2].Contains("@")) { continue; }
				
				// для общей статистики учитываем все строки
				statsTotal.bookCount++;
				statsTotal.pagesCount += linePages;
				if (isNew)
					statsTotal.newBooks++;
				if (isLiked)
					statsTotal.likedBooks++;
				statsTotal.cumulativeRating += lineRating;
				
				// отбираем строки этого года:
				if (lineDate.Year == DateTime.Today.Year) {
					statsYTD.bookCount++;
					statsYTD.pagesCount += linePages;
					if (isNew)
						statsYTD.newBooks++;
					if (isLiked)
						statsYTD.likedBooks++;
					statsYTD.cumulativeRating += lineRating;					
					
					// и если месяц == текущему, тогда еще и месяц заполняем
					if (lineDate.Month == DateTime.Today.Month) {
						statsMonth.bookCount++;
						statsMonth.pagesCount += linePages;
						if (isNew)
							statsMonth.newBooks++;
						if (isLiked)
							statsMonth.likedBooks++;
						statsMonth.cumulativeRating += lineRating;											
					}
				}

			}	
			statsTotal.months = MonthDiff(startDate, DateTime.Today) + 1;
			statsTotal.startDate = startDate;
			statsYTD.months = DateTime.Today.Month;
			statsMonth.months = 1;
			
			UserInterface.showStatisticsHeader();
			UserInterface.showStatistics(statsMonth, StatsPeriod.ThisMonth);
			UserInterface.showStatistics(statsYTD, StatsPeriod.YearToDate);
			UserInterface.showStatistics(statsTotal, StatsPeriod.Total);
			UserInterface.confirmOperation("", "", "Нажмите Enter для продолжения...", ConsoleKey.Enter);
			

		}
		
		private static int MonthDiff(DateTime startDate, DateTime endDate) {
			return (endDate.Month + endDate.Year * 12) - (startDate.Month + startDate.Year * 12);
		}
	}
}
