using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class PROGRAM : ExpressionBase
	{
		ExpressionBase m_Level;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break; //  throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_Level = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Int32 Level = 0;

			if (m_Level != null)
			{
				Variant value = m_Level.GetVariant(context);
				if (value.IsNull)
				{
					throw new ErrorException(ErrorCodes.InvalidArgument);
				}

				Level = m_Level.GetInt(context);
			}

			if (Level < 0)
			{
				return new Variant(GetCurrentLevel(context),10);
			}
			else
			{
				return new Variant(GetProgram(context, Level));
			}
		}

		private String GetProgram(CallingContext context, int Level)
		{
			if (Level == 0)
			{
				Level = 1;
			}
			if (Level > context.Context.Stack.Count)
			{
				return "";
			}
			String name = context.Context.Stack[Level - 1].ModuleName;
			if (name.Length == 0)
			{
				name = System.IO.Path.GetFileNameWithoutExtension(context.Context.Stack[Level - 1].FileName);
			}
			return name;
		}

		private Int32 GetCurrentLevel(CallingContext context)
		{
			return context.Context.Stack.Count;
		}

	}

}