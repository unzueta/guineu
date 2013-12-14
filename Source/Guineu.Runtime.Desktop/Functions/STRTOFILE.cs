using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;
using Guineu.Gui;
using Guineu.Properties;

namespace Guineu.Functions
{
	class STRTOFILE : ExpressionBase
	{
		ExpressionBase dataExpression;
		ExpressionBase file;
		ExpressionBase flags;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					dataExpression = param[0];
					file = param[1];
					break;
				case 3:
					dataExpression = param[0];
					file = param[1];
					flags = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetInt(context), 10);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			string fileName = file.GetString(context);
			string data = dataExpression.GetString(context);
			fileName = GuineuInstance.FileMgr.MakePath(fileName);

			// The third parameter with flags can be a numeric or a boolean value
			Variant allFlags;
			if (flags == null)
				allFlags = new Variant(false);
			else
				allFlags = flags.GetVariant(context);

			// Append text to the end of the file or overwrite existing content?
			Boolean append = false;
			if (allFlags.Type == VariantType.Logical)
				append = allFlags;

			// TODO : Test SET SAFETY and FileExists only when param count < 3
			// TODO : Fred : have a look on system localized strings and standard messages...

			if (GuineuInstance.Set.Safety.Value && File.Exists(fileName))
			{
				if (DialogResult.No == GuineuInstance.WinMgr.MessageBox(
												fileName + '\n' + Resources.FileExistsOverwrite,
												Resources.App_Title,
												MessageBoxButtons.YesNo,
												MessageBoxIcon.Question))
					return 0;
			}

			// TODO: Evaluate last parameter for Unicode flag
			//       If no parameter has been specified, open existing file to determine encoding
			int retVal = GuineuInstance.UseUnicode ? WriteUnicode(data, fileName, append) : WriteAnsi(data, fileName, append);
			return retVal;
		}


		static private int WriteAnsi(string data, string name, bool append)
		{
			Int32 cnt;
			try
			{
				using (Stream fs = GuineuInstance.FileMgr.Open(name, append ? FileMode.OpenOrCreate : FileMode.Create))
				using (var wr = new BinaryWriter(fs))
				{
					if (append)
						fs.Seek(0, SeekOrigin.End);
					byte[] values = GuineuInstance.CurrentCp.GetBytes(data);
					Int64 pos = (int)fs.Position;
					wr.Write(values);
					cnt = (Int32)(fs.Position - pos);
				}
			}
			catch
			{
				cnt = 0;
			}
			return cnt;
		}

		static private int WriteUnicode(string data, string name, bool append)
		{
			Int32 cnt;
			try
			{
				using (var writer = new StreamWriter(name))
				{
					if (append)
						writer.BaseStream.Seek(0, SeekOrigin.End);
					Int64 pos = (int)writer.BaseStream.Position;
					writer.Write(data);
					cnt = (Int32)(writer.BaseStream.Position - pos);
				}
			}
			catch
			{
				cnt = 0;
			}
			return cnt;
		}


	}

}