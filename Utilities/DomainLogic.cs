﻿/*
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
		public int cumulativeRating;
		public DateTime startDate;
		public int secondsReading;
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
			 *  - предыдущий месяц
			 *  - с начала года
			 *  - с начала истории
			 */
			
			var statsMonth = new Stats();
			var statsPrevMonth = new Stats();
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
				
				// отбираем статистику для предыдущего месяца
				statsPrevMonth.startDate = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(-1);
				if (lineDate.Year == statsPrevMonth.startDate.Year && lineDate.Month == statsPrevMonth.startDate.Month) {
						statsPrevMonth.bookCount++;
						statsPrevMonth.pagesCount += linePages;
						if (isNew)
							statsPrevMonth.newBooks++;
						if (isLiked)
							statsPrevMonth.likedBooks++;
						statsPrevMonth.cumulativeRating += lineRating;																
				}

			}	
			statsTotal.months = MonthDiff(startDate, DateTime.Today) + 1;
			statsYTD.months = DateTime.Today.Month;
			statsMonth.months = 1;
			statsPrevMonth.months = 1;
			
			statsMonth.period=StatsPeriod.ThisMonth;
			statsPrevMonth.period = StatsPeriod.PrevMonth;
			statsYTD.period = StatsPeriod.YearToDate;
			statsTotal.period = StatsPeriod.Total;

			statsMonth.startDate = DateTime.Today;
			statsYTD.startDate = new DateTime(DateTime.Today.Year,1,1);
			statsTotal.startDate = startDate;
			
			statsMonth.secondsReading = 56659;
			statsPrevMonth.secondsReading = 52058;
			statsYTD.secondsReading = 43994 + 52058 + 56659;
			statsTotal.secondsReading = statsYTD.secondsReading + 19076;
			
			UserInterface.showStatisticsTable(new [] {statsMonth, statsPrevMonth, statsYTD, statsTotal});
			
			UserInterface.confirmOperation("", "", "Нажмите Enter для продолжения...", ConsoleKey.Enter);
			

		}
		
		private static int MonthDiff(DateTime startDate, DateTime endDate) {
			return (endDate.Month + endDate.Year * 12) - (startDate.Month + startDate.Year * 12);
		}

		public static void getReadingTimeReport() {
			string[] report = FileSystemService.Instance.getReadingTimeFile();
			string startDate = "";
			startDate = report.Length == 0 ? "2014-12-01" : report[0].Split("\t".ToCharArray())[0];
			string[] json = FileSystemService.Instance.getReportFromRescueTime(startDate);
			//HACK: Move to FileSystemService
			File.WriteAllLines(FileSystemService.Instance.ReportFile.FullName, json);
		}
	}
}

