/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 14:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace book2read.Commands {
	/// <summary>
	/// Description of GetCommand.
	/// </summary>
	public class GetCommand : BaseCommand {
		public GetCommand(string[] commandLine)
			: base(commandLine) {
		}
		
		
		#region implemented abstract members of BaseCommand
		public override bool run() {
			int.TryParse(_commandLine[1], out _bookIndex);
			int local = _fs.isLibraryFound() ? _fs.LocalBoksCount : 0;
			int web = _fs.isWebLibraryAvailable() ? _fs.WebBooksCount : 0;
			int flibusta = _fs.FlibustaBooksCount;
		
			if (local + web + flibusta == 0) {
				return true;
			}
			
			var bookNums = getRandomSequence(_bookIndex, local + web + flibusta);
			
			foreach (var element in bookNums) {
				if (element < local) {
					_fs.getBookFromLocalLibrary(element);
				} else if (element < local + web) {
					_fs.getBookFromOnlineLibrary(element - local);
				} else {
					_fs.getBookFromFlibusta(element - local - web);
				}
			}			
			
			return true;
		}
		public override bool argsAreOk() {
			return checkArgsCommandAndNumber();
		}
		#endregion
	
		List<int> getRandomSequence(int index, int maxValue) {
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
	}
}
