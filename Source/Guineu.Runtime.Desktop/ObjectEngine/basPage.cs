using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basPage : UiControl
	{
		ClickMethod clickEvent;
		PageActivateEvent pemActivateEvent;
		
		public basPage(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(
				SupportedMembers.Caption |
				SupportedMembers.Colors
			);
			AddFontProperties();
			AddMethods();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		private void AddMethods()
		{
			clickEvent = new ClickMethod(this);
			Add(KnownNti.SetFocus, new SetFocusMethod(this));
			pemActivateEvent = new PageActivateEvent(this);
			Add(KnownNti.Activate, pemActivateEvent);
		}

		virtual internal void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.Page);
			InitUiControl();
			Bind();
		}

		private void Bind()
		{
			clickEvent.Bind(View);
			pemActivateEvent.Bind();
		}
	}

	public class basPageTemplate : UiControlTemplate
	{
		internal basPageTemplate() { }
		public basPageTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basPage(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basPageTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 375;
			DefaultHeight = 254;
			base.DoAddMembers();
			AddFontProperties();
		}
	}

}
