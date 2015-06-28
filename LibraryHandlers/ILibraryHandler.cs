/*
 * Created by SharpDevelop.
 * User: Yosemite
 * Date: 20.06.2015
 * Time: 19:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.LibraryHandlers
{
	/// <summary>
	/// Description of ILibraryHandler.
	/// </summary>
	public interface ILibraryHandler {
		int getDaysAfterLastUpdate();
		void updateLibrary();
		int getBookCount();
		BookInfo[] getRandomBooks(int quantity);
		void deleteFromLibrary(BookInfo bookToDelete);
		void markAsRead(BookInfo readBook);
	}
}
