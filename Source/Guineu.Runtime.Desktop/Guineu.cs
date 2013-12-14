using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using Guineu.Expression;
using Guineu.Functions;
using Guineu.ObjectEngine;
using Guineu.Core;
using Guineu.Data;
using Guineu.Gui;
using Guineu.Util;

namespace Guineu
{
	// TODO: Change the static class into a single static property
	public static partial class GuineuInstance
	{
		static Boolean initialized;

		static DataSessionList datasessions;
		static TokenCompiler tokenCompilerField;
		static Options optionsField;
		internal static Options Options
		{
			get { return optionsField; }
		}

		internal static Stream ProfileStream { get; set; }

		static ExecutableContext storage;
		static internal ExecutableContext ServerExecutable
		{
			get { return storage; }
		}

		public static bool UseUnicode { get; set; }
		public static SptConnectionManager Connections { get; set; }
		static public Boolean DebugLogRecordGather { get; set; }
		public static Boolean CallDebugger { get; set; }
		public static Boolean DebugMode { get; set; }

		static public event EventHandler<DebugEventArgs> DebugBefore;
		static public event EventHandler<DebugEventArgs> DebugAfter;

		static void OnDebugAfter(CallingContext sender, DebugEventArgs e)
		{
			EventHandler<DebugEventArgs> handler = DebugAfter;
			if (handler != null)
			{
				handler(sender, e);
			}
		}

		static void OnDebugBefore(CallingContext sender, DebugEventArgs e)
		{
			EventHandler<DebugEventArgs> handler = DebugBefore;
			if (handler != null)
			{
				handler(sender, e);
			}
		}

		static internal void RaiseDebugBefore(CallingContext sender, DebugEventArgs e)
		{
			OnDebugBefore(sender, e);
		}

		static internal void RaiseDebugAfter(CallingContext sender, DebugEventArgs e)
		{
			OnDebugAfter(sender, e);
		}

		public static bool IgnoreUnknownTokens { get; set; }
		public static Settings Set { get; set; }
		public static ExecutionPath Context { get; set; }

		static public CallingContext CallingContext
		{
			get { return Context.CurrentContext; }
		}

		static internal TokenCompiler TokenCompiler
		{
			get { return tokenCompilerField; }
		}

		static public DataSession GetDatasession(int id)
		{
			return datasessions[id];
		}

		public static ErrorItemCollection Errors { get; private set; }

		/// <summary>
		/// Replace the ObjectFactory if you want to use your own C# base classes.
		/// </summary>
		public static IObjectFactory ObjectFactory { get; set; }
		public static FileManager FileMgr { get; set; }
		public static LowLevelFileMngr FFilesManager { get; set; }
		public static PathHandling SetPathHandling { get; set; }
		public static WindowManager WinMgr { get; internal set; }

		static public void InitInstance()
		{
			InitInstance(Assembly.GetCallingAssembly());
		}
		static public void InitInstance(Assembly storageAssembly)
		{
			// TODO: right now InitInstance is not thread safe.
			if (!initialized)
			{
				initialized = true;
				storage = new ExecutableContext(storageAssembly);
				Context = new ExecutionPath();
				tokenCompilerField = new TokenCompiler();
				ObjectFactory = new ObjectFactory();
				datasessions = new DataSessionList();
				datasessions.Add(new DataSession());
				Public = new MemberList();
				Set = new Settings();
				FileMgr = new FileManager();
				Connections = new SptConnectionManager();
				FFilesManager = new LowLevelFileMngr();
				Errors = new ErrorItemCollection();
				optionsField = new Options();
				CommandFactory = new CommandFactory();
				PlatformSpecificInit();
			}
		}

		static public String FileNameToResourceName(String path)
		{
			ExecutableContext context = Context.CurrentExecutable ?? ServerExecutable;
			if (context == null)
				return null;

			return context.FileNameToResourceName(path);
		}

		static public ExecutableContext CurrentExecutable
		{
			get { return Context.CurrentExecutable; }
		}

		public static Encoding CurrentCp { get; private set; }
		public static MemberList Public { get; private set; }
		internal static CommandFactory CommandFactory { get; private set; }

		static ParameterCollection ToParameterCollection(IEnumerable<string> p)
		{
			var c = new ParameterCollection();
			foreach (var s in p)
			{
				var val = new ValueMember();
				val.Set(new Variant(s));
				c.Add(val);
			}
			return c;
		}

