using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	partial class SYS1079 : ISys
	{
		public string getString(CallingContext context, List<ExpressionBase> param)
		{
			// TODO: Add a messagebox "Cause an intentional crash for test purposes" OK/Cancel.
			throw new SystemException();
		}
	}

}