using System;
using System.Reflection;
using System.Windows;
using Guineu.Classes;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu
{
	public class ProcedureFile
	{
		readonly CompiledProgram file;

		public ProcedureFile(String name)
		{
			GuineuInstance.InitInstance(Assembly.GetCallingAssembly());
			file = new CompiledProgram(name);
		}

		public Variant Do(String procedure, params object[] p)
		{
			var code = file.Locate(new Nti(procedure));
			return GuineuInstance.Context.ExecuteInNewContext(code, GetParameters(p));
		}

		public ParameterCollection GetParameters(object[] parms)
		{
			var parm = new ParameterCollection();
			foreach (object p in parms)
				parm.AddParameter(p);
			return parm;
		}
	}

	static class ProcedureFileExtensionMethods
	{
		public static void AddParameter(this ParameterCollection parm, object p)
		{
			var member = new ValueMember();
            member.Set(Variant.Create(p));
			parm.Add(member);
		}
	}
}
