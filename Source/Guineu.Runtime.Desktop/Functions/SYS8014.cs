using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	/// <summary>
	/// Raise the click event when the user presses Enter while the focus is on a button.
	/// </summary>
	class SYS8014 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			KnownNti cmd = param[1].ToNti(context).ToKnownNti();
			String retVal = "";

			switch (cmd)
			{
				case KnownNti.ButtonClickOnEnter:
					retVal = ButtonClickOnEnter(context, param);
					break;
				default:
					break;
			}

			return retVal;
		}

		private static string ButtonClickOnEnter(CallingContext context, IList<ExpressionBase> param)
		{
			String retVal = GuineuInstance.Options.ButtonClickOnEnter ? "1" : "0";
			if (param.Count >= 3)
				GuineuInstance.Options.ButtonClickOnEnter = param[2].GetBool(context);
			return retVal;
		}
	}
}