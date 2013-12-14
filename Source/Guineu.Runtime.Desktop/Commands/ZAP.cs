using System;
using Guineu.Data;
using Guineu.Gui;
using Guineu.Properties;

namespace Guineu.Commands
{
	class ZAP : ICommand
	{

		public void Compile(CodeBlock code)
		{
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			ICursor csr;
			csr = exec.DataSession.Cursor;
			if(csr==null)
				throw new ErrorException(ErrorCodes.NoTableOpen);
			if (GuineuInstance.Set.Safety.Value)
			{
				if (PromptToZap(csr.Filename))
					csr.Zap();
			}
			else
				csr.Zap();
		}

		static Boolean PromptToZap(String path)
		{
			return GuineuInstance.WinMgr.MessageBox(
							"ZAP "+path+"?",
							Resources.App_Title,
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question) == DialogResult.Yes;
		}
	}
}