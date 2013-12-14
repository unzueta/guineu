using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Guineu;

namespace WinForm
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// Step 1: Initialize the Guineu runtime environment
			GuineuInstance.InitInstance();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}