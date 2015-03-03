/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 14:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using book2read.Utilities;

namespace book2read.Commands
{

	/// <summary>
	/// Description of DeleteCommand.
	/// </summary>
	public class ClearCommand : BaseCommand {
		public ClearCommand(string[] commandLine)
			: base(commandLine) {
		}

		#region implemented abstract members of BaseCommand

		public override bool run() {
			UserInterface.showHomeScreen();
			return true;
		}

		public override bool argsAreOk() {
			return true;
		}

		#endregion
	}
}
