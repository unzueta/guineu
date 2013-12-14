using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class LEN : ExpressionBase
	{
		ExpressionBase m_String;

		override internal void Compile(Compiler comp)
		{
			// Get all parameters
			List<ExpressionBase> param = comp.GetParameterList();

			// LEN() has been called without any parameters
			if (param.Count == 0)
			{
				throw new ErrorException(ErrorCodes.TooFewArguments);
			}

			// LEN() has been called with more than one parameter
			if (param.Count > 1)
			{
				throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			m_String = param[0];
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			Variant value = m_String.GetVariant(exec);
			if(value.IsNull)
					return new Variant(VariantType.Number,true);
			else 
			switch (value.Type)
			{
				case VariantType.Character:
					return new Variant(value.Length(),10);
				default:
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}
		}

		internal override int GetInt(CallingContext exec)
		{
			int length = m_String.GetStringLength(exec);
			return length;
		}

		internal override double GetDouble(CallingContext exec)
		{
			double length = m_String.GetStringLength(exec);
			return length;
		}

	}

}