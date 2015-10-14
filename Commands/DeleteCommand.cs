/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 11:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using book2read.Utilities;

namespace book2read.Commands {
	/// <summary>
	/// Description of DeleteCommand.
	/// </summary>
	public class DeleteCommand : BaseCommand {
		public DeleteCommand(string[] commandLine)
			: base(commandLine) {
		}

		#region implemented abstract members of BaseCommand

		public override bool run() {
			if (!argsAreOk()) {
				return true;
			}
			var bookInfo = new BookMetaData();
			bookInfo.file = _fs.getBookFromQueue(_bookIndex);
			
			if (UserInterface.confirmDeleteFromQueueOperation(bookInfo.file.Name)) {
				if (UserInterface.confirmDeleteFromLibraryOperation()) {
					_fs.removeFromLibrary(bookInfo.file);
					_fs.appendIdToWebIds(bookInfo);
				}
				_fs.removeFromQueue(bookInfo.file);
			}
			
			return true;
		}

		public override bool argsAreOk() {
			return checkArgsCommandAndNumber();
		}

		#endregion
	}
}
