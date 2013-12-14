using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Guineu.Core;
using Guineu.Expression;

namespace Guineu
{
	public class StreamHelper
	{
		static public Stream GetStream(String filename)
		{
			FileLocation fxpFile = GuineuInstance.FileMgr.LocateFile(filename);
			Stream fs = fxpFile.Open(
					FileMode.Open,
					FileAccess.Read,
					FileShare.Read
			);
			return fs;
		}
	}

	public interface IFxpFile
	{
		Stream OpenStream();
		CompiledProgram Program { get; }
		ExecutableContext Executable { get; }
	}

	public class FxpFile : IFxpFile
	{
		readonly CompiledProgram program;
		ExecutableContext context;

		public FxpFile(CompiledProgram fxp)
		{
			program = fxp;
		}
		public Stream OpenStream()
		{
			FileLocation fxpFile = GuineuInstance.FileMgr.LocateFile(program.FullName);
			context = fxpFile.Executable; // TODO: Get rid of the Executable here.

			Stream fs = fxpFile.Open(
					FileMode.Open,
					FileAccess.Read,
					FileShare.Read
			);
			return fs;
		}

		public CompiledProgram Program
		{
			get { return program; }
		}

		public ExecutableContext Executable
		{
			get
			{
				if (context == null)
				{
					FileLocation fxpFile = GuineuInstance.FileMgr.LocateFile(program.FullName);
					context = fxpFile.Executable;
				}
				return context;
			}
		}
	}

	public class CompiledProgramFile
	{
		public String FullName { get; set; }
		public Nti Name
		{
			get { return new Nti(Path.GetFileNameWithoutExtension(FullName)); }
		}
		public FxpProcedureList Procedures { get; private set; }
		public FxpClassList Classes { get; private set; }

		readonly IFxpFile fxp;

		public CompiledProgramFile(IFxpFile f)
		{
			fxp = f;
			using (var file = new BinaryReader(f.OpenStream(), GuineuInstance.CurrentCp))
			{
				Procedures = new FxpProcedureList();
				Classes = new FxpClassList();

				var header = new FxpHeader(file);
				if (header.ClassHeaderPos != 0)
					GetClassHeaders(file, header.ClassHeaderPos, header.ClassCount);

				if (header.ProcedureHeaderPos != 0)
					GetProcedureHeaders(file, header.ProcedureHeaderPos, header.ProcedureCount);
			}
		}

		void GetClassHeaders(BinaryReader file, Int32 position, Int32 count)
		{
			file.BaseStream.Seek(position + 0x29, SeekOrigin.Begin);
			for (var classNo = 0; classNo < count; classNo++)
				Classes.Add(classNo, new ClassHeader(file, fxp));
		}

		void GetProcedureHeaders(BinaryReader file, Int32 position, Int32 count)
		{
			file.BaseStream.Seek(position + 0x29, SeekOrigin.Begin);
			for (var proc = 1; proc <= count; proc++)
			{
				var header = new ProcedureHeader(file, fxp);
				if (header.ClassId == 0xFFFF)
					Procedures.Add(proc, header);
				else
				{
					Classes[header.ClassId].Methods.Add(proc, header);
				}
			}
		}
		public CompiledCode ReadCode()
		{
			using (var reader = new BinaryReader(fxp.OpenStream()))
			{
				reader.BaseStream.Seek(0x4E, SeekOrigin.Begin);
				return new CompiledCode(reader, Name);
			}
		}

	}

	/// <summary>
	/// A list of ProcedurHeader that belong to the same class or procedure file.
	/// </summary>
	public class FxpProcedureList : IEnumerable<ProcedureHeader>
	{
		readonly Dictionary<Int32, ProcedureHeader> procedures = new Dictionary<Int32, ProcedureHeader>();

		public void Add(Int32 index, ProcedureHeader header)
		{
			procedures.Add(index, header);
		}
		public ProcedureHeader this[Nti name]
		{
			get
			{
				foreach (var proc in procedures.Values)
					if (proc.Name == name)
						return proc;
				return null;
			}
		}
		public ProcedureHeader this[Int32 i]
		{
			get { return procedures[i]; }
		}

