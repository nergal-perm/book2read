/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 29.06.2015
 * Time: 14:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using book2read.Utilities;

namespace book2read.UnitTests
{
	/// <summary>
	/// Description of FakeFSWrapper.
	/// </summary>
	public class FakeFSWrapper : IFileSystemWrapper {
		int _age;
		int _linesCount;
		bool _fileExists;
		
		#region Установщики свойств для поддельной реализации
		public void setAge(int newAge) {
			_age = newAge;
		}

		public void setAvailability(bool b) {
			_fileExists = b;
		}
		public void setLinesCount(int i) {
			_linesCount = i;
		}		
		#endregion
		
		#region Реализация абстрактных членов
		public override int getAge() {
			return _age;
		}

		public override int getLinesCount() {
			return _linesCount;
		}

		public override void updateFile() {
			_fileExists = true;
			_age = 0;
			_linesCount = _linesCount + 50;
		}
		
		public override bool fileExists() {
			return _fileExists;
		}
		#endregion
		
		public FakeFSWrapper() {
			
		}
	}
}
