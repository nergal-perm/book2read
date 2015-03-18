/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 18.03.2015
 * Time: 13:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.Commands
{
	/// <summary>
	/// Description of ReportCommand.
	/// </summary>
	public class ReportCommand: BaseCommand
	{
		public ReportCommand(string[] commandLine) : base(commandLine)	{}
		
		
		#region implemented abstract members of BaseCommand
		public override bool run() {
			throw new NotImplementedException();
		}
		public override bool argsAreOk() {
			throw new NotImplementedException();
		}
		#endregion
	}
}
