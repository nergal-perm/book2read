/*
 * Created by SharpDevelop.
 * User: Yosemite
 * Date: 20.06.2015
 * Time: 19:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using book2read.Model;

namespace book2read.LibraryHandlers
{
	/// <summary>
	/// Description of ILibraryHandler.
	/// </summary>
	public interface ILibraryHandler {
		int getDaysAfterLastUpdate();
		int getBookCount();
		bool isAvailable();
		
		void updateLibrary();
		BookInfo[] getRandomBooks(int quantity);
		void deleteFromLibrary(BookInfo bookToDelete);
		void markAsRead(BookInfo readBook);
		
	}
}
