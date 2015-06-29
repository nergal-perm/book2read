/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 29.06.2015
 * Time: 14:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.Utilities
{
	/// <summary>
	/// Description of IFileSystemWrapper.
	/// </summary>
	public abstract class IFileSystemWrapper {
		public abstract int getAge();
		public abstract int getLinesCount();
		public abstract void updateFile();
		public abstract bool fileExists();
	}
}
