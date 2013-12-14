using System;
using System.IO;
using Guineu.Expression;

namespace Guineu.Commands
{
	class AMPERSAND : ICommand
	{
		String source;

		public void Compile(CodeBlock code)
		{
			using (var sr = new StreamReader(code.Reader.BaseStream, code.Encoding))
				source = sr.ReadToEnd();
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			var line = new NormalizedLine(source, exec);
			var gc = new GuineuCompiler();
			var code = gc.Compile(line);
		}
	}
}