/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 11:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.Commands {
	/// <summary>
	/// Команда для осуществленя выхода из приложения
	/// </summary>
	public class ExitCommand : BaseCommand {
		public ExitCommand(string [] command) : base(command) {}
		
		#region implemented abstract members of BaseCommand

		public override bool run() {
			return false;
		}

		public override bool argsAreOk() {
			return true;
		}

		#endregion
	}
}
