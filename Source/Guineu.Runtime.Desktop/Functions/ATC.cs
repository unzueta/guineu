using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class ATC : ExpressionBase
	{
		ExpressionBase m_String;
		ExpressionBase m_StringSearched;
		ExpressionBase m_Occurrence;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					m_String = param[0];
					m_StringSearched = param[1];
					break;
				case 3:
					m_String = param[0];
					m_StringSearched = param[1];
					m_Occurrence = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (m_String.CheckString(context, false))
				return new Variant(VariantType.Number, true);
			if (m_StringSearched.CheckString(context, false))
				return new Variant(VariantType.Number, true);

			return new Variant(GetInt(context),10);
		}

		internal override int GetInt(CallingContext context)
		{
			Int32 count;
			if (m_Occurrence == null)
			{
				count = 1;
			}
			else
			{
				count = m_Occurrence.GetInt(context);
			}

			String value = m_StringSearched.GetString(context);
			String search = m_String.GetString(context);

			Int32 retVal = 0;
			for (Int32 loop = 0; loop < count; loop++)
			{
				retVal = value.IndexOf(search, retVal, value.Length-retVal, StringComparison.InvariantCultureIgnoreCase);
				retVal = retVal + 1;
				if (retVal == 0)
				{
					break;
				}
			}
			return retVal;
		}

		
	}

}