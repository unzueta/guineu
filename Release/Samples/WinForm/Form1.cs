using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Guineu;

namespace WinForm
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Calling a function in an FXP file
		/// </summary>
		/// <remarks>
		/// This sample executes an existing procedure, passing a parameter. 
		/// The return value is added to a list.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnExec_Click(object sender, EventArgs e)
		{
			// Step 2: Create a list of parameters
			ParameterCollection parm = new ParameterCollection();
			ValueMember tcName = new ValueMember();
			tcName.Set(new Variant(txtName.Text));
			parm.Add(tcName);

			// Step 3: Locate the code
			CompiledProgram code = new CompiledProgram(@"..\..\winform.fxp");
			CodeBlock procedure = code.Locate("netClick");

			// Step 4: Call the function
			Variant retVal = GuineuInstance.Context.ExecuteInNewContext(procedure, parm);

			// Step 5: Handle the return value
			this.list.Items.Add(retVal.ToString(null));
		}

		/// <summary>
		/// Calling an FXP file
		/// </summary>
		/// <remarks>
		/// In this sample, we call just the FXP file without checking return values
		/// or passing parameters.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDo_Click(object sender, EventArgs e)
		{
			// Step 2: Execute the FXP file
			GuineuInstance.Do(@"..\..\winform.fxp");
		}

	}
}