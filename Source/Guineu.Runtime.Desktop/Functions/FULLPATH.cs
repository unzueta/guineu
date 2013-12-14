using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class FULLPATH : ExpressionBase
	{
		ExpressionBase m_FileName;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_FileName = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		internal override String GetString(CallingContext context)
		{
			m_FileName.CheckString(context, false);
			String path = GuineuInstance.FileMgr.FullPath(m_FileName.GetString(context), false);
			if ((GuineuInstance.SetPathHandling & PathHandling.UseUpperCaseNames) > 0)
			{
				path = path.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
			}
			return path;
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}
	}

}