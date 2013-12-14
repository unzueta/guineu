using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basHeader : UiControl
	{
		public basHeader(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(
				SupportedMembers.Colors | SupportedMembers.Caption
			);
			AddFontProperties();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		virtual internal void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.Header);
			InitUiControl();
		}
	}

	internal class basHeaderTemplate : UiControlTemplate
	{
		internal basHeaderTemplate() { }
		internal basHeaderTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basHeader(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basHeaderTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 75;
			DefaultHeight = 100;
			base.DoAddMembers();
			AddFontProperties();
		}
	}

}
