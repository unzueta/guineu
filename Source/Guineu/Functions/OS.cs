using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu
{
	class OS : ExpressionBase
	{
		ExpressionBase value;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 1:
					value = param[0];
					break;
default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			Int32 index;
			if (value == null)
				index = 1;
			else 
			index = value.GetVariant(exec);
			switch (index)
			{
				case 1:
					return new Variant(Environment.OSVersion.ToString());
				case 2:
					return new Variant("");
				case 3:
					return new Variant(Environment.OSVersion.Version.Major.ToString());
				case 4:
					return new Variant(Environment.OSVersion.Version.Minor.ToString());
				case 5:
					return new Variant(Environment.OSVersion.Version.Build.ToString());
				case 6:
					return new Variant(((Int32) Environment.OSVersion.Platform).ToString());
			}
			throw new ErrorException(ErrorCodes.InvalidArgument);
			}
	}

}