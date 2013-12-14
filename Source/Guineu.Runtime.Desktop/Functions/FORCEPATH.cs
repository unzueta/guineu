using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class FORCEPATH : ExpressionBase
	{
		ExpressionBase m_FileName;
		ExpressionBase m_Path;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					m_FileName = param[0];
					m_Path = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		internal override String GetString(CallingContext context)
		{
			m_FileName.CheckString(context, false);
			m_Path.CheckString(context, false);

			string fileName = m_FileName.GetString(context);
			string path = m_Path.GetString(context);

			return ForcePath(fileName, path);
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}

		internal string ForcePath(string cFileName, string cPath)
		{	// maybe we should have used Path class ...
			cFileName = cFileName.Trim();
			cPath = cPath.Trim();

			FileInfo fi = new FileInfo(cFileName);
			cFileName = fi.Name; 

			if (cPath.Length == 0)
				return cFileName;
			else if (cPath[cPath.Length - 1] == Path.DirectorySeparatorChar)
				return cPath + cFileName;
			else
				return cPath + Path.DirectorySeparatorChar.ToString() + cFileName;
		}
	}
}
