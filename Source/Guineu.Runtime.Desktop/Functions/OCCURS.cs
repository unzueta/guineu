using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class OCCURS : ExpressionBase
	{
		ExpressionBase m_SearchExpression;
		ExpressionBase m_ExpressionSearched;
		
		override internal void Compile(Compiler comp)
		{
			// Get all parameters
			List<ExpressionBase> param = comp.GetParameterList();

			// LEN() has been called without any parameters
			if (param.Count <= 1)
			{
				throw new ErrorException(ErrorCodes.TooFewArguments);
			}

			// LEN() has been called with more than one parameter
			if (param.Count > 2)
			{
				throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			m_SearchExpression = param[0];
			m_ExpressionSearched = param[1];
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			Variant value = m_SearchExpression.GetVariant(exec);
			if (value.IsNull)
				return new Variant(VariantType.Number, true);
			if(value.Type!=VariantType.Character)
				throw new ErrorException(ErrorCodes.InvalidArgument);

			value = m_ExpressionSearched.GetVariant(exec);
			if (value.IsNull)
				return new Variant(VariantType.Number, true);
			if (value.Type != VariantType.Character)
				throw new ErrorException(ErrorCodes.InvalidArgument);
			return new Variant(GetInt(exec), 10);
		}

		internal override int GetInt(CallingContext exec)
		{
			string cSearchExpression = m_SearchExpression.GetString(exec);
			string cExpressionSearched = m_ExpressionSearched.GetString(exec);

			int nbOccurs = 0;
			int pos = 0;
			while (pos >= 0 && cExpressionSearched.Length >= cSearchExpression.Length)
			{
				// TODO : use IndexOfAny is Unicode set
				pos = cExpressionSearched.IndexOf(cSearchExpression, pos);
				if ( pos >= 0)
				{
					nbOccurs++;
					cExpressionSearched = cExpressionSearched.Substring(pos + cSearchExpression.Length);
				}
			}
			return nbOccurs;
		}

		internal override double GetDouble(CallingContext exec)
		{
			return 1.0*GetInt(exec);
		}

	}

}