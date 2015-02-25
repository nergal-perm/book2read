/*
 * Created by SharpDevelop.
 * User: Yosemite
 * Date: 25.02.2015
 * Time: 19:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ManyConsole;

namespace book2read.Commands
{
	/// <summary>
	/// Description of AnotherCommand.
	/// </summary>
	public class AnotherCommand : ConsoleCommand   
	{
		public AnotherCommand()	{
			this.IsCommand("another", "Just another command");
		}

		#region implemented abstract members of ConsoleCommand

		public override int Run(string[] remainingArguments) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
