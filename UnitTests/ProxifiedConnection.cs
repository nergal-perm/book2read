/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 06.07.2015
 * Time: 8:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Net;

namespace book2read.UnitTests {
	/// <summary>
	/// Description of ProxifiedConnection.
	/// </summary>
	public class ProxifiedConnection {
		WebClient client;
		WebProxy wp;
		
		public ProxifiedConnection() {
			client = new WebClient();
			wp = new WebProxy("127.0.0.1", 8118);
			client.Proxy = wp;
		}
		
		public bool DownloadFile(string fileUrl) {
			string fileName = string.Empty;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fileUrl);
			request.Proxy = wp;
			WebResponse response = request.GetResponse();
			string contentDisposition = request.Address.AbsoluteUri;
			if (!string.IsNullOrEmpty(contentDisposition)) {
				string lookFor = "/";
				int index = contentDisposition.LastIndexOf(lookFor, StringComparison.CurrentCultureIgnoreCase);
				if (index > 0)
					fileName = contentDisposition.Substring(index+1);
			}
			if (fileName.Length > 0) {
				client.DownloadFile(fileUrl, @"D:\Temp\" + fileName);
			} else {
				client.DownloadFile(fileUrl, @"D:\Temp\flibusta.fb2");
			}
			
			return true;
		}
	}
}
