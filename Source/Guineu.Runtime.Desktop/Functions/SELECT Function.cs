using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SelectFunction : ExpressionBase
	{
		ExpressionBase alias;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					alias = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetInt(context), 10);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			if (alias == null)
				return context.DataSession.Select();
			
			Variant value = alias.GetVariant(context);
			if (value.Type == VariantType.Character)
				return context.DataSession.Select(value.ToNti(context.Context));
			// TODO: Handle SELECT(0) and SELECT(1)
			return 1;
		}
	}
}