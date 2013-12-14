using System;

namespace Guineu
{
	partial class CLEAR : ICommand
	{
		public void Compile(CodeBlock code)
		{
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			DoClear();
		}

		partial void DoClear();
	}
}
