using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Guineu.Commands;
using Guineu.Expression;
using Guineu.Core;

namespace Guineu
{
	/// <summary>
	/// A CodeBlock is the smallest build block above a single line. Control structures
	/// such as loops, IF statements, etc. must lie completely within one CodeBlock. They
	/// cannot start in one CodeBlock and end in another one.
	/// 
	/// A CodeBlock can exist in tokenized form and in compiled form. To execute code
	/// it must be available in compiled form.
	/// </summary>
	sealed public class CodeBlock : IDisposable
	{
		internal CodeReader Reader;
		internal int CurrentLine;
		internal FlowControl ControlFlowStack = new FlowControl();
		internal CompiledProgram FileContext;
		readonly ExecutableContext storage;

		//readonly CodeLine[] code;
		//readonly List<string> nameList = new List<string>();
		readonly CompiledCode cc;
		Boolean compiled;

		// TODO: Evaluate encoding from FXP file
		public Encoding Encoding { get { return GuineuInstance.CurrentCp; } }

		//internal void SetContext(ExecutableContext sourceExecutable, CompiledProgram sourceFile)
		//{
		//    storage = sourceExecutable;
		//    FileContext = sourceFile;
		//}

		internal ExecutableContext Executable
		{
			get { return storage; }
		}

		//internal CodeBlock(CodeLine[] code, List<string> nameList)
		//{
		//    this.nameList = nameList;
		//    this.code = code;
		//}

		// TODO: Finish
		internal CodeBlock(CompiledCode code, ExecutableContext sourceExecutable, CompiledProgram sourceFile)
		{
			cc = code;
			storage = sourceExecutable;
			FileContext = sourceFile;
		}

		internal Nti Name { get { return cc.Name; } }

		internal ICommand Code(int line)
		{
			if (!compiled)
				Compile();
			return cc.Code[line].Compiled;
		}

		/// <summary>
		/// Compiles the entire code block
		/// </summary>
		/// <remarks>
		/// Our first approach was to compile only the current line to increase performance.
		/// This works fine if lines are independent, but fails for DO CASE. The ControlFlow
		/// stack is invalid when not the entire structure is compiled at once.
		/// </remarks>
		void Compile()
		{
			if (!compiled)
			{
				CodeReader tmpReader = Reader;
				int tmpCurrentLine = CurrentLine;
				for (Int32 line = 0; line < cc.Code.Count; line++)
					if (cc.Code[line].Compiled == null)
					{
						Goto(line);
						try
						{
							cc.Code[line].Compiled = GuineuInstance.TokenCompiler.Compile(this);
						}
						catch (ErrorException e)
						{
							ErrorAction action = ExecutionPath.Error(e.Error, e.Param, line);
							if (action == ErrorAction.Cancel)
							{
								GuineuInstance.Quit();
								throw;
							}
						}
					}
				Reader = tmpReader;
				CurrentLine = tmpCurrentLine;
			}
			compiled = true;
		}

		public int LineCount
		{
			get { return cc.Code.Count; }
		}

		public int NameCount
		{
			get { return cc.Names.Count; }
		}

		internal int GetLineAtPosition(int position)
		{
			int retVal = -2;
			for (int curLine = 0; curLine < cc.Code.Count; curLine++)
			{
				if (cc.Code[curLine].Position == position)
				{
					retVal = curLine;
					break;
				}
			}
			return retVal;
		}

		public void Goto(int line)
		{
			CurrentLine = line;
			var ms = new MemoryStream(cc.Code[line].Line);
			Reader = new CodeReader(ms);
		}

		/// <summary>
		/// Returns a Name from the Name index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetName(int index)
		{
			return cc.Names[index];
		}

		public Nti GetNti(int index)
		{
			return new Nti(cc.Names[index]);
		}


		#region IDisposable Members

