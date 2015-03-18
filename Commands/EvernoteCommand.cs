/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 13.03.2015
 * Time: 8:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
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
			Console.ReadLine();
			return true;
		}
		public override bool argsAreOk() {
			throw new NotImplementedException();
		}
		#endregion

		void connectToEvernote() {
			Console.WriteLine("Connecting to Evernote...");
			try {
				ENSession.SetSharedSessionDeveloperToken("S=s1:U=900da:E=151ab755e3b:C=14a53c43030:P=1cd:A=en-devtoken:V=2:H=a26c9a039a23f8a387534810682c5d7f",
					"https://evernote.com/shard/s1/notestore");
				if (!ENSession.SharedSession.IsAuthenticated) {
					ENSession.SharedSession.AuthenticateToEvernote();
					Console.WriteLine("Got EN Connection");
				} else {
					Console.WriteLine("Authenticated");
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
			
			List<ENSessionFindNotesResult> myNotesList = ENSession.SharedSession.FindNotes(ENNoteSearch.NoteSearch(""), 
				                                             null, ENSession.SearchScope.All, 
				                                             ENSession.SortOrder.RecentlyUpdated, 20);

			if (myNotesList.Count > 0) {
				foreach (ENSessionFindNotesResult result in myNotesList) {
					// Each ENSessionFindNotesResult has a noteRef along with other important metadata.
					Console.WriteLine("Found note with title: " + result.Title);
				}
			}			
			
		}

	}
}
