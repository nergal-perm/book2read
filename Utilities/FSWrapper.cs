/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 29.06.2015
 * Time: 14:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Linq;

namespace book2read.Utilities
{
	/// <summary>
	/// Description of FSWrapper.
	/// </summary>
	public abstract class FSWrapper : IFileSystemWrapper {
		protected readonly FileInfo file;
		
		protected FSWrapper() {}
		
		protected FSWrapper(string filePath) {
			file = new FileInfo(filePath);
		}
		
		#region Реализация абстрактных членов
		public override int getAge() {
			if (!this.fileExists()) {
				throw new FileNotFoundException("File " + file.FullName + " not found");	
			} 
			return (int)(DateTime.Today - file.LastWriteTime).TotalDays;
		}
		
		public override int getLinesCount() {
			if(!this.fileExists()) {
				throw new FileNotFoundException("File " + file.FullName + " not found");
			}
			return System.IO.File.ReadLines(file.FullName).Count();
		}
		
		public override bool fileExists() {
			return file.Exists;
		}
		#endregion
	}
}
