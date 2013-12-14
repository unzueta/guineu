using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class RECNO : ExpressionBase
	{
		ExpressionBase _Alias;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					_Alias = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetInt(context), 10);
		}

		internal override int GetInt(CallingContext context)
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
			{
				return 0;
			}
			else
			{
				return (Int32)csr.RecNo;
			}
		}


	}

}