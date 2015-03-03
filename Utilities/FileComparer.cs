/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 03.03.2015
 * Time: 12:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.IO;

namespace book2read.Utilities
{
	/// <summary>
	/// Description of FileComparer.
	/// </summary>
	public class FileComparer : IComparer
	{

		#region IComparer implementation

		public int Compare(object x, object y) {
			string xName = ((FileInfo)x).Name;
			string yName = ((FileInfo)y).Name;
			return String.Compare(xName, yName, true);
		}

		#endregion
	}
}
