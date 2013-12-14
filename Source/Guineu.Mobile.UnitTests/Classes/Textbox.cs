using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guineu.Mobile.UnitTests.Classes
{
	[TestClass]
	public class Textbox
	{
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Can we instantiate a textbox without errors?
		/// </summary>
		// TT #159
		[TestMethod]
		[DeploymentItem("Guineu.compact.runtime.dll")]
		public void CreateInstance()
		{
			GuineuInstance.InitInstance();
			GuineuInstance.DebugMode = true;
			GuineuInstance.Do("Textbox.fxp", "CreateInstance");
		}
	}
}