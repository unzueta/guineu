using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guineu.Desktop.UnitTests
{
	[TestClass]
	public class RunFxpFiles
	{
		public TestContext TestContext { get; set; }
		public RunFxpFiles()
		{
			GuineuInstance.InitInstance();
			GuineuInstance.DebugMode = true;
		}
	
		[TestMethod]
		public void AssignArrayElementToValueProperty()
		{
			Assert.IsTrue( GuineuInstance.Do("AssignArrayElementToValueProperty"));
		}
	}
}
