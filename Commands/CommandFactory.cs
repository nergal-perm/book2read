/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 02.03.2015
 * Time: 15:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.Commands {
	/// <summary>
	/// Description of CommandFactory.
	/// </summary>
	public static class CommandFactory {
		public static BaseCommand getCommand(string[] commandLine) {
			BaseCommand command = null;
			
			switch (commandLine[0].ToUpper()) {
				case "READ":
					command = new ReadCommand(commandLine);
					break;
				case "DELETE":
					command = new DeleteCommand(commandLine);
					break;
				case "UPDATE":
					command = new UpdateCommand(commandLine);
					break;
				case "GET":
					command = new GetCommand(commandLine);
					break;
				case "CLEAR":
					command = new ClearCommand(commandLine);
					break;
				case "EXIT":
					command = new ExitCommand(commandLine);
					break;
				case "STATS":
					command = new StatCommand(commandLine);
					break;
				case "SELECT":
					//TODO: Организовать выборку
					/* Например, выбрать все понравившиеся книги, чтобы выбрать из них
					 * какую-нибудь и перечитать ее. Или выбрать книги по авторам. Или 
					 * выбрать книги, содержащие какое-нибудь слово в названии.
					 */
					break;
				case "EVERNOTE":
					command = new EvernoteCommand(commandLine);
					break;
				case "REPORT":
					command = new ReportCommand(commandLine);
					break;
				default:
					command = new HelpCommand(commandLine);
					break;
			}
			
			if (command == null) {
				command = new HelpCommand(commandLine);
			}
			return command;
		}
	}
}
