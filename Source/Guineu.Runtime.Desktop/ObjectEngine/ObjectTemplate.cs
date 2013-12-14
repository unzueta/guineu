using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	//=====================================================================================
	public abstract class ObjectTemplate : ObjectBase
	{
		protected  ObjectTemplate()
		{ }

		protected ObjectTemplate(String name)
		{
			Nti = new Nti(name);
			DoAddMembers();
		}

		public ObjectBase CreateInstance()
		{
			ObjectBase newObj = DoCreateInstance();
			newObj.Nti = Nti;
			foreach (ObjectBase obj in Controls)
			{
				var ctrl = obj as ObjectTemplate;
				if (ctrl != null)
				{
					ObjectBase chld = ctrl.CreateInstance();
					newObj.Controls.Add(chld);
				}
			}
			return newObj;
		}

		public ObjectTemplate CreateSubclass(Nti name)
		{
			ObjectTemplate obj = DoCreateTemplate();
			obj.Nti = name;
			obj.DoAddMembers(this);
			obj.AddUserDefinedMembers(this);
			foreach (ObjectTemplate ctrl in Controls)
			{
				ObjectTemplate chld = ctrl.CreateSubclass(ctrl.Nti);
				obj.Controls.Add(chld);
			}
			return obj;
		}


		protected abstract void DoAddMembers();
		protected abstract void DoAddMembers(IMemberList members);
		protected abstract ObjectBase DoCreateInstance();
		protected abstract ObjectTemplate DoCreateTemplate();

	}

}

