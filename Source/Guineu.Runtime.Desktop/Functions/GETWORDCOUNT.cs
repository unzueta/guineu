using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class GETWORDCOUNT : ExpressionBase
	{
		ExpressionBase m_String;
		ExpressionBase m_Separators;

		override internal void Compile(Compiler comp)
		{
			// Get all parameters
			List<ExpressionBase> param = comp.GetParameterList();

			// LEN() has been called without any parameters
			switch(param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_String = param[0];
					break;
				case 2:
					m_String = param[0];
					m_Separators = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			m_String.CheckString(exec, false);
			if(m_Separators!=null)
				m_Separators.CheckString(exec, false);

			Variant value = m_String.GetVariant(exec);

			return new Variant(GetInt(exec), 10);
		}

		internal override int GetInt(CallingContext exec)
		{
			string str = m_String.GetString(exec);
			string separators ;
			if (m_Separators != null)
				separators = m_Separators.GetString(exec);
			else
				separators = " \n\t"; // default sep are space, tab and newline. Help says that CR is a sep too, but in facto it isn't

			int number=0;

			char[] Separators=separators.ToCharArray();

			string[] result ;
			// TODO : add the StringSplitOptions.RemoveEmptyEntries for the desktop. and do it by hand for the compact
			result = str.Split(Separators);
			number = result.GetLength(0);
			return number;
		}

		internal override double GetDouble(CallingContext exec)
		{
			int number = GetInt(exec);
			return 1.0*number;
		}

	}

}