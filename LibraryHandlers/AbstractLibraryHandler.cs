/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 23.06.2015
 * Time: 13:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.LibraryHandlers
{
	/// <summary>
	/// Description of AbstractLibraryHandler.
	/// </summary>
	public abstract class AbstractLibraryHandler
	{
		public AbstractLibraryHandler()	{}
		
		// Количество книг в библиотеке
		protected int bookCount;
		public int getBookCount() {
			return bookCount;
		}
		
		public abstract Boolean isAvailable();
		
		public abstract void updateLibrary();
		
		
	}
}
