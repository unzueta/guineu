using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class FIELD : ExpressionBase
	{
		ExpressionBase fieldNo;
		ExpressionBase _Alias;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					fieldNo = param[0];
					break;
				case 2:
					fieldNo = param[0];
					_Alias = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}

		internal override String GetString(CallingContext context)
		{
			ICursor csr;
			if (_Alias == null)
			{
				csr = context.DataSession.Cursor;
			}
			else
			{
				csr = context.DataSession[_Alias, context];
			}


			if (csr == null)
				return String.Empty;
			else
				return csr.GetFieldName(fieldNo.GetInt(context));
		}


	}

}