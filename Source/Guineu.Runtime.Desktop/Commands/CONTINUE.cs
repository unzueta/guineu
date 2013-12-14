using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{

	class CONTINUE : ICommand
	{
		public void Compile(CodeBlock code)
		{
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			ICursor csr = context.GetCursor();
			ExpressionBase forClause = csr.LocateExpression;
			if (forClause == null)
				throw new ErrorException(ErrorCodes.ContinueWithoutLocate);
			while(true)
			{
				csr.Skip(1);
				if (csr.Eof())
					break;
				if (forClause == null || forClause.GetBool())
					break;
			}
			csr.Found = !csr.Eof();
		}
	}

}
