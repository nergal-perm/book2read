/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 25.02.2015
 * Time: 14:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using book2read.Utilities;

namespace book2read {
	class Program {
		
		public static void Main(string[] args) {   
			var f = FileSystemService.Instance;
			do {
				UserInterface.showHomeScreen();
			} while (dispatchCommand());
		}
		
		public static bool dispatchCommand() {
			if (UserInterface.getUserCommand().run()) {
				UserInterface.showHomeScreen();
				return true;
			} 
			return false;
		}
	}
}