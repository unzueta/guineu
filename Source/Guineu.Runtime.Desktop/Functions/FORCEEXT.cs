using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class FORCEEXT : ExpressionBase
	{
		ExpressionBase m_FileName;
		ExpressionBase m_Ext;

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
					m_Ext = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		internal override String GetString(CallingContext context)
		{
			m_FileName.CheckString(context, false);
			m_Ext.CheckString(context, false);

			string fileName = m_FileName.GetString(context);
			string ext = m_Ext.GetString(context);

			return ForceExt(fileName, ext);
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}

		internal string ForceExt(string cFileName, string cExtension)
		{	// maybe we should have used Path class ...
			cFileName = cFileName.Trim();

			int nLastDot, nLastBackSlash;

			if (cFileName.Length == 0)	// if filename is empty return empty
				return cFileName;

			// check if the file has an extension
			nLastDot = cFileName.LastIndexOf('.');

			if (nLastDot < 1)
			{
				// Specify an extension
				return cFileName + "." + cExtension;
			}
			else
			{
				nLastBackSlash = cFileName.LastIndexOf(Path.DirectorySeparatorChar);
				if (nLastDot > nLastBackSlash)
				{
					// remove the extension and force the new one
					return cFileName.Substring(0, nLastDot) + "." + cExtension;
				}
				else
				{	// the point is in the path, just add the extension
					return cFileName + "." + cExtension;
				}
			}

		}
	}
}
