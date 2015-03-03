/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 15:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using book2read.Utilities;

namespace book2read.Commands {
	/// <summary>
	/// Description of HelpCommand.
	/// </summary>
	public class HelpCommand : BaseCommand {
		public HelpCommand(string[] commandLine)
			: base(commandLine) {
		}

		#region implemented abstract members of BaseCommand

		public override bool run() {
			UserInterface.showHelp();
			return true;
		}

		public override bool argsAreOk() {
			return true;
		}

		#endregion
	}
}
