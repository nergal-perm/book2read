/*
 * Created by SharpDevelop.
 * User: terekhov-ev
 * Date: 26.02.2015
 * Time: 9:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
#if TEST
using System;
using NUnit.Framework;
using book2read.Utilities;

namespace book2read.Tests
{
	[TestFixture]
	public class LoggingServiceTest {
		LoggingService _log;
		
		[Test]
		public void TestEmptyLog() {
			string _expected = "";
			Assert.AreEqual(_expected, string.Join("\n", _log.GetWholeLog()));
		}
		
		[Test]
		public void TestSimpleLogOperation() {
			// TODO: Переписать с использованием списка и метода Contains.
			string _expected = "INFO: Hello, world!\nDEBUG: Hello, world!";
			_log.RecordMessage("Hello, world!", LoggingService.MessageType.Info);
			_log.RecordMessage("Hello, world!", LoggingService.MessageType.Debug);
			Assert.AreEqual(_expected, string.Join("\n",_log.GetWholeLog()));
			Assert.AreEqual("INFO: Hello, world!", string.Join("\n",_log.GetLogMessagesBySeverity(LoggingService.MessageType.Info)));
			Assert.AreEqual("DEBUG: Hello, world!", string.Join("\n",_log.GetLogMessagesBySeverity(LoggingService.MessageType.Debug)));
		}
		
		[TestFixtureSetUp]
		public void Init() {
			_log = LoggingService.Instance;
		}
		
		[TestFixtureTearDown]
		public void Dispose() {
			_log = null;
		}
	}
}
#endif
