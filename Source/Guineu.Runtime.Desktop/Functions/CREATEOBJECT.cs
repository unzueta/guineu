using System.Collections.Generic;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Functions
{
	class CREATEOBJECT : ExpressionBase
	{
		ExpressionBase className;
		readonly List<ExpressionBase> parms = new List<ExpressionBase>();

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					className = param[0];
					break;
				default:
					className = param[0];
					parms.AddRange(param.GetRange(1, param.Count - 1));
					break;
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var name = className.ToNti(context);
			ObjectBase newObj = GuineuInstance.ObjectFactory.CreateObject(context, name, parms);
			return new Variant(newObj);
		}

	}
}
