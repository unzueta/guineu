using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	sealed class BasCheckBox : UiControl
	{
		ClickMethod clickEvent;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		VariantProperty tabIndexProperty;

		public BasCheckBox(ObjectTemplate obj)
			: base(obj)
		{ }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.CheckControl);
			AddFontProperties();
			AddProperties();
			AddMethods();
			AddEvents();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		void AddProperties()
		{
			tabIndexProperty = new VariantProperty(KnownNti.TabIndex, new Variant(GetPropInt32("TABINDEX"), 10));
			AddMember(tabIndexProperty);
		}

		private void AddEvents()
		{
			AddValidEvent();
			AddWhenEvent();
		}

		void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.CheckBox);
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
			tabIndexProperty.AssignParent(this);
		}
	}

	public class BasCheckBoxTemplate : UiControlTemplate
	{
		BasCheckBoxTemplate() { }
		public BasCheckBoxTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new BasCheckBox(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new BasCheckBoxTemplate();
		}

		protected override void DoAddMembers()
		{
			//// default for visually added controls (VCX, SCX)
			//DefaultWidth = 100;
			//DefaultHeight = 23;


			// default for programmatically added controls
			DefaultWidth = 100;
			DefaultHeight = 21;
			UsedMembers = SupportedMembers.CheckControl;
			DefaultValue = new Variant(0, 10);

			base.DoAddMembers();
			AddFontProperties();
			AddProperty(KnownNti.TabIndex, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.CheckControl;
			base.DoAddMembers(template);
			CloneMember(KnownNti.TabIndex, template);
		}
	}

}
