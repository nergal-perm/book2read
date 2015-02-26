/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 26.02.2015
 * Time: 9:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace book2read.Utilities {
	/// <summary>
	/// Description of LoggingServiceger.
	/// </summary>
	public sealed class LoggingService {
		public enum MessageType {
			Info = 1,
   			Failure = 2,
			Warning = 3,
			Error = 4,
			Debug = 5,
			Output = 6,
			System = 7
		}		
		
		readonly List<string> _log;
		
		#region Реализация "одиночки"
		private static readonly Lazy<LoggingService> lazy =
			new Lazy<LoggingService>(() => new LoggingService());
		
		public static LoggingService Instance { get { return lazy.Value; } }
		
		private LoggingService() {
			_log = new List<string>();
		}
		#endregion		
		
		#region implemented abstract members of Log

		public void RecordMessage(Exception Message, MessageType Severity) {
			RecordMessage(Message.Message, Severity);
		}

		public void RecordMessage(string Message, MessageType Severity) {
			var sb = new StringBuilder();
			sb.Append(Severity.ToString().ToUpper()).Append(": ").Append(Message);
			_log.Add(sb.ToString());
		}

		public string[] GetWholeLog() {
			// TODO: Возможно, стоит возвращать List<string>?
			return _log.ToArray();
		}

		public string[] GetLogMessagesBySeverity(MessageType severity) {
			// TODO: Возможно, стоит возвращать List<string>?
			var _result = new List<string>();
			foreach (string element in _log) {
				if (element.StartsWith(severity.ToString().ToUpper(), StringComparison.CurrentCulture)) {
					_result.Add(element);
				}
			}
			return _result.ToArray();
		}

		#endregion
	}
}
