using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Guineu.Commands;
using Guineu.ObjectEngine;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Interop
{
	public partial class GuineuHost : UserControl
	{
		UiControl objReference;

		public GuineuHost(String file, String className)
		{
			InitializeComponent();
			GuineuInstance.InitInstance(Assembly.GetCallingAssembly());
			LoadObject(file, new Nti(className));
		}

		protected UiControl Object
		{
			get { return objReference; }
		}

		private void LoadObject(String file, Nti className)
		{
			var code = new NativeClassLibrary(new CompiledProgram(file));
			var clsLoc = new ClassLocator(className, code, true);

			//StackLevel currentLevel = GuineuInstance.Context.CreateStackLevel((CodeBlock) null , (ParameterCollection) null);
			//GuineuInstance.Context.Stack.Add(currentLevel);

			using (var context = new CallingContext(GuineuInstance.Context))
			{
				ObjectBase newObj = GuineuInstance.ObjectFactory.CreateObject(context, clsLoc, new List<ExpressionBase>());
				if (newObj == null)
					throw new COMException();

				objReference = newObj as UiControl;
				if (objReference == null)
					throw new ErrorException(ErrorCodes.ObjectClassInvalid);
				
				var ctrl = objReference.View as Control;
				if (ctrl == null)
					throw new ErrorException(ErrorCodes.ObjectClassInvalid);
				
				ctrl.Location = new Point(0, 0);
				Size = ctrl.Size;
				Controls.Add(ctrl);
			}

			//GuineuInstance.Context.Stack.RemoveAt(GuineuInstance.Context.Stack.Count - 1);

		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (objReference != null)
			{
				var ctrl = objReference.View as Control;
				if (ctrl != null)
				{
					ctrl.Size = Size;
				}
			}
		}
	}
}
