using System.Collections.Generic;
using Guineu.Data;
using Guineu.Data.Engines.Dbf;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SEEKFunction : ExpressionBase
	{
		ExpressionBase value;
		ExpressionBase alias;
		ExpressionBase tag;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					value = param[0];
					break;
				case 2:
					value = param[0];
					alias = param[1];
					break;
				case 3:
					value = param[0];
					alias = param[1];
					tag = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}

		internal override bool GetBool(CallingContext context)
		{
			ICursor csr = context.GetCursor(alias);
			var tbl = csr as Table;
			if (tbl == null)
				throw new ErrorException(ErrorCodes.NoIndexOrderSet);
			tbl.Seek(value.GetVariant(context));
			return tbl.Found;
		}

	}

}