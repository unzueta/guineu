using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guineu.Mobile.UnitTests.Classes
{
	[TestClass]
	public class Timer
	{
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Can we instantiate a timer without errors?
		/// </summary>
		[TestMethod]
		[DeploymentItem("Guineu.compact.runtime.dll")]
		public void CreateInstance()
		{
			GuineuInstance.InitInstance();
			GuineuInstance.DebugMode = true;
			GuineuInstance.Do("Timer.fxp", "CreateInstance");
		}
	}
}