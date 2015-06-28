/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 23.06.2015
 * Time: 13:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace book2read.Wrappers
{
	/// <summary>
	/// Description of FileSystemWrapper.
	/// </summary>
	public class FileSystemWrapper {
		protected DirectoryInfo _libraryPath;
		
		public FileSystemWrapper() {}
		
		public void setLibraryPath(String path) {
			_libraryPath = new DirectoryInfo(path);
			if (!_libraryPath.Exists) {
				_libraryPath = getLibraryDirectory();
			}
		}
		
		public DirectoryInfo getLibraryPath() {
			return _libraryPath;
		}
		
		private DirectoryInfo getLibraryDirectory() {
			foreach (var drive in DriveInfo.GetDrives()) {
				if (drive.IsReady && drive.VolumeLabel.Equals("Big Storage")) {
					return new DirectoryInfo(drive.Name + @"Library\");
				}
			}
			return null;
		}
	}
}
