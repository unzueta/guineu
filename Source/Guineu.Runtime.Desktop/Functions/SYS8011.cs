using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS8011 : ISys
	{
		/// <summary>
		/// Changes internal debug options
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			KnownNti cmd = param[1].ToNti(context).ToKnownNti();
			String retVal = "";

			switch (cmd)
			{
				case KnownNti.LogRecordGather:
					retVal = ChangeLogRecordGather(context, param);
					break;
				default:
					break;
			}

			return retVal;
		}

		private static string ChangeLogRecordGather(CallingContext context, List<ExpressionBase> param)
		{
			String retVal = GuineuInstance.DebugLogRecordGather ? "1" : "0";
			if (param.Count >= 3)
				GuineuInstance.DebugLogRecordGather = param[2].GetBool(context);
			return retVal;
		}
	}
}