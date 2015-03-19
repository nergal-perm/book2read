/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 15:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using book2read.Utilities;

namespace book2read.Commands {
	/// <summary>
	/// Description of StatCommand.
	/// </summary>
	public class StatCommand : BaseCommand {
		
		public StatCommand(string[] commandLine)
			: base(commandLine) {
		}
		
		#region implemented abstract members of BaseCommand
		public override bool run() {
			if (!argsAreOk()) {
				return true;
			}
			
			DomainLogic.getReadingStats();
			return true;
		}
		
		public override bool argsAreOk() {
//			// Аргументов должно быть 2 или 3 (команда, дата начала, кол-во месяцев анализа)
//			if (!(_commandLine.Length == 2 || _commandLine.Length == 3) || _commandLine[1].Length != 6) {
//				return false;
//			}
//
//			_startDate = _commandLine[1];
//			
//			if (_commandLine.Length == 2) {
//				_months = 0;
//			} else {
//				int.TryParse(_commandLine[2], out _months);
//			}
			
			return true;
		}
		#endregion
	}
}
