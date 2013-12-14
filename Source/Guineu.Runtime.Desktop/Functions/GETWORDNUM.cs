using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class GETWORDNUM : ExpressionBase
	{
		ExpressionBase m_String;
		ExpressionBase m_Index;
		ExpressionBase m_Separators;

		override internal void Compile(Compiler comp)
		{
			// Get all parameters
			List<ExpressionBase> param = comp.GetParameterList();

			// LEN() has been called without any parameters
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					m_String = param[0];
					m_Index = param[1];
					break;
				case 3:
					m_String = param[0];
					m_Index = param[1];
					m_Separators = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			m_String.CheckString(exec, false);
			if (m_Separators != null)
				m_Separators.CheckString(exec, false);

			if(m_Index.GetInt(exec)<=0)
				throw new ErrorException(ErrorCodes.InvalidArgument);

			return new Variant(GetInt(exec), 10);
		}

		internal override string GetString(CallingContext exec)
		{
			string str = m_String.GetString(exec);
			string separators;
			if (m_Separators != null)
				separators = m_Separators.GetString(exec);
			else
				separators = " \n\t"; // default sep are space, tab and newline. Help says that CR is a sep too, but in facto it isn't

			int index = m_Index.GetInt(exec);

			if (index < 1)
				index = 1;

			int number;

			char[] separator = separators.ToCharArray();

			// TODO : add the StringSplitOptions.RemoveEmptyEntries for the desktop. and do it by hand for the compact
			string[] result = str.Split(separator);
			number = result.GetLength(0);

			if (index > number)
				return string.Empty;

			return result[index - 1];

		}

	}

}