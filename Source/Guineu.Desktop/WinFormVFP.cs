using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu
{
	public partial class WinFormVFP : Form
	{
		public WinFormVFP()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//var code = new CompiledProgram("winform.fxp");
			//var procedure = code.Locate(new Nti("netClick"));
			//Variant retVal = GuineuInstance.Context.ExecuteInNewContext(procedure, null);
			//textBox1.Text = retVal.ToString(null);
		}

		private void WinFormVFP_Load(object sender, EventArgs e)
		{
		}
	}
}