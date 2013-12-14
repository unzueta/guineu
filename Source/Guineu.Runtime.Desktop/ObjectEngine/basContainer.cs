using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basContainer : UiControl
	{
		ClickMethod clickEvent;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		VariantProperty pemTabIndex;

		public basContainer(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.Colors);
			AddProperties();
			AddMethods();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		void AddProperties()
		{
			pemTabIndex = new VariantProperty(KnownNti.TabIndex, new Variant(GetPropInt32("TABINDEX"), 10));
			AddMember(pemTabIndex);
		}
		virtual internal void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.Container);
			InitUiControl();
			Bind();
		}

		private void AddMethods()
		{
			clickEvent = new ClickMethod(this);
			Add(KnownNti.Click, clickEvent);

			gotFocusEvent = new GotFocusEvent(this);
			Add(KnownNti.GotFocus, gotFocusEvent);

			lostFocusEvent = new LostFocusEvent(this);
			Add(KnownNti.LostFocus, lostFocusEvent);
			Add(KnownNti.SetFocus, new SetFocusMethod(this));
		}

		private void Bind()
		{
			clickEvent.Bind(View);
			gotFocusEvent.Bind(View);
			lostFocusEvent.Bind(View);
			pemTabIndex.AssignParent(this);
		}
	}

	public class basContainerTemplate : UiControlTemplate
	{
		internal basContainerTemplate() { }
		public basContainerTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basContainer(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basContainerTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 75;
			DefaultHeight = 75;
			UsedMembers = SupportedMembers.Colors;
			base.DoAddMembers();
			AddProperty(KnownNti.TabIndex, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.Colors;
			base.DoAddMembers(template);
			CloneMember(KnownNti.TabIndex, template);
		}
	}

}