		void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
				if (Reader != null)
				{
					Reader.Close();
				}
			}
			// free native resources
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}

	/// <summary>
	/// Maintains the starting points of loops during compilation. We make the assumption here that
	/// Code is always compiled in execution order. A FOR statement must be compiled before the ENDFOR
	/// statement. Nested loops must be completely compiled inbetween.
	/// </summary>
	class FlowControlEntry
	{
		internal int Line;
		internal Int32 AlternativeLine;
		public enum Types { For, DoWhile, If, Scan, DoCase };
		internal Types Type;
	}

	class FlowControl
	{
		readonly Stack<FlowControlEntry> stack = new Stack<FlowControlEntry>();

		public void Push(int line, FlowControlEntry.Types type)
		{
			var se = new FlowControlEntry { Line = line, Type = type };
			stack.Push(se);
		}

		public void Push(int line, FlowControlEntry.Types type, Int32 alternativeLine)
		{
			var se = new FlowControlEntry
									 {
										 Line = line,
										 Type = type,
										 AlternativeLine = alternativeLine
									 };
			stack.Push(se);
		}

		public int Pop(FlowControlEntry.Types type)
		{
			FlowControlEntry se = stack.Pop();
			if (se.Type != type)
			{
				throw new ErrorException(ErrorCodes.Syntax);
			}
			return se.Line;
		}
		public FlowControlEntry PeekLastLoop()
		{
			FlowControlEntry found = null;
			var tmpStack = new Stack<FlowControlEntry>();
			while (stack.Count > 0)
			{
				FlowControlEntry fce = stack.Pop();
				tmpStack.Push(fce);
				if (fce.Type == FlowControlEntry.Types.DoWhile
						|| fce.Type == FlowControlEntry.Types.For
						|| fce.Type == FlowControlEntry.Types.Scan)
				{
					found = fce;
					break;
				}
			}
			while (tmpStack.Count > 0)
				stack.Push(tmpStack.Pop());
			return found;
		}
	}


	public class CodeReader : BinaryReader
	{
		public CodeReader(Stream s) : base(s) { }

		internal Token ReadToken()
		{
			return (Token)ReadByte();
		}

		internal Token PeekToken()
		{
			var tok = (Token)ReadByte();
			BaseStream.Seek(-1, SeekOrigin.Current);
			return tok;
		}

		internal Token PeekToken(Token defaultToken)
		{
			if (BaseStream.Position == BaseStream.Length)
				return defaultToken;
			return PeekToken();
		}

		internal SetToken PeekSetToken()
		{
			var tok = (SetToken)ReadByte();
			BaseStream.Seek(-1, SeekOrigin.Current);
			return tok;
		}
	}

	/// <summary>
	/// Contains procedures and classes. This is the in-memory compiled version, not
	/// the one using FXP code.
	/// </summary>
	public class CompiledProgram
	{
		readonly String name;
		readonly String fullName;

		readonly CodeBlockCollection procedures;
		readonly Dictionary<Nti, Class> classes;
		readonly CompiledProgramFile code;
		readonly IFxpFile fxp;

		public String Name
		{
			get { return name.ToUpper(System.Globalization.CultureInfo.InvariantCulture); }
		}

		public String FullName
		{
			get { return fullName.ToUpper(System.Globalization.CultureInfo.InvariantCulture); }
		}

		// Every FXP can be called using its Name, eg. DO sample executes the anonymous code
		// at the top of sample.fxp. This code must be present in an FXP, even it is empty.
		public CompiledProgram(string filename)
		{
			fullName = GuineuInstance.FileMgr.FullPath(filename, false);
			name = Path.GetFileNameWithoutExtension(filename);
			// reader = new FxpReader(filename);
			fxp = new FxpFile(this);
			code = new CompiledProgramFile(fxp); // TODO: Remove back reference to this class itself.
			procedures = new CodeBlockCollection(code.Procedures, fxp.Executable, fxp.Program);
			classes = new Dictionary<Nti, Class>();
		}

		/// <summary>
		/// Locates a code block given the Name of a procedure.
		/// </summary>
		/// <param name="procName"></param>
		/// <returns>CodeBlock or null, if code is not present.</returns>
		public CodeBlock Locate(Nti procName)
		{
			return procedures[procName];
		}

		public CodeBlock MainProgram()
		{
			return new CodeBlock(code.ReadCode(), fxp.Executable, fxp.Program);
		}

		public Class LocateClass(Nti nti)
		{
			if (classes.ContainsKey(nti))
				return classes[nti];

			Class c = null;
			foreach (var cl in code.Classes)
				if (cl.Name == nti)
					c = new Class(new CodeBlock(cl.ReadCode(), fxp.Executable, fxp.Program), cl.ParentClass, cl.Methods);
			
			if(c!= null)
			classes.Add(nti, c);

			if (classes.ContainsKey(nti))
				return classes[nti];

			return null;
		}

		public CodeBlock GetProcedure(int index)
		{
			var procedure = code.Procedures[index];
			return Locate(procedure.Name);
		}

		public Nti GetProcedureName(int index)
		{
			return code.Procedures[index].Name;
		}
	}

	public class Class
	{
		readonly CodeBlockCollection methods;

		public Nti ParentClass;
		public CodeBlock Constructor;

		public CodeBlockCollection Methods { get { return methods; } }  // TODO: use an enumerator here?

		public Class(CodeBlock code, Nti name, FxpProcedureList methods)
		{
			Constructor = code;
			ParentClass = name;
			this.methods = new CodeBlockCollection(methods, code.Executable, code.FileContext);
		}
	}

	public class CodeBlockCollection
	{
		readonly FxpProcedureList procs;
		readonly Dictionary<Nti, CodeBlock> procedures = new Dictionary<Nti, CodeBlock>();

		readonly ExecutableContext executable;
		readonly CompiledProgram prg;

		public CodeBlockCollection(FxpProcedureList procedureList, ExecutableContext exe, CompiledProgram cp)
		{
			procs = procedureList;
			executable = exe;
			prg = cp;
		}

		public CodeBlock this[Nti name]
		{
			get
			{
				if (procedures.ContainsKey(name))
					return procedures[name];

				foreach (var header in procs)
				{
					if (header.Name == name)
					{
						var proc = new CodeBlock(header.ReadCode(), executable, prg);
						procedures.Add(name, proc);
					}
				}

				if (procedures.ContainsKey(name))
					return procedures[name];

				return null;
			}
		}
		public CodeBlock this[Int32 index]
		{
			get
			{
				var name = procs[index].Name;
				return this[name];
			}
		}
	}

	// TODO: Make struct (?). Was before
	public class CodeLine
	{
		internal int Position;
		internal byte[] Line;
		internal ICommand Compiled;

	}


	// ((...) list of procedure with Name and offset
	struct ProcedureListEntry
	{
		internal string Name;
		internal int Position;
		internal CodeBlock Code;
		internal int ClassId;
	}

	struct ClassListEntry
	{
		internal Nti Name;
		internal Nti ParentClass;
		internal int Position;
		internal CodeBlock Constructor;
	}

	//partial class FxpReader : IDisposable
	//{
	//    readonly String procName;
	//    readonly BinaryReader file;

	//    int procedures;
	//    int classes;
	//    ProcedureListEntry[] procedureList;
	//    ClassListEntry[] classList;

	//    public FxpReader(string filename)
	//    {
	//        FileLocation fxpFile = GuineuInstance.FileMgr.LocateFile(filename);
	//        hostFile = fxpFile.Executable;
	//        Stream fs = fxpFile.Open(
	//            FileMode.Open,
	//            FileAccess.Read,
	//            FileShare.Read
	//        );
	//        file = new BinaryReader(fs);
	//        procName = Path.GetFileNameWithoutExtension(filename).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
	//        LoadFxp();
	//    }

	//    #region Public methods and properties

	//    private readonly ExecutableContext hostFile;

	//    /// <summary>
	//    /// Returns the EXE, APP or DLL file that contains the FXP file.
	//    /// </summary>
	//    public ExecutableContext Executable
	//    {
	//        get { return hostFile; }
	//    }

	//    #endregion

	//    private void LoadFxp()
	//    {
	//        file.BaseStream.Seek(0x1C+0x0D, SeekOrigin.Begin);
	//        procedures = file.ReadInt16();
	//        classes = file.ReadInt16();
	//        GetProcedureHeaders();
	//        GetClassHeaders();
	//    }

	//    private void GetProcedureHeaders()
	//    {
	//        // New procedure list
	//        procedureList = new ProcedureListEntry[procedures+1];

	//        procedureList[0].ClassId = 0xFFFF;
	//        procedureList[0].Position = 0x4E;
	//        procedureList[0].Name = "";

	//        // We create a list of procedure names
	//        file.BaseStream.Seek(0x1C + 0x15, SeekOrigin.Begin);
	//        int position = file.ReadInt32();
	//        if (position > 0)
	//        {
	//            file.BaseStream.Seek(position+0x29, SeekOrigin.Begin);
	//            for (int proc = 1; proc <= procedures; proc++)
	//            {
	//                int length = file.ReadInt16();
	//                procedureList[proc].Name = new String(file.ReadChars(length)).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
	//                procedureList[proc].Position = file.ReadInt32() + 0x29;
	//                file.ReadUInt16(); // unknown
	//                procedureList[proc].ClassId = file.ReadUInt16();
	//            }
	//        }

	//        // if the first procedure is named like the file and the FXP doesn't contain its own code
	//        // then we treat the first procedure as the main program.
	//        if (procedures > 0)
	//        {
	//            if (String.Compare(procedureList[1].Name, procName, StringComparison.CurrentCultureIgnoreCase) == 0)
	//            {
	//                file.BaseStream.Seek(procedureList[procedures].Position, SeekOrigin.Begin);
	//                int length = file.ReadInt16();
	//                if (length == 3)
	//                {
	//                    procedureList[0].Position = procedureList[1].Position;
	//                }
	//            }
	//        }
	//    }

	//    private void GetClassHeaders()
	//    {
	//        classList = new ClassListEntry[classes];

	//        // todo: FXP Header structure
	//        file.BaseStream.Seek(0x1C + 0x19, SeekOrigin.Begin);
	//        int position = file.ReadInt32();
	//        if (position > 0)
	//        {
	//            file.BaseStream.Seek(position + 0x29, SeekOrigin.Begin);
	//            for (int classNo = 0; classNo < classes; classNo++)
	//            {
	//                int length = file.ReadInt16();
	//                classList[classNo].Name = new Nti(file.ReadChars(length));
	//                length = file.ReadInt16();
	//                classList[classNo].ParentClass = new Nti(file.ReadChars(length));
	//                classList[classNo].Position = file.ReadInt32() + 0x29;
	//                file.ReadUInt16(); // unknown
	//            }
	//        }
	//    }

	//    internal CodeBlock GetProcedure(int index, int classId)
	//    {
	//        CodeBlock result = null;
	//        if (procedureList[index].ClassId == classId)
	//        {
	//            if (procedureList[index].Code == null)
	//            {
	//                file.BaseStream.Seek(procedureList[index].Position, SeekOrigin.Begin);
	//                procedureList[index].Code = ReadCodeBlock();
	//            }
	//            result = procedureList[index].Code;
	//            result.ClassId = procedureList[index].ClassId;
	//        }
	//        return result;
	//    }

	//    internal String GetProcedureName(int index, int classId)
	//    {
	//        String result = "";
	//        if (procedureList[index].ClassId == classId)
	//        {
	//            result = procedureList[index].Name;
	//        }
	//        return result;
	//    }

	//    internal CodeBlock GetProcedure(string proc)
	//    {
	//        return GetProcedure(proc, 0xFFFF);
	//    }

	//    internal CodeBlock GetProcedure(string proc, int classId)
	//    {
	//        CodeBlock result = null;
	//        for (int procNo = procedures; procNo >= 0; procNo--)
	//        {
	//            if (procedureList[procNo].ClassId == classId)
	//            {
	//                if (String.Compare(procedureList[procNo].Name, proc, StringComparison.CurrentCultureIgnoreCase) == 0)
	//                    //if (String.Compare(m_ProcedureList[Proc].Name, ProcName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
	//                {
	//                    if (procedureList[procNo].Code == null)
	//                    {
	//                        file.BaseStream.Seek(procedureList[procNo].Position, SeekOrigin.Begin);
	//                        procedureList[procNo].Code = ReadCodeBlock();
	//                    }
	//                    result = procedureList[procNo].Code;
	//                    result.ClassId = procedureList[procNo].ClassId;
	//                    break;
	//                }
	//            }
	//        }
	//        return result;
	//    }

	//    internal CodeBlock GetClassConstructor(Nti className)
	//    {
	//        CodeBlock result = null;
	//        for (int classNo = classes-1; classNo >= 0; classNo--)
	//        {
	//            if (classList[classNo].Name == className)
	//            {
	//                if (classList[classNo].Constructor == null)
	//                {
	//                    file.BaseStream.Seek(classList[classNo].Position, SeekOrigin.Begin);
	//                    classList[classNo].Constructor = ReadCodeBlock();
	//                }
	//                result = classList[classNo].Constructor;
	//                result.ClassId = classNo;
	//                break;
	//            }
	//        }
	//        return result;
	//    }

	//    internal Nti GetParentClass(Nti className)
	//    {
	//        for (int classNo = classes-1; classNo >= 0; classNo--)
	//        {
	//            if (classList[classNo].Name == className)
	//            {
	//                return classList[classNo].ParentClass;
	//            }
	//        }
	//        return new Nti();
	//    }

	//    private CodeBlock ReadCodeBlock()
	//    {
	//        int currentPosition = 0;
	//        var code = new List<CodeLine>();
	//        int codeSize = file.ReadUInt16();
	//        do
	//        {
	//            int length = file.ReadUInt16();
	//            var cl = new CodeLine
	//                         {
	//                             Position = currentPosition, 
	//                             Line = file.ReadBytes(length - 2)
	//                         };
	//            code.Add(cl);
	//            currentPosition += length;
	//        } while (currentPosition < codeSize);

	//        List<string> nameList = ReadNameList();

	//        return new CodeBlock(code.ToArray(), nameList);
	//    }

	//    private List<string> ReadNameList()
	//    {
	//        int count = file.ReadInt16();
	//        var nameList = new List<string>();
	//        for (int name = 0; name < count; name++)
	//        {
	//            int length = file.ReadInt16();
	//            // (...) current Guineu code page or CODEPAGE=AUTO
	//            char[] strName = GuineuInstance.CurrentCp.GetChars(file.ReadBytes(length));
	//            nameList.Add(new String(strName));
	//        }
	//        return nameList;
	//    }

	//    protected virtual void Dispose(bool disposing)
	//    {
	//        if (disposing)
	//        {
	//            file.BaseStream.Close();
	//        }
	//    }

	//    public void Dispose()
	//    {
	//        Dispose(true);
	//        GC.SuppressFinalize(this);
	//    }
	//}
}