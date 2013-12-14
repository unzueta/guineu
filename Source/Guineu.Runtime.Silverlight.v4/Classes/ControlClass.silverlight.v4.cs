using System;
using System.Windows;
using System.Windows.Controls;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Classes
{
	public partial class ControlClass
	{
		partial void DoAddControl()
		{
			if (control is Panel)
				AddPanel((Panel)control);
		}

		void AddPanel(Panel ctrl)
		{
			foreach (FrameworkElement obj in ctrl.Children)
			{
				if (obj != null)
					AddControl(obj.Name, new ControlClass(obj));
			}
		}

		void AddControl(String name, ObjectBase obj)
		{
			var prop = new ControlMember(obj);
			Add(new Nti(name), prop);
		}
	}
}