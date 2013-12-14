using System;
using System.Collections.Generic;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class ALIAS : ExpressionBase
	{
		ExpressionBase _WorkArea;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					_WorkArea = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}


		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			ICursor cr;
			if (_WorkArea == null)
				cr = context.DataSession.Cursor;
			else
				cr = context.DataSession[_WorkArea.GetInt(context)];
			if (cr == null)
				return "";
			return cr.Alias.ToString();
		}
	}

}