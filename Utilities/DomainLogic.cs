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
using System.IO;


namespace book2read.Utilities {
	public struct Stats {
		public StatsPeriod period;
		public int months;
		public int bookCount;
		public int pagesCount;
		public int newBooks;
		public int likedBooks;
		public int audioBooks;		
		public int cumulativeRating;
		public DateTime startDate;
		public TimeSpan timeReading;
	}
	
	public enum StatsPeriod {
		ThisMonth,
		PrevMonth,
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

			if (bookInfo.file != null) {
				if (bookInfo.dbRow.Contains("*")) {
					fs.archiveBook(bookInfo);
				}
	
				fs.removeFromLibrary(bookInfo.file);
				fs.removeFromQueue(bookInfo.file);				
			}
		}

		public static void getReadingStats() {
			/* Собираем статистику по четырем периодам:
			 *  - текущий месяц
			 *  - предыдущий месяц
			 *  - с начала года
			 *  - с начала истории
			 */
			
			var statsMonth = new Stats();
			var statsPrevMonth = new Stats();
			var statsYTD = new Stats();
			var statsTotal = new Stats();
			
			statsMonth.period=StatsPeriod.ThisMonth;
			statsPrevMonth.period = StatsPeriod.PrevMonth;
			statsYTD.period = StatsPeriod.YearToDate;
			statsTotal.period = StatsPeriod.Total;
			
			string[] readLog = FileSystemService.Instance.getWholeReadingLog();
			CultureInfo provider = CultureInfo.InvariantCulture;
			var startDate = new DateTime(0L);
			foreach (var line in readLog) {
				var lineDate = DateTime.ParseExact(line.Substring(0, 8), "yyyyMMdd", provider);
				if (startDate.Year == 0001)
					startDate = lineDate;
				string[] splitLine = line.Split("[]".ToCharArray());
				
				// Пропускаем аудиокниги
				//if (splitLine[2].Contains("@")) { continue; }
				
				int linePages = int.Parse(splitLine[1].Split(":".ToCharArray())[0]);
				int lineRating = int.Parse(splitLine[1].Split(":".ToCharArray())[1]);
				bool isNew = !splitLine[2].Contains("^");
				bool isLiked = splitLine[2].Contains("*");
				bool isAudio = splitLine[2].Contains("@");
				
				
				// для общей статистики учитываем все строки
				statsTotal.pagesCount += linePages;
				if (isNew)
					statsTotal.newBooks++;
				if (isLiked)
					statsTotal.likedBooks++;
				if (isAudio) {
					statsTotal.audioBooks++;
				} else {
					statsTotal.bookCount++;
				}
				statsTotal.cumulativeRating += lineRating;
				
				// отбираем строки этого года:
				if (lineDate.Year == DateTime.Today.Year) {
					statsYTD.pagesCount += linePages;
					if (isNew)
						statsYTD.newBooks++;
					if (isLiked)
						statsYTD.likedBooks++;
					if (isAudio) {
						statsYTD.audioBooks++;					
					} else {
						statsYTD.bookCount++;
					}
					statsYTD.cumulativeRating += lineRating;
					
					// и если месяц == текущему, тогда еще и месяц заполняем
					if (lineDate.Month == DateTime.Today.Month) {
						statsMonth.pagesCount += linePages;
						if (isNew)
							statsMonth.newBooks++;
						if (isLiked)
							statsMonth.likedBooks++;
						if (isAudio) {
							statsMonth.audioBooks++;
						} else {
							statsMonth.bookCount++;
						}
						statsMonth.cumulativeRating += lineRating;
					}
				}
				
				// отбираем статистику для предыдущего месяца
				statsPrevMonth.startDate = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(-1);
				if (lineDate.Year == statsPrevMonth.startDate.Year && lineDate.Month == statsPrevMonth.startDate.Month) {
					statsPrevMonth.pagesCount += linePages;
					if (isNew)
						statsPrevMonth.newBooks++;
					if (isLiked)
						statsPrevMonth.likedBooks++;
					if (isAudio) {
						statsPrevMonth.audioBooks++;
					} else {
						statsPrevMonth.bookCount++;
					}
					statsPrevMonth.cumulativeRating += lineRating;
				}

			}
			statsTotal.months = MonthDiff(startDate, DateTime.Today) + 1;
			statsYTD.months = DateTime.Today.Month;
			statsMonth.months = 1;
			statsPrevMonth.months = 1;
			
			statsMonth.startDate = DateTime.Today;
			statsYTD.startDate = new DateTime(DateTime.Today.Year,1,1);
			statsTotal.startDate = startDate;
			
			FileSystemService.Instance.updateReadingTimeReport();
			
			string[] readTimeLog = FileSystemService.Instance.getReadingTimeFile();
			foreach (var line in readTimeLog) {
				DateTime dt = DateTime.ParseExact(line.Split("\t".ToCharArray())[0],"yyyy-MM-dd", provider);
				TimeSpan seconds = new TimeSpan(0,0, int.Parse(line.Split("\t".ToCharArray())[1]));
				// для общей статистики учитываем все строки
				statsTotal.timeReading += seconds;
				if (dt.Year == DateTime.Today.Year) {
					statsYTD.timeReading += seconds;
					if (dt.Month == DateTime.Today.Month) {
						statsMonth.timeReading += seconds;
					}
				}
				if (dt == new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(-1)) {
					statsPrevMonth.timeReading += seconds;
				}
			}

			UserInterface.showStatisticsTable(new [] {statsMonth, statsPrevMonth, statsYTD, statsTotal});
			UserInterface.confirmOperation("", "", "Нажмите Enter для продолжения...", ConsoleKey.Enter);
			

		}

		private static int MonthDiff(DateTime startDate, DateTime endDate) {
			return (endDate.Month + endDate.Year * 12) - (startDate.Month + startDate.Year * 12);
		}
		
	}
}

