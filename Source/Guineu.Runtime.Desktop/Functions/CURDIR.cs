using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Functions
{
	partial class CURDIR : ExpressionBase
	{
		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetString(context));
			return retVal;
		}

		internal override string GetString(CallingContext context)
		{
			String curDir = GuineuInstance.FileMgr.CurrentDirectory;
			curDir = curDir.Substring(Path.GetPathRoot(curDir).Length-1);
			if ((GuineuInstance.SetPathHandling & PathHandling.UseUpperCaseNames) > 0)
			{
				curDir = curDir.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
			}
			return curDir;
		}

	}

}