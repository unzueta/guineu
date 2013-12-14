using System;
using System.Collections.Generic;
using System.Text;

using Guineu.Expression;
using Guineu.Data;

namespace Guineu
{
	partial class SQLSTRINGCONNECT : ExpressionBase
	{
		ExpressionBase _ConnectionString;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					_ConnectionString = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = new Variant(GetInt(context),10);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			Int32 handle;
			ISptEngine engine = GuineuInstance.Connections.Engine;
			SptConnection cn = engine.StringConnect(_ConnectionString.GetString(context));
			if (cn != null)
			{
				cn.Engine = engine;
				handle = GuineuInstance.Connections.Add(cn);
			}
			else
			{
				handle = -1;
			}
			return handle;
		}
	}
}