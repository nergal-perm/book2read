/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 02.03.2015
 * Time: 15:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using book2read.Utilities;

namespace book2read.Commands {
	/// <summary>
	/// Description of ICommand.
	/// </summary>
	public abstract class BaseCommand {
		protected readonly string[] _commandLine;
		protected int _bookIndex;
		protected readonly FileSystemService _fs;

		protected BaseCommand(string [] command) {
			_commandLine = command;
			_fs = FileSystemService.Instance;
		}
		
		public abstract bool run();
		
		public abstract bool argsAreOk();
		
		/// <summary>
		/// Проверяет наличие двух обязательных аргументов:
		/// [0]: собственно команда
		/// [1]: индекс книги из списка чтения
		/// </summary>
		/// <returns>true, если индекс в допустимом диапазоне</returns>
		protected bool checkArgsCommandAndNumber() {
			// Аргументов должно быть ровно 2 (команда и номер книги)
			if (_commandLine.Length != 2) {
				return false;
			}

			// Номер книги должен находиться в пределах общего 
			// количества книг в очереди чтения и не быть меньше нуля
			int.TryParse(_commandLine[1], out _bookIndex);
			_bookIndex--;
			if (_bookIndex < 0 || _bookIndex >= _fs.getCurrentQueue().Length) {
				return false;
			}
			
			// Все в порядке
			return true;			
		}
				
	}
}
