using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basShape : UiControl
	{
		public basShape(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.None);
			AddUserDefinedMembers();
			DoCreateControl();
		}

		virtual internal void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.Shape);
			InitUiControl();
		}
	}

	internal class basShapeTemplate : UiControlTemplate
	{
		internal basShapeTemplate() { }
		internal basShapeTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basShape(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basShapeTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 100;
			DefaultHeight = 17;
			base.DoAddMembers();
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.None;
			base.DoAddMembers(template);
		}
	}

}
