using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SOUNDEX : ExpressionBase
	{
		ExpressionBase value;

		override internal void Compile(Compiler comp)
		{
			// Get all parameters
			List<ExpressionBase> param = comp.GetParameterList();

			// ALLTRIM() has been called without any parameters
			if (param.Count == 0)
			{
				throw new ErrorException(ErrorCodes.TooFewArguments);
			}

			// ALLTRIM() has been called with more than one parameter
			if (param.Count > 1)
			{
				throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			value = param[0];
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (value.CheckString(context, true))
				return new Variant(VariantType.Character, true);

			return new Variant(value.GetString(context).Trim());
		}

		internal override string GetString(CallingContext exec)
		{
			string s = value.GetString(exec);
			if (s != null)
			{
				s = SoundexCodeConverter(s);
			}
			return s;
		}
		internal string SoundexCodeConverter(string s)
		{
			String word = s.ToUpper();
			var soundexCode = new StringBuilder();

			int wordLength = word.Length;

			soundexCode.Append(word.Substring(0, 1));

			for (int i = 1; i < wordLength; i++)
			{
				String transformedChar = Transform(word.Substring(i, 1));
				if (!transformedChar.Equals(soundexCode.ToString().Substring(soundexCode.Length - 1)))
				{
					if (!transformedChar.Equals(" ") && !transformedChar.Equals("S"))
					{
						soundexCode.Append(transformedChar);
					}
				}
			}
			soundexCode.Append("0000");
			return soundexCode.ToString().Substring(0, 4);
		}

		internal string Transform(string s)
		{
			switch (s)
			{
				case "H":
				case "W":
					return "S";
				case "B":
				case "F":
				case "P":
				case "V":
					return "1";
				case "C":
				case "G":
				case "J":
				case "K":
				case "Q":
				case "S":
				case "X":
				case "Z":
					return "2";
				case "D":
				case "T":
					return "3";
				case "L":
					return "4";
				case "M":
				case "N":
					return "5";
				case "R":
					return "6";
			}
			return " ";
		}

	}

}




