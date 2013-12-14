using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal class basCustom : NestedClass
	{
			internal basCustom (ObjectTemplate obj) : base(obj) { }

			protected override void DoInitializeInstance()
			{
                base.DoInitializeInstance();
				AddUserDefinedMembers();
			}

	}

	internal class basCustomTemplate : ObjectTemplate
	{
		internal basCustomTemplate() { }
		internal basCustomTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basCustom(this);
		}

		protected override void DoAddMembers()
		{
		}
		protected override void DoAddMembers(IMemberList members)
		{
			
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basCustomTemplate();
		}
	}
}
