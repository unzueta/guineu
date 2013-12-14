using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class FormShowMethod : MethodMember
	{
		basForm m_Form;

		public FormShowMethod(ObjectBase obj) : base(obj)
		{
		}

		public FormShowMethod(basForm form)
			: 
			base(form)
		{
			m_Form = form;

		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			bool IsModal = false;
			switch (param.Count)
			{
				case 0:
					// TODO: Use WindowType
					break;
				case 1:
					int ShowType = param[0].Get();
					switch (ShowType)
					{
						case 1:
							IsModal = true;
							break;
						case 2:
							IsModal = false;
							break;
						default:
							throw new ErrorException(ErrorCodes.InvalidArgument);
					}
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			if (IsModal)
			{
				((IGuiForm) m_Form.View).GuiShowDialog();
			}
			else
			{
				((IGuiForm)m_Form.View).GuiShow();
			}
			return new Variant(true);
		}

	}
}
