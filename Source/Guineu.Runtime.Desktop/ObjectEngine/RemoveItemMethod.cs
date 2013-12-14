using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class RemoveItemMethod : MethodMember
	{
		public RemoveItemMethod(ObjectBase obj)
			: base(obj)
		{
		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			switch (param.Count)
			{
				case 0:
					// TODO: Use WindowType
					break;
				case 1:
					// TODO: Check subscript out of range
					Int32 item = param[0].Get();
					UiControl ctrl = (UiControl)Object;
					IGuiList lst = ctrl.View as IGuiList;
					lst.GuiRemoveItem(item-1);
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			return new Variant(true);
		}

	}
}
