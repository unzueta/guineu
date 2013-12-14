using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

// TOTO : compact and Chris mail to fix

namespace Guineu.Functions
{
	class FILETOSTR : ExpressionBase
	{
		ExpressionBase file;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					file = param[0];
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
			string retVal;
			string fileName = file.GetString(context);
			if (GuineuInstance.UseUnicode)
				retVal = ReadUnicode(fileName);
			else
				retVal = ReadAnsi(fileName);
			return retVal;
		}

		private static string ReadAnsi(string name)
		{
			string retVal;
			using (Stream fs = GuineuInstance.FileMgr.Open(name, FileMode.Open))
			using (var br = new BinaryReader(fs))
			{
				var length = (int)br.BaseStream.Length;
				byte[] values = br.ReadBytes(length);
				char[] charValue = GuineuInstance.CurrentCp.GetChars(values);
				retVal = new String(charValue);
			}
			return retVal;
		}

		private static string ReadUnicode(string name)
		{
			string retVal;
			using (Stream fs = GuineuInstance.FileMgr.Open(name, FileMode.Open))
			using (var reader = new StreamReader(fs, true))
			{
				retVal = reader.ReadToEnd();
			}
			return retVal;
		}


	}

}