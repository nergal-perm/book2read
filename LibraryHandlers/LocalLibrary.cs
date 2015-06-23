/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 23.06.2015
 * Time: 13:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.LibraryHandlers
{
	/// <summary>
	/// Description of LocalLibrary.
	/// </summary>
	public class LocalLibrary : AbstractLibraryHandler {
		private String libraryDirectory;
		
		public LocalLibrary() : base() {}
		
		public LocalLibrary(String path){
			libraryDirectory = path;
			
		}
		
		public override void updateLibrary() {
			bookCount = 1;
		}
		
		public override bool isAvailable() {
			return true;
		}
	}
}
