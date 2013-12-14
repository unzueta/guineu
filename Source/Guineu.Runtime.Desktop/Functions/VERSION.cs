using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	partial class VERSION : ExpressionBase
	{
		ExpressionBase option;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					option = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		internal override Variant GetVariant(CallingContext context)
		{
			// Get version flag
			Int32 versionType;
			if (option == null)
				versionType = 0;
			else
			{
				versionType = option.GetInt(context);
				if (versionType == 0)
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			switch (versionType)
			{
				case 0:
					return new Variant("Guineu " + getVersion() + " for " + GetPlatform());
				case 1:
					//TODO: Add time stamp and serial number
					return new Variant(
							"Guineu " + getVersion() + " for " + GetPlatform()
					);
				case 2:
					return new Variant(0, 1);
				case 3:
					return new Variant("00");
				case 4:
					return new Variant(getVersion());
				case 5:
					return new Variant("910");
				default:
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}

		}

		private string getVersion()
		{
			return "09.10.0000." + String.Format("{0:0000}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build);
		}

	}
}
