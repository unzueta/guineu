using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Guineu.Core
{
	public class DebugEventArgs : EventArgs
	{
		private Int32 _Line;

		public Int32 Line
		{
			get { return _Line; }
			set { _Line = value; }
		}

		private String _File;

		public String File
		{
			get { return _File; }
		}

		private String _Module;

		public String Module
		{
			get { return _Module; }
		}

		private MemberList _Locals;

		public MemberList Locals
		{
			get { return _Locals; }
		}
	

		public DebugEventArgs(CallingContext context, CodeBlock code, Int32 line)
		{
			_Line = line;
			_Module = context.Stack.ModuleName;
			_File = context.Stack.FileName;
			_Locals = context.Locals;	}
	}
}