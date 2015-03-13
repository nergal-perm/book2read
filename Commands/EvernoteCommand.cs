/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 13.03.2015
 * Time: 8:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using EvernoteSDK;

namespace book2read.Commands {
	/// <summary>
	/// Description of EvernoteCommand.
	/// </summary>
	public class EvernoteCommand : BaseCommand {
		public EvernoteCommand(string[] commandLine)
			: base(commandLine) {
		}
		

		#region implemented abstract members of BaseCommand
		public override bool run() {
			connectToEvernote();
			return true;
		}
		public override bool argsAreOk() {
			throw new NotImplementedException();
		}
		#endregion

		void connectToEvernote() {
			Console.WriteLine("Connecting to Evernote...");
			try {
				ENSession.SetSharedSessionDeveloperToken("",
					"");
				if (!ENSession.SharedSession.IsAuthenticated) {
					ENSession.SharedSession.AuthenticateToEvernote();
					Console.WriteLine("Got EN Connection");
				} else {
					Console.WriteLine("No Connection");
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			} finally {
				Console.WriteLine("What the hell?");
				Console.ReadLine();
			}
		}
	}
}
