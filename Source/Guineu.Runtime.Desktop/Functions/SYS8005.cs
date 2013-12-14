using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using Guineu.Expression;

namespace Guineu.Functions

{
    class SYS8005 : ISys
	{
		/// <summary>
		/// Provides access to additional functions of the Microsoft Compact Engine
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			// Get the command
			var cmd = param[1].GetString(context).ToLower(System.Globalization.CultureInfo.InvariantCulture);
			var retVal = "";

			switch (cmd)
			{
				case "upgrade":
					retVal = UpgradeDatebase(context, param);
					break;
				case "create":
					retVal = CreateDatabase(context, param);
					break;
				default:
					break;
			}

			return retVal;
		}
		static String CreateDatabase(CallingContext context, List<ExpressionBase> param)
		{
			switch (param.Count)
			{
				case 0:
				case 1:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					var connectionStr = param[2].GetString(context);
					var engine = new SqlCeEngine {LocalConnectionString = connectionStr};
			        engine.CreateDatabase();
					return "";
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		static String UpgradeDatebase(CallingContext context, List<ExpressionBase> param)
		{
			switch (param.Count)
			{
				case 0:
				case 1:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					String connectionStr = param[2].GetString(context);
					var engine = new SqlCeEngine {LocalConnectionString = connectionStr};
			        engine.Upgrade(connectionStr);
					return "";
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}
	}

}