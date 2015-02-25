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

namespace book2read {
	class Program {
		public static void Main(string[] args) {   
			
			// Compile conditional sample, use WORK / HOME / TEST to mark dev environment
			#if WORK
			Console.WriteLine("This is WORK-DEBUG build");
			#else	
			Console.WriteLine("This is HOME-DEBUG build");
			#endif
			
			
			// Get n-th line number from file
			string FileName = @"D:\Temp\lines1-10.txt";
			string line = File.ReadLines(FileName).Skip(5).First();
			long line_count = File.ReadLines(FileName).Count();
			Console.WriteLine("Found {1} lines, here is line 6: {0}", line, line_count);
			
			// Show progress percent count
			for (int i = 0; i < 100; i++) {
				ShowPercentProgress("Processing...", i, 100);   
				System.Threading.Thread.Sleep(100);   
			}   
		}

		static void ShowPercentProgress(string message, int currElementIndex, int totalElementCount) {
			if (currElementIndex < 0 || currElementIndex >= totalElementCount) {
				throw new InvalidOperationException("currElement out of range");
			}
			int percent = (100 * (currElementIndex + 1)) / totalElementCount;
			Console.Write("\r{0}{1}% complete", message, percent);
			if (currElementIndex == totalElementCount - 1) {
				Console.WriteLine(Environment.NewLine);
			}
		}
	}
	
}