		public IEnumerator<ProcedureHeader> GetEnumerator()
		{
			return procedures.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class FxpClassList : IEnumerable<ClassHeader>
	{
		readonly Dictionary<Int32, ClassHeader> classes = new Dictionary<Int32, ClassHeader>();

		public void Add(Int32 index, ClassHeader header)
		{
			classes.Add(index, header);
		}
		public ClassHeader this[Nti name]
		{
			get
			{
				foreach (var proc in classes.Values)
					if (proc.Name == name)
						return proc;
				return null;
			}
		}
		public ClassHeader this[Int32 i]
		{
			get { return classes[i]; }
		}

		public IEnumerator<ClassHeader> GetEnumerator()
		{
			return classes.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class ProcedureHeader
	{
		public Nti Name { get; private set; }
		public Int32 ClassId { get; private set; }

		readonly IFxpFile fxp;
		readonly Int32 position;

		public ProcedureHeader(BinaryReader reader, IFxpFile f)
		{
			fxp = f;
			Int32 length = reader.ReadInt16();
			Name = new Nti(new String(reader.ReadChars(length)));
			position = reader.ReadInt32() + 0x29;
			reader.ReadUInt16(); // unknown
			ClassId = reader.ReadUInt16();
		}

		public CompiledCode ReadCode()
		{
			using (var reader = new BinaryReader(fxp.OpenStream()))
			{
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
				return new CompiledCode(reader, Name);
			}
		}
	}

	public class ClassHeader
	{
		// TODO: Make Name a MultiLevelNti
		public Nti Name { get; private set; }
		public Nti ParentClass { get; private set; }

		public FxpProcedureList Methods = new FxpProcedureList();

		readonly IFxpFile fxp;
		readonly Int32 position;

		public ClassHeader(BinaryReader reader, IFxpFile f)
		{
			fxp = f;
			Int32 length = reader.ReadInt16();
			Name = new Nti(reader.ReadChars(length));
			length = reader.ReadInt16();
			ParentClass = new Nti(reader.ReadChars(length));
			position = reader.ReadInt32() + 0x29;
			reader.ReadUInt16(); // unknown
		}

		public CompiledCode ReadCode()
		{
			using (var reader = new BinaryReader(fxp.OpenStream()))
			{
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
				return new CompiledCode(reader, Name);
			}
		}
	}

	public class FxpHeader
	{
		public Int32 ProcedureHeaderPos { get; private set; }
		public Int32 ProcedureCount { get; private set; }
		public Int32 ClassHeaderPos { get; private set; }
		public Int32 ClassCount { get; private set; }

		public FxpHeader(BinaryReader br)
		{
			br.BaseStream.Seek(0x1C + 0x0D, SeekOrigin.Begin);
			ProcedureCount = br.ReadInt16();
			ClassCount = br.ReadInt16();
			br.ReadInt32(); // unknown
			ProcedureHeaderPos = br.ReadInt32();
			ClassHeaderPos = br.ReadInt32();
		}
	}

	public class CompiledCode
	{
		readonly List<CodeLine> code = new List<CodeLine>();
		readonly List<String> nameList = new List<String>();
		readonly Nti entityName;

		public List<CodeLine> Code { get { return code; } }
		public List<String> Names { get { return nameList; } }
		public Nti Name { get { return entityName; } }

		public CompiledCode(BinaryReader file, Nti name)
		{
			entityName = name;
			ReadCode(file);
			ReadNameList(file);
		}

		public CompiledCode(IEnumerable<CodeLine> lines, IEnumerable<string> names)
		{
			code.AddRange(lines);
			nameList.AddRange(names);
		}

		void ReadCode(BinaryReader file)
		{
			int currentPosition = 0;
			int codeSize = file.ReadUInt16();
			do
			{
				int length = file.ReadUInt16();
				var cl = new CodeLine
										 {
											 Position = currentPosition,
											 Line = file.ReadBytes(length - 2)
										 };
				code.Add(cl);
				currentPosition += length;
			} while (currentPosition < codeSize);
		}

		void ReadNameList(BinaryReader file)
		{
			int count = file.ReadInt16();
			for (int name = 0; name < count; name++)
			{
				int length = file.ReadInt16();
				// TODO: current Guineu code page or CODEPAGE=AUTO
				// TODO: Remove dependency to GuineuInstance
				char[] strName = GuineuInstance.CurrentCp.GetChars(file.ReadBytes(length));
				nameList.Add(new String(strName));
			}
		}

	}

	/* 
	public class FxpReader
	{
		readonly CompiledProgramFile fxp;

		public FxpReader(string filename)
		{
			fxp = new CompiledProgramFile(new FxpFile(new CompiledProgram(filename)));
		}

		private readonly ExecutableContext hostFile;

		/// <summary>
		/// Returns the EXE, APP or DLL file that contains the FXP file.
		/// </summary>
		public ExecutableContext Executable
		{
			get { return hostFile; }
		}

		internal CodeBlock GetProcedure(int index, int classId)
		{
			var code = fxp.Procedures[index].ReadCode();
			var result = new CodeBlock(code);
			return result;
		}

		internal CodeBlock GetProcedure(string proc)
		{
			return GetProcedure(proc, 0xFFFF);
		}

		internal CodeBlock GetProcedure(string proc, int classId)
		{
			CodeBlock result = null;
			FxpProcedureList procs;
			if (classId == 0xFFFF)
				procs = fxp.Procedures;
			else
				procs = fxp.Classes[classId].Methods;
			var header = procs[new Nti(proc)];
			if (header != null)
			{
				var code = header.ReadCode();
				result = new CodeBlock(code);
			}
			return result;
		}

		internal CodeBlock GetClassConstructor(Nti className)
		{
			var cls = fxp.Classes[className];
			if (cls == null)
				return null;
			var code = cls.ReadCode();
			var result = new CodeBlock(code);
			return result;
		}

		internal Nti GetParentClass(Nti className)
		{
			return fxp.Classes[className].ParentClass;
		}
	} 
	*/

	class ProcedureList
	{
		public Procedure Default { get; set; }
	}

	class Procedure
	{
		public CodeLine2[] Code { get; set; }


		public Procedure(CompiledCode code)
		{
			// Compile procedure
		}
		/// <summary>
		/// Compiles the entire code block
		/// </summary>
		/// <remarks>
		/// Our first approach was to compile only the current line to increase performance.
		/// This works fine if lines are independent, but fails for DO CASE. The ControlFlow
		/// stack is invalid when not the entire structure is compiled at once.
		/// </remarks>
		void Compile(CompiledCode fxp)
		{
			var code = new CodeLine2[fxp.Code.Count];
			for (var line = 0; line < fxp.Code.Count; line++)
			{
				try
				{
					// TODO: add compiler
					// code[line].Command = GuineuInstance.TokenCompiler.Compile(this);
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
		}
	}

	class CodeLine2
	{
		public ICommand Command;
		public Int32 SourceLine;
	}
}


/* Im Speicher:
 * - ProgramFile mit allen einer ProcedureList und allen Klassennamen
 * - Function<Stream> liefert einen Stream für die FXP Datei. Wird mit Using geöffnet.
 * 
 * Suche nach Klassen und Prozeduren läuft über die Primärliste.
 * Beim Anfordern des Codes einer Prozedur wird in einem Cache geschaut. Gibt es die
 * Prozedur noch nicht, wird sie geladen. Dazu dient die Liste mit den Verweisen
 * bzw. die Default-Angabe
 * 
 * Bei der Suche nach einer Klasse wird diese ebenfalls als ProzedurListe geladen.
 * Default der Klasse ist der Konstruktur.
 * 
 * Prozeduren werden immer vollst#ndig im Speicher gehalten. Es wird also erstmal
 * nicht zeilenweise kopiert.
 */








