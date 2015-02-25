/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 25.02.2015
 * Time: 18:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace book2read.Commands
{
	/// <summary>
	/// initialize application and renders some information
	/// about current reading state
	/// </summary>
	public class InitCommand : ManyConsole.ConsoleCommand
	{
		public InitCommand() {
			this.IsCommand("init", "Инициализирует приложение, выводит текущий список чтения");
		}
		
		public override int Run(string[] remainingArguments) {
			System.Console.WriteLine("Hello, World!");
			System.Console.Read();
			return 0;
		}
	}
}

