using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Guineu.Classes;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Commands
{
	class SETCLASSLIB : ICommand
	{
		ExpressionBase classLib;
		Boolean additive;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.TO:
						classLib = comp.GetCompiledExpression();
						break;
					case Token.ADDITIVE:
						additive = true;
						break;
					case Token.CmdEnd:
						break;
					default:
						// (...) Invalid token
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			if (additive)
				GuineuInstance.Set.ClassLib.Add(classLib.GetString(context));
			else
				GuineuInstance.Set.ClassLib.Value = classLib.GetString(context);
		}
	}

	public interface IClassLibrary
	{
		Boolean ContainsClass(Nti name);
		Boolean IsSameAs(String name);
		ObjectCreationContext GetObjectCreationContext(CallingContext context, ClassLocator clsLoc, IClassObjectManager mgr);

		// ObjectTemplate GetClassObject(CallingContext ctx);
	}

	partial class ManagedClassLibrary : IClassLibrary
	{
		readonly Assembly file;

		public ManagedClassLibrary(String name)
		{
			try
			{
				file = Assembly.LoadFrom(name);
			}
			catch (IOException)
			{
				throw new ErrorException(ErrorCodes.FileNotFound, name);
			}
			}
		public bool ContainsClass(Nti name)
		{
			var type = GetTypeFromFile(name.ToString());
			return (type != null);
		}

		public bool IsSameAs(string name)
		{
			return false;
		}

		public ObjectCreationContext GetObjectCreationContext(CallingContext context, ClassLocator clsLoc, IClassObjectManager mgr)
		{
			var type = GetTypeFromFile(clsLoc.Name.ToString());
			var template = new ControlClassTemplate(type);
			var ctx = new ObjectCreationContext
									{
										Context = context,
										Name = clsLoc.Name,
										ClassObject = template
									};
			return ctx;
		}
	}

	class NativeClassLibrary : IClassLibrary
	{
		readonly CompiledProgram prg;

		public NativeClassLibrary(CompiledProgram file)
		{
			prg = file;
		}

		public bool ContainsClass(Nti name)
		{
			var info = prg.LocateClass(name);
			if (info != null && info.Constructor != null)
				return true;
			return false;
		}

		public bool IsSameAs(string name)
		{
			var fullName = GuineuInstance.FileMgr.FullPath(name, false).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
			return (prg.FullName == fullName);
		}

		public ObjectCreationContext GetObjectCreationContext(CallingContext context, ClassLocator clsLoc, IClassObjectManager mgr)
		{
			var ctx = new ObjectCreationContext
									{
										Context = context,
										Name = clsLoc.Name
									};
			var info = prg.LocateClass(clsLoc.Name);
			if (info != null && info.Constructor != null)
			{
				ctx.ClassObject = mgr.GetClassObject(context, new ClassLocator(info.ParentClass));
				ctx.Constructor = info.Constructor;
			}
			return ctx;
		}
	}

	public class SetClassLibValue : IEnumerable<IClassLibrary>
	{
		String classLib;
		readonly List<IClassLibrary> classLibList = new List<IClassLibrary>();

		public String Value
		{
			get { return classLib; }
			set { classLib = value; LoadClassLib(); }
		}

		void LoadClassLib()
		{
			// TODO: Handle all variations of SET CLASSLIB TO
			classLibList.Clear();
			foreach (var s in classLib.Split(','))
			{
				if (Path.GetExtension(s).ToUpper() == ".DLL")
				{
					classLibList.Add(new ManagedClassLibrary(s));
				}
				else
				{
					var name = Path.ChangeExtension(s, "VCX.FXP");
					classLibList.Add(new NativeClassLibrary(new CompiledProgram(name)));
				}
			}
		}

		public IEnumerator<IClassLibrary> GetEnumerator()
		{
			return classLibList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return classLibList.GetEnumerator();
		}

		public void Add(string s)
		{
			var filename = Path.ChangeExtension(s, "VCX.FXP");
			// TODO: Refactoring. String handling isn't optimal here.
			foreach (var lib in GuineuInstance.Set.ClassLib)
				if (lib.IsSameAs(filename))
					return;

			classLibList.Add(new NativeClassLibrary(new CompiledProgram(filename)));
			classLib += ", " + s;
		}
	}
}