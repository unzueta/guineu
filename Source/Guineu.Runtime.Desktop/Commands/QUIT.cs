using System;

namespace Guineu
{

	class QUIT : ICommand
	{

		public void Compile(CodeBlock code)
		{
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			GuineuInstance.WinMgr.ClearEvents();
		}

	}

}
