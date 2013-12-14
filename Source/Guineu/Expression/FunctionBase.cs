using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	abstract class FunctionBase : ExpressionBase
	{
		protected List<ExpressionBase> Param;

		protected internal void GetParameters(Compiler comp, params Int32[] valid)
		{
			Param = comp.GetParameterList();
			if (valid.Length == 0)
				return;

			foreach (var i in valid)
				if (i == Param.Count)
					return;

			if(Param.Count>valid[valid.Length-1])
				throw new ErrorException(ErrorCodes.TooManyArguments);
			throw new ErrorException(ErrorCodes.TooFewArguments);
		}

		protected ExpressionBase GetParam(Int32 index)
		{
			if (index >= Param.Count)
				return null;
			return Param[index];
		}

		public override string ToString()
		{
			String s = base.ToString();
			s = s.Substring(s.LastIndexOf('.') + 1).ToUpper(System.Globalization.CultureInfo.InvariantCulture)+"(";

			for (var i = 0; i < Param.Count; i++)
			{
				if (i > 0)
					s = s + ",";
				s = s + Param[i];
			}

			return s+")";
		}
	}
}