		static public void Do(string fileName, String[] args)
		{
			try
			{
				DoInContext(fileName, Context, ToParameterCollection(args));
			}
			catch (Exception ex)
			{
				OnException(ex);
			}
		}

		static public Variant Do(string fileName)
		{
			try
			{
				return DoInContext(fileName, Context, null);
			}
			catch (Exception ex)
			{
				OnException(ex);
			}
			return new Variant(true);
		}

		static public void Do(string fileName, String procName)
		{
			Do(fileName, procName, new string[0]);
		}

		static public void Do(string fileName, String procName, String[] args)
		{
			try
			{
				var resolver = new ProcedureResolver();
				CodeBlock code;
				resolver.FindProcedureIn(Context.CurrentContext, fileName, procName, out code);
				Context.ExecuteInNewContext(code, ToParameterCollection(args));
			}
			catch (Exception ex)
			{
				OnException(ex);
			}
		}

		private static void OnException(Exception ex)
		{
			if (DebugMode)
				throw ex;

			var callstack = "";
			foreach (var x in Context.Stack)
				if (String.IsNullOrEmpty(x.ModuleName))
					callstack += Path.GetFileNameWithoutExtension(x.FileName) + "\n";
				else
					callstack += x.ModuleName + "\n";

			var msg = ""
															 + "Version: " + Assembly.GetExecutingAssembly().GetName().Version + "\n"
															 + ex.Message + "\n\n"
															 + callstack + "\n\n"
															 + ex.StackTrace;
			var exceptionFile =
					"exception" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
			WriteAnsi(msg, exceptionFile);
			//MessageBox.Show(
			//  ex.Message + "\n" + ex.StackTrace);
			//	Application.Exit();
		}

		static private void WriteAnsi(string data, string name)
		{
			Stream fs = FileMgr.Open(name, FileMode.Create);
			var wr = new BinaryWriter(fs);
			byte[] values = CurrentCp.GetBytes(data);
			wr.Write(values);
			fs.Close();
			wr.Close();
		}
		static public void DoInContext(CompiledProgram fxp, ExecutionPath context, ParameterCollection parm)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			context.ExecuteInNewContext(fxp, parm);
		}

		#region for backward compatibility

		static public void DoInContext(string fileName, ExecutionPath context)
		{
			DoInContext(fileName, context, null);
		}
		static public void DoInContext(CompiledProgram fxp, ExecutionPath context)
		{
			DoInContext(fxp, context, null);
		}
		#endregion

