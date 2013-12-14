using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Guineu.ObjectEngine
{
	class FormReleaseMethod : MethodMember
	{
		basForm m_Form;

		public FormReleaseMethod(ObjectBase obj)
			: base(obj)
		{
		}

		public FormReleaseMethod(basForm form)
			:
			base(form)
		{
			m_Form = form;

		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			//TODO: Implement Destroy behavior
			((Form)m_Form.View).Close();
			return new Variant(true);
		}

	}
}
