using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	partial class GETENV : ExpressionBase
	{
		internal override string GetString(CallingContext context)
		{
			String env = Environment.GetEnvironmentVariable(
				envVariable.GetString(context)
			);
			if (String.IsNullOrEmpty(env))
				return "";
			else
				return env;
		}
	}
}