		static Variant DoInContext(string fileName, ExecutionPath context, ParameterCollection parms)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			var fxp = new CompiledProgram(fileName);
			return context.ExecuteInNewContext(fxp, parms);
		}

		internal static void Quit()
		{
			if (OnShutdown != null)
				Context.ExecuteInNewContext(OnShutdown, null);
			// (...) Release resources
			// (...) terminate process
		}

		public static CodeBlock OnShutdown;

		static GuineuInstance()
		{
			SetPathHandling = PathHandling.Default;
		}
	}

	sealed public class DataSessionList : IndexedList<DataSession>
	{
		public DataSessionList()
		{
			// Create the system datasession.
			this[0] = new DataSession();
		}

	}

	sealed public class ParameterCollection : List<ValueMember>
	{
		public ParameterCollection()
		{ }

		internal ParameterCollection(CallingContext ctx, IEnumerable<ExpressionBase> p)
		{
			FillCollection(p, item => item.GetVariant(ctx));
		}

		public ParameterCollection(IEnumerable<Variant> p)
		{
			FillCollection(p, item => item);
		}

		void FillCollection<T>(IEnumerable<T> p, Func<T, Variant> conversion)
		{
			foreach (var item in p)
				Add(conversion(item));
		}

		public static ParameterCollection CreateByReference(CallingContext ctx, IEnumerable<ExpressionBase> p)
		{
			var pNew = new ParameterCollection();
			foreach (var item in p)
				pNew.Add(item.GetValueMember(ctx));
			return pNew;
		}

		public void Add(Variant item)
		{
			var val = new ValueMember();
			val.Set(item);
			Add(val);
		}

		public Object[] ToObjectArray()
		{
			var obj = new object[Count];
			for (var i = 0; i < Count; i++)
				obj[i] = this[i].Get().ToValue();
			return obj;
		}
	}

	sealed class StackLevel
	{
		internal Variant ReturnValue { get; set; }
		internal CompiledProgram Fxp { get; set; }
		internal String ModuleName { get; set; }
		internal String FileName { get; set; }
		internal ExecutableContext Executable { get; set; }
		internal List<ValueMember> Parameters { get; set; }
		internal MemberList Local { get; set; }
		internal MemberList Private { get; set; }
		internal readonly List<Nti> PrivatePatterns = new List<Nti>();

		public StackLevel(CodeBlock code, List<ValueMember> param)
		{
			Fxp = code.FileContext;
			Executable = code.Executable;
			ModuleName = code.Name.ToString();
			FileName = code.FileContext.FullName;
			Parameters = param;
			Local = new MemberList();
			Private = new MemberList();
		}
	}

	public class DataSession
	{
		const Int32 MaxWorkarea = 32767;

		readonly IndexedList<ICursor> workarea = new IndexedList<ICursor>();
		Int32 current = 1;

		// Adds a cursor to the datasession and returns the work area index.
		// If a table with that alias already exists, the existing work area
		// is replaced.
		public void Add(ICursor cr, Int32 area)
		{
			workarea.Add(cr, area);
		}

		/// <summary>
		/// Returns the workarea for a particular alias.
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Int32 Select(Nti alias)
		{
			for (Int32 area = 1; area < workarea.Length; area++)
				if (workarea.IsValid(area))
					if (workarea[area].Alias == alias)
						return area;
			return 0;
		}

		public void Select(Int32 area)
		{
			current = area;
		}

		public Int32 Select()
		{
			return current;
		}

		public int GetWorkArea(ExpressionBase area, CallingContext context)
		{
			Int32 areaNo = current;
			if (area != null)
			{
				if (area.FixedInt)
					areaNo = area.GetInt(context);
				else
				{
					String name = area.GetName(context);
					areaNo = !String.IsNullOrEmpty(name) ? Select(new Nti(name)) : area.GetInt(context);
				}
			}
			if (areaNo != 0)
				if (!WorkareaIsValid(areaNo))
					areaNo = 0;
			return areaNo;
		}

		public ICursor this[Int32 index]
		{
			get { return workarea[index]; }
		}

		internal ICursor this[ExpressionBase area, CallingContext context]
		{
			get { return workarea[GetWorkarea(area)]; }
		}

		internal ICursor GetExistingCursor(Nti name)
		{
			Int32 areaNo = Select(name);
			if (!WorkareaIsValid(areaNo))
				throw new ErrorException(ErrorCodes.AliasNotFound, name.ToString());
			return workarea[areaNo];
		}

		public Int32 GetNextFreeWorkarea()
		{
			Int32 area = workarea.GetNextFreeSlot();
			if (area <= MaxWorkarea)
				return area;
			// TODO: all work areas are used.  
			return 0;
		}


		internal ICursor GetCursor(ExpressionBase area)
		{
			Int32 areaNo = GetWorkarea(area);
			if (workarea[areaNo] == null)
				OpenCursor(areaNo);
			if (!WorkareaIsValid(areaNo))
				throw new ErrorException(ErrorCodes.NoTableOpen);
			return workarea[areaNo];
		}

		void OpenCursor(Int32 areaNo)
		{
			// TODO: Check SET TABLEPROMPT
			// TODO: Prompt for file Name
			// TODO: Open file, if a file Name has been returned
			workarea[areaNo] = null;
		}


		static Boolean WorkareaIsValid(Int32 area)
		{
			return (area >= 1) && (area <= MaxWorkarea);
		}
		Int32 GetWorkarea(ExpressionBase area)
		{
			CallingContext context = GuineuInstance.Context.CurrentContext;
			Int32 areaNo;
			if (area == null)
				areaNo = context.DataSession.Select();
			else
			{
				Nti n = area.ToNti(context);
				if (n.Valid())
					areaNo = Select(n);
				else
				{
					Variant val = area.GetVariant(context);
					if (val.Type == VariantType.Character)
					{
						var alias = area.ToNti(context);
						areaNo = Select(alias);
					}
					else
						areaNo = area.GetInt(context);
				}
			}

			// Make sure that we have a valid area number
			if (!WorkareaIsValid(areaNo))
				throw new ErrorException(ErrorCodes.TableNumberInvalid);

			return areaNo;
		}


		public ICursor Cursor
		{
			get { return workarea[current]; }
		}

		#region Manage work areas

		/// <summary>
		/// Returns the work area specified as a numeric work area or a Name with
		/// the alias.
		/// </summary>
		/// <param name="area"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		internal Int32 WorkareaFromName(ExpressionBase area, CallingContext context)
		{
			// Determine the name of the cursor.
			String name2 = area.GetName(context);
			
			Int32 areaNo;
			if (String.IsNullOrEmpty(name2))
				areaNo = area.GetInt(context);
			else
			{
				var name = new Nti(name2);
				if (!name.Valid())
					throw new ErrorException(ErrorCodes.TableNumberInvalid);
				areaNo = Select(name);
				if (areaNo == 0)
					throw new ErrorException(ErrorCodes.TableNumberInvalid);
			}
			return areaNo;
		}

		#endregion

		internal void Close(Int32 area)
		{
			if (area == 0)
				return;
			if (!WorkareaIsValid(area))
				throw new ErrorException(ErrorCodes.TableNumberInvalid);
			if (workarea[area] != null)
				workarea[area].Close();
			workarea[area] = null;
		}

		internal string GetAlias(String name, int area, ExpressionBase aliasClause)
		{
			String alias;

			// If an alias clause has been specified, the alias cannot be one of the predefined
			// aliases A-J and M. In case of A-J there's the exception that the alias Name can
			// be specified if it matches the current work area. Otherwise an "Alias Name already
			// in use" error is raised.
			if (aliasClause == null)
				alias = Path.GetFileNameWithoutExtension(
						name.Replace('\\', Path.DirectorySeparatorChar)
				);
			else
			{
				alias = StringUtil.Upper(aliasClause.GetName(GuineuInstance.CallingContext));
				if (IsDefaultAlias(alias))
					if (alias != DefaultArea(area))
						throw new ErrorException(ErrorCodes.AliasNameAlreadyInUse);
			}

			// Check if the alias is already in use in a different work area
			Int32 testArea = Select(new Nti(name));
			if (testArea == 0 || testArea == area)
				return alias;

			// The alias Name does already exist. If the user specified this alias Name, we
			// raise an error message. 
			if (aliasClause != null)
				throw new ErrorException(ErrorCodes.AliasNameAlreadyInUse);

			// Assemble the standard work area Name.
			if (area >= 1 && area <= 10)
				alias = DefaultArea(area);
			else
			{
				alias = "W" + area;
				testArea = Select(new Nti(name));
				if (testArea != 0 && testArea != area)
					alias = SYS2015.GetString();
			}
			return alias;
		}

		static String DefaultArea(Int32 area)
		{
			switch (area)
			{
				case 1:
					return "A";
				case 2:
					return "B";
				case 3:
					return "C";
				case 4:
					return "D";
				case 5:
					return "E";
				case 6:
					return "F";
				case 7:
					return "G";
				case 8:
					return "H";
				case 9:
					return "I";
				case 10:
					return "J";
				default:
					return "";
			}
		}

		static Boolean IsDefaultAlias(String alias)
		{
			if (alias.Length != 1)
				return false;
			if (alias == "A" || alias == "B"
					|| alias == "C" || alias == "D" || alias == "E"
					|| alias == "F" || alias == "G" || alias == "H" || alias == "I"
					|| alias == "J" || alias == "M")
				return true;
			return false;
		}


		internal void CloseAll()
		{
			for (Int32 area = 0; area < workarea.Length; area++)
				Close(area);
		}
	}

	/// <summary>
	/// The ExecutionContext is roughly a single thread
	/// </summary>
	/// <remarks>Currently Guineu supports only one ExecutionContext per runtime instance</remarks>
	public partial class ExecutionPath
	{
		private readonly List<StackLevel> stackField;
		readonly List<CallingContext> contextListField;

		public ExecutionPath()
		{
			stackField = new List<StackLevel>();
			contextListField = new List<CallingContext>();
		}

		internal CallingContext CurrentContext
		{
			get
			{
				if (contextListField.Count == 0)
					return null;
				return contextListField[contextListField.Count - 1];
			}
		}

		internal void AddContext(CallingContext context)
		{
			contextListField.Add(context);
		}

		/// <summary>
		/// Called by the CallingContext type to remove itself from the calling
		/// context stack.
		/// </summary>
		/// <param name="context"></param>
		internal void RemoveContext(CallingContext context)
		{
			if (contextListField.Count == 0)
				throw new Exception("context mismatch");
			//throw new ApplicationException("context mismatch");
			Debug.Assert(context == contextListField[contextListField.Count - 1]);

			contextListField.RemoveAt(contextListField.Count - 1);
		}

		internal EventData GetEvent ()
		{
			for (var i = contextListField.Count - 1; i >= 0; i--)
				if (contextListField[i].Event != null)
					return contextListField[i].Event;
			return null;
		}

		internal List<StackLevel> Stack
		{
			get { return stackField; }
		}

		internal static ErrorAction Error(ErrorCodes err, string param, Int32 line)
		{
			String name;
			String fileName;
			Int32 level = GuineuInstance.CallingContext.Context.Stack.Count;
			if (level > GuineuInstance.CallingContext.Context.Stack.Count)
			{
				name = "";
				fileName = "";
			}
			else
			{
				name = GuineuInstance.CallingContext.Context.Stack[level - 1].ModuleName;
				fileName = Path.GetFileNameWithoutExtension(GuineuInstance.CallingContext.Context.Stack[level - 1].FileName);
			}
			if (String.IsNullOrEmpty(name))
				name = fileName;
			return RespondToError(name, fileName, err, param, line);
		}


		/// <summary>
		/// Executes a CodeBlock in a new PROGRAM() level and returns the result.
		/// </summary>
		/// <param name="fxp"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		internal Variant ExecuteInNewContext(CompiledProgram fxp, ParameterCollection param)
		{
			CodeBlock code = fxp.MainProgram();
			return ExecuteInNewContext(code, param);
		}

		public Variant ExecuteInNewContext(CodeBlock code, ParameterCollection param)
		{
			return ExecuteInNewContext(code, param, null);
		}

		internal Variant ExecuteInNewContext(CodeBlock code, ParameterCollection param, ObjectBase obj)
		{
			StackLevel currentLevel = CreateStackLevel(code, param);
			stackField.Add(currentLevel);

			using (var context = new CallingContext(this, obj))
				context.Execute(code, null);

			stackField.RemoveAt(stackField.Count - 1);

			Variant retVal = currentLevel.ReturnValue;
			return retVal;
		}
		
		internal StackLevel CreateStackLevel(CodeBlock code, ParameterCollection param)
		{
			// (...) Currently we don't support private datasessions
			var newLevel = new StackLevel(code, param);
			return newLevel;
		}

		internal StackLevel GetCurrentStackLevel()
		{
			if (stackField.Count == 0)
				return null;

			return stackField[stackField.Count - 1];
		}

		internal ExecutableContext CurrentExecutable
		{
			get
			{
				if (stackField.Count == 0)
					return GuineuInstance.ServerExecutable;

				return stackField[stackField.Count - 1].Executable;
			}
		}

	}

	abstract class LineInfo
	{
	}

	/// <summary>
	/// Executes code in a particular context of stack level, object identitiy
	/// and file context.
	/// </summary>
	public class CallingContext : IDisposable
	{
		readonly ExecutionPath contextField;
		readonly StackLevel stackField;
		IMemberList writeMemberList;
		readonly ObjectBase thisField;
		MemberResolver resolverField;
		readonly EventData eventField;

		// Management of loops, etc.
		LineInfo[] lineInfo;

		public CallingContext(ExecutionPath ec)
		{
			if (ec == null)
				throw new ArgumentNullException("ec");
			contextField = ec;
			stackField = ec.GetCurrentStackLevel();
			ec.AddContext(this);
		}

		public CallingContext(ExecutionPath ec, ObjectBase obj)
			: this(ec)
		{
			thisField = obj;
		}

		public CallingContext(ExecutionPath ec, ObjectBase obj, EventData ev)
			: this(ec, obj)
		{
			eventField = ev;
		}
		public ICursor GetCursor(ExpressionBase workareaOrAlias)
		{
			var area = DataSession.GetWorkArea(workareaOrAlias, GuineuInstance.Context.CurrentContext);
			if (area == 0)
				return null;
			return DataSession[area];
		}
		public ICursor GetCursor()
		{
			return DataSession[null, GuineuInstance.Context.CurrentContext];
		}

		internal ExecutionPath Context
		{
			get { return contextField; }
		}
		internal ObjectBase This
		{
			get { return thisField; }
		}

		internal EventData Event
		{
			get { return eventField; }
		}

		internal List<Nti> PrivatePatterns { get { return stackField.PrivatePatterns; } }
		internal MemberList Privates { get { return stackField.Private; } }
		internal MemberList Locals
		{
			get
			{
				if (stackField == null)
					return GuineuInstance.Public;
				return stackField.Local;
			}
		}
		internal ValueMember CreateVariable(Variable var, Nti name)
		{
			if (var == null)
				throw new ErrorException(ErrorCodes.VariableNotFound, name);

			var dest = new ValueMember();
			
			var mbrList = var.GetResolverObject(this);
			if (mbrList == null)
				mbrList = PrivatePatterns.Contains(name) ? Privates : Write;
			mbrList.Add(name, dest);

			return dest;
		}

		internal IMemberList Write
		{
			get { return writeMemberList; }
			set { writeMemberList = value; }
		}
		internal MemberResolver Resolver
		{
			get { return resolverField ?? (resolverField = new NameResolver()); }
			set { resolverField = value; }
		}
		internal StackLevel Stack
		{
			get { return stackField; }
		}
		internal LineInfo GetLineInfo(int line)
		{
			return lineInfo[line];
		}

		internal void SetLineInfo(int line, LineInfo val)
		{
			lineInfo[line] = val;
		}

		public void Execute(CodeBlock code, IMemberList member)
		{
			// Validate parameters
			if (code == null)
				throw new ArgumentNullException("code");

			resolverField = new NameResolver();

			writeMemberList = member ?? Locals;

			int nextLine = 0;
			int lineCount = code.LineCount;
			lineInfo = new LineInfo[lineCount];

			//SharedStopwatch sw = null;

			// This is our main execution loop. Each iteration is one line of Guineu code.
			while (nextLine >= 0 && nextLine < lineCount)
			{
				Int32 lastLine = nextLine;
				if (GuineuInstance.CallDebugger)
					GuineuInstance.RaiseDebugBefore(this, new DebugEventArgs(this, code, nextLine));
				#region coverage profiler
				if (GuineuInstance.ProfileStream != null)
				{
					Stream profileStream = GuineuInstance.ProfileStream;
					var sb = new StringBuilder();
					sb.Append("".PadLeft(11));
					sb.Append(",");
					sb.Append("");  // Class Name
					sb.Append(",");
					String name = Context.Stack[Context.Stack.Count - 1].ModuleName;
					if (String.IsNullOrEmpty(name))
						name = Path.GetFileNameWithoutExtension(Context.Stack[Context.Stack.Count - 1].FileName);
					sb.Append(name);
					sb.Append(",");
					sb.Append(nextLine + 1);
					sb.Append(",");
					sb.Append(code.Code(nextLine).ToString());
					sb.Append(",");
					sb.Append(Context.Stack[Context.Stack.Count - 1].FileName.ToLower(System.Globalization.CultureInfo.InvariantCulture));
					sb.Append(",");
					sb.Append(Context.Stack.Count);
					sb.Append("\r\n");
					String strLine = sb.ToString();
					profileStream.Write(GuineuInstance.CurrentCp.GetBytes(strLine), 0, strLine.Length);
					//if (sw == null)
					//  sw = new SharedStopwatch();
					//else
					//  sw.Reset();
					//sw.Start();
				}
				#endregion
				try
				{
					var cmd = code.Code(nextLine++);
					if (cmd != null)
						cmd.Do(this, ref nextLine);
				}
				catch (ErrorException e)
				{
					ErrorAction action = ExecutionPath.Error(e.Error, e.Param, lastLine);
					if (action == ErrorAction.Cancel)
					{
						GuineuInstance.Quit();
						// (...) break all levels
						break;
					}
					if (action == ErrorAction.Retry)
						nextLine = lastLine;
				}
				//#region coverage profiler
				//if (GuineuInstance.ProfileStream != null)
				//{
				//  if (sw != null && GuineuInstance.ProfileStream == profileStream)
				//  {
				//    sw.Stop();
				//    profileStream.Position = position;
				//    Double val = (sw.ElapsedTicks / (Double)SharedStopwatch.Frequency);
				//    String sVal = val.ToString("###0.000000").PadLeft(11);
				//    sVal = sVal.Replace(',', '.');
				//    profileStream.Write(GuineuInstance.CurrentCp.GetBytes(sVal), 0, sVal.Length);
				//    profileStream.Seek(0, SeekOrigin.End);
				//  }
				//}
				//#endregion
				if (GuineuInstance.CallDebugger)
					GuineuInstance.RaiseDebugAfter(this, new DebugEventArgs(this, code, nextLine));
			}
		}

		internal DataSession DataSession
		{
			get { return GuineuInstance.GetDatasession(1); }
		}

		public void Dispose()
		{
			contextField.RemoveContext(this);
		}

	}
	[Flags]
	public enum PathHandling
	{
		UseUpperCaseNames = 0x00000001,

		Default = UseUpperCaseNames,
	}
}