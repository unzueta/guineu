using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Commands;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Functions
{
	class NEWOBJECT : ExpressionBase
	{
		ExpressionBase className;
		ExpressionBase classLibrary;
		ExpressionBase application;
		readonly List<ExpressionBase> parameters = new List<ExpressionBase>();

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					className = param[0];
					break;
				case 2:
					className = param[0];
					classLibrary = param[1];
					break;
				case 3:
					className = param[0];
					classLibrary = param[1];
					application = param[2];
					break;
				default:
					className = param[0];
					classLibrary = param[1];
					application = param[2];
					parameters.AddRange(param.GetRange(3, param.Count - 3));
					break;
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			ClassLocator clsLoc;
			if( classLibrary == null )
			{
				clsLoc = new ClassLocator( className.ToNti(context));
			}
			else
			{
				String name = Path.ChangeExtension(classLibrary.GetString(context), "FXP");

				if (!File.Exists(name))
				{
					// TODO: File not found
				}
				var program = new NativeClassLibrary( new CompiledProgram(name));
				clsLoc = new ClassLocator(className.ToNti(context),program);
			}

			ObjectBase newObj = GuineuInstance.ObjectFactory.CreateObject(context, clsLoc,parameters);
			return new Variant(newObj);
		}
    }
}
