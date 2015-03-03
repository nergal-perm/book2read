/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 13:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.Commands {
	/// <summary>
	/// Description of UpdateCommand.
	/// </summary>
	public class UpdateCommand : BaseCommand {
		public UpdateCommand(string[] commandLine)
			: base(commandLine) {
		}

		#region implemented abstract members of BaseCommand

		public override bool run() {
			_fs.updateLibrary();
			return true;
		}

		public override bool argsAreOk() {
			return true;
		}

		#endregion
	}
}
