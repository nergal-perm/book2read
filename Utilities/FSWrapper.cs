/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 29.06.2015
 * Time: 14:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace book2read.Utilities
{
	/// <summary>
	/// Description of FSWrapper.
	/// </summary>
	public class FSWrapper : IFileSystemWrapper {
		public FSWrapper() {
			
		}
		#region Реализация абстрактных членов
		public override int getAge() {
			throw new NotImplementedException();
		}
		
		public override int getLinesCount() {
			throw new NotImplementedException();
		}
		
		public override void updateFile() {
			throw new NotImplementedException();
		}		
		
		public override bool fileExists() {
			throw new NotImplementedException();
		}
		#endregion
	}
}
