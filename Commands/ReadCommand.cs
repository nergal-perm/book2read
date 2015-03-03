/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 02.03.2015
 * Time: 15:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using book2read.Utilities;

namespace book2read.Commands {
	/// <summary>
	/// Регистрирует книгу в списке прочитанных, архивирует файл с книгой
	/// (при необходимости) и удаляет его из локальной библиотеки.
	/// </summary>
	class ReadCommand : BaseCommand {
		public ReadCommand(string [] command) : base(command) {}
		
		#region implemented abstract members of BaseCommand

		public override bool run() {
			if (!argsAreOk()) {
				return true;
			}
			BookMetaData bookInfo = UserInterface.getMetadataForFile(_bookIndex);
			
			// Регистрация книги в качестве прочитанной осуществляется только
			// после ввода дополнительной информации и подтверждения операции
			// пользователем
			if (UserInterface.confirmReadOperation(bookInfo)) {
				DomainLogic.registerReadBook(bookInfo);
			} 
			
			return true;
		}

		public override bool argsAreOk() {
			return base.checkArgsCommandAndNumber();
		}

		#endregion

	}
}
