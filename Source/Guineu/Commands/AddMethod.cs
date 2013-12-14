using System;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu
{
	class AddMethod : ICommand
	{
		ExpressionBase methodIndex;
		Class hostClass;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			methodIndex = comp.GetCompiledExpression();
			hostClass = code.FileContext.LocateClass(code.Name);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			int index = methodIndex.GetInt(context);
			CodeBlock method = hostClass.Methods[index];
			Nti name = method.Name;

			var parent = context.This.GetActualParent(name.ToString());
			// TODO: Instead of ADD we might need to use an Update, to replace the implementation of an existing method
			//           A test case might be a container class that has a button object, where the container is added to the form and then 
			//           the button click changed
			parent.Add(ObjectBase.GetActualName(name.ToString()), new MethodImplementation(method));
		}
	}

}
