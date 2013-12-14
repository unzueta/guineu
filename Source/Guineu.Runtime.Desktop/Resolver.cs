using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Commands;
using Guineu.Data;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu
{
	/// <summary>
	/// translates procedures names into code by searching for a procedure
	/// </summary>
	class ProcedureResolver
	{
		internal CompiledProgram FindProcedure(CallingContext context, string name)
		{
			CodeBlock dummy;
			return FindProcedure(context, name, out dummy);
		}

		internal CompiledProgram FindProcedureIn(CallingContext context, string fileName, string procName, out CodeBlock code)
		{
			String name = Path.ChangeExtension(fileName, "FXP");

			if (!GuineuInstance.FileMgr.Exists(name))
			{
				code = null;
				return null;
			}

			var program = new CompiledProgram(name);
			code = program.Locate(new Nti(procName));
			return program;
		}

		internal CompiledProgram FindProcedure(CallingContext context, string procName, out CodeBlock code)
		{
			CompiledProgram rVal = LocateProcedureInCallingStack(context, procName, out code) ??
														 LocateProcedureAsFxp(procName, out code);
			return rVal;
		}

		/// <summary>
		/// Searches for an FXP file
		/// </summary>
		/// <param name="procName"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		private static CompiledProgram LocateProcedureAsFxp(string procName, out CodeBlock code)
		{
			String name = Path.ChangeExtension(procName, "FXP");

			// Does the FXP file exist
			if (!GuineuInstance.FileMgr.Exists(name))
			{
				code = null;
				return null;
			}

			var program = new CompiledProgram(name);
			// TODO: use a separate method
			code = program.MainProgram();
			return program;
		}

		/// <summary>
		/// Searches all FXPs in the calling hierarchy to locate a particular procedure
		/// </summary>
		/// <param name="context"></param>
		/// <param name="name"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		static CompiledProgram LocateProcedureInCallingStack(CallingContext context, string name, out CodeBlock code)
		{
			List<StackLevel> stack = context.Context.Stack;
			CompiledProgram retVal = null;
			code = null;
			for (int level = stack.Count - 1; level >= 0; level--)
			{
				CompiledProgram curProgram = stack[level].Fxp;
				code = curProgram.Locate(new Nti(name));
				if (code != null)
				{
					retVal = curProgram;
					break;
				}
			}
			return retVal;
		}

	}

	/*
	internal interface IClassObjectFactory
	{
		ObjectTemplate GetClassObject(CallingContext ctx);
	}

	class NativeClassObjectFactory : IClassObjectFactory
	{
		ClassLocator classLocator;

		public NativeClassObjectFactory(ClassLocator locator)
		{
			classLocator = locator;	
		}

		public ObjectTemplate GetClassObject(CallingContext context)
		{
			var ctx = new ObjectCreationContext { Context = context, Name = classLocator.Name };
			Nti parentClass;
				ClassResolver.FindClass(context, classLocator, out ctx.Constructor, out parentClass);

			var cls = GuineuInstance.ObjectFactory. GetClassObject(context, new ClassLocator(parentClass));
			return cls;
		}

	}
	*/

	/// <summary>
	/// Locates the constructor procedure for a given class Name
	/// </summary>
	static class ClassResolver
	{
		static internal IClassLibrary FindClass(CallingContext context, ClassLocator clsLoc)
		{
			// (...) Locate in cache
			IClassLibrary rVal = LocateClassInLibrary(clsLoc)
					?? LocateClassInCallingStack(context, clsLoc.Name)
					?? LocateClassInProcedures(clsLoc.Name)
					?? LocateClassInClassLibs(clsLoc.Name);

			// (...) Locate as FXP file
			return rVal;
		}

		static IClassLibrary LocateClassInClassLibs(Nti name)
		{
			// TODO: Refactoring. Code same as following procedure
			foreach (var prg in GuineuInstance.Set.ClassLib)
			{
				var clsLoc = new ClassLocator(name, prg);
				var rVal = LocateClassInLibrary(clsLoc);
				if (rVal != null)
					return rVal;
			}

			return null;
		}

		static IClassLibrary LocateClassInProcedures(Nti name)
		{
			foreach (var prg in GuineuInstance.Set.Procedure)
			{
				var clsLoc = new ClassLocator(name, new NativeClassLibrary(prg));
				var rVal = LocateClassInLibrary(clsLoc);
				if (rVal != null)
					return rVal;
			}

			return null;
		}

		private static IClassLibrary LocateClassInLibrary(ClassLocator clsLoc)
		{
			if (clsLoc.Src == null)
			{
				return null;
			}

			if (clsLoc.Src.ContainsClass(clsLoc.Name))
			{
				return clsLoc.Src;
			}
			return null;
		}

		/// <summary>
		/// Searches all FXPs in the calling hierarchy to locate a particular class
		/// </summary>
		/// <param name="context"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		static IClassLibrary LocateClassInCallingStack(CallingContext context, Nti name)
		{
			List<StackLevel> stack = context.Context.Stack;
			IClassLibrary retVal = null;
			for (int level = stack.Count - 1; level >= 0; level--)
			{
				CompiledProgram curProgram = stack[level].Fxp;
				var library = new NativeClassLibrary(curProgram);
				if (library.ContainsClass(name))
				{
					retVal = library;
					break;
				}
			}
			return retVal;
		}

		//    var info = curProgram.LocateClass(name);
		//if (info != null && info.Constructor != null)
		//{
		//  constructor = info.Constructor;
		//  parentClass = info.ParentClass;
		//  retVal = curProgram;
		//  break;
		//}

		// (...) REfactor to avoid duplicate code. Resolver can use a strategy to call Locate or LocateClass
	}

	/// <summary>
	/// Is responsible for locating members given a Name. Subclasses implement
	/// various strategies depending on the context.
	/// </summary>
	abstract public class MemberResolver
	{
		abstract internal Member Resolve(CallingContext context, Nti name);
	}

	/// <summary>
	/// Resolve a member only by looking at variables. This includes LOCALs, PRIVATE and PUBLIC
	/// variables.
	/// </summary>
	class VariableResolver : MemberResolver
	{
		internal override Member Resolve(CallingContext context, Nti name)
		{
			Member memberFound = null;

			// TODO: On Mono context.Write is null

			if (context.Write != null)
				memberFound = context.Write.GetMember(name);

			// TODO: Resolve local variables

			// Resolve private variables
			if (memberFound == null)
			{
				var stack = GuineuInstance.Context.Stack;
				for (var i = stack.Count-1; i >= 0; i--)
				{
					memberFound = stack[i].Private.GetMember(name);
					if (memberFound != null)
						break;
				}
			}

			// Resolve public variables
			if (memberFound == null)
				memberFound = GuineuInstance.Public.Get(name);

			return memberFound;
		}
	}

	/// <summary>
	/// Resolves a single Name which can be a field Name or a variable.
	/// </summary>
	class NameResolver : MemberResolver
	{
		readonly VariableResolver var;

		internal NameResolver()
		{
			var = new VariableResolver();
		}

		internal override Member Resolve(CallingContext context, Nti name)
		{

			// 
			Member mbr = null;

			// check if Name is an alias
			ICursor csr = context.DataSession.Cursor;
			if (csr != null)
				mbr = csr.Fields.Get(name);

			if (mbr == null)
				mbr = var.Resolve(context, name);

			return mbr;
		}
	}

	/// <summary>
	/// Resolve a member within a single MemberList such as properties.
	/// </summary>
	internal class MemberListResolver : MemberResolver
	{
		readonly IMemberList memberlist;
		readonly ObjectBase objRef;

		internal MemberListResolver(IMemberList list)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			memberlist = list;
			objRef = list as ObjectBase;
		}

		internal override Member Resolve(CallingContext context, Nti name)
		{
			return memberlist.GetMember(name);
		}

		internal Member Resolve(Nti name)
		{
			return Resolve(null, name);
		}

		internal ObjectBase GetThis()
		{
			return objRef;
		}
	}

}
