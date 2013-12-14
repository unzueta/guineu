using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class EditboxClass : UiControl
	{
		readonly ClickMethod clickEvent;
		readonly GotFocusEvent gotFocusEvent;
		readonly LostFocusEvent lostFocusEvent;
		readonly VariantProperty tabIndexProperty;
		readonly GenericProperty selStartProperty;
		readonly GenericProperty selLengthProperty;

		public EditboxClass(ObjectTemplate obj)
			: base(obj)
		{
			selStartProperty = new GenericProperty(KnownNti.SelStart, GetPropVariant(KnownNti.SelStart));
			selLengthProperty = new GenericProperty(KnownNti.SelLength, GetPropVariant(KnownNti.SelLength));
			tabIndexProperty = new VariantProperty(KnownNti.TabIndex, new Variant(GetPropInt32(KnownNti.TabIndex), 10));

			clickEvent = new ClickMethod(this);
			gotFocusEvent = new GotFocusEvent(this);
			lostFocusEvent = new LostFocusEvent(this);
		}

		protected override void DoInitializeInstance()
		{
			base.DoInitializeInstance();
			AddMembers();
			DoCreateControl();
			BindMembers();
		}

		void AddMembers()
		{
			AddUserInterfaceControlProperties(SupportedMembers.TextControl);
			AddFontProperties();
			AddReadonlyProperty();
			AddProperties();
			AddMethods();
			AddEvents();
			AddUserDefinedMembers();
		}

		void BindMembers()
		{
			clickEvent.Bind(View);
			gotFocusEvent.Bind(View);
			lostFocusEvent.Bind(View);
			tabIndexProperty.AssignParent(this);
			selStartProperty.AssignParent(View);
			selLengthProperty.AssignParent(View);
		}

		void AddProperties()
		{
			AddMember(tabIndexProperty);
			AddMember(selStartProperty);
			AddMember(selLengthProperty);
		}

		private void AddEvents()
		{
			AddValidEvent();
			AddWhenEvent();
		}

		protected virtual void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.EditBox);
			InitUiControl();
		}

		private void AddMethods()
		{
			Add(KnownNti.Click, clickEvent);
			Add(KnownNti.GotFocus, gotFocusEvent);
			Add(KnownNti.LostFocus, lostFocusEvent);
			Add(KnownNti.SetFocus, new SetFocusMethod(this));
		}
	}

	public class EdtiboxClassTemplate : UiControlTemplate
	{
		EdtiboxClassTemplate() { }
		public EdtiboxClassTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new EditboxClass(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new EdtiboxClassTemplate();
		}

		protected override void DoAddMembers()
		{
			//// default for visually added controls (VCX, SCX)
			//DefaultWidth = 100;
			//DefaultHeight = 75;

			// default for programmatically added controls
			DefaultWidth = 100;
			DefaultHeight = 75;
			UsedMembers = SupportedMembers.TextControl;

			base.DoAddMembers();
			AddFontProperties();
			AddProperty(KnownNti.ReadOnly, false);
			AddProperty(KnownNti.TabIndex, 0);
			AddProperty(KnownNti.SelStart, 0);
			AddProperty(KnownNti.SelLength, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.TextControl;
			base.DoAddMembers(template);
			CloneMember(KnownNti.TabIndex, template);
		}
	}

}
