/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 06.07.2015
 * Time: 8:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using book2read.Utilities;

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
			string contentType = response.ContentType;
			if (!string.IsNullOrEmpty(contentDisposition) && contentType.Contains("zip")) {
				string lookFor = "/";
				int index = contentDisposition.LastIndexOf(lookFor, StringComparison.CurrentCultureIgnoreCase);
				if (index > 0)
					fileName = contentDisposition.Substring(index+1);
			}
			if (fileName.Length > 0) {
				client.DownloadFile(fileUrl, FileSystemService.Instance.ToReadPath.FullName + fileName);
			} else {
				// Do nothing
			}
			
			return true;
		}
	}
}
