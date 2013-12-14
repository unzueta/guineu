using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS16 : ISys
	{
		/// <summary>
		/// Executing program file Name
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			if (param.Count >= 2)
			{
				Variant value = param[1].GetVariant(context);
				if (value.IsNull)
				{
					throw new ErrorException(ErrorCodes.InvalidArgument);
				}
			}

			Int32 level;
			if (param.Count >= 2)
			{
				level = param[1].GetInt(context);
				if (level < 1)
				{
					level = 1;
				}
			}
			else
			{
				level = context.Context.Stack.Count;
			}
			if (level > context.Context.Stack.Count)
			{
				return "";
			}


			String name = context.Context.Stack[level - 1].ModuleName;
			if (name.Length > 0)
			{
				name = "PROCEDURE " + name + " ";
			}
			name = name + context.Context.Stack[level - 1].FileName;
			return name;
		}
	}

}