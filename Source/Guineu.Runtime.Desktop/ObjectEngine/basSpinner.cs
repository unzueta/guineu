using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class basSpinner : UiControl
	{
		ClickMethod clickEvent;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		VariantProperty pemTabIndex;

		public basSpinner(ObjectTemplate obj)
			: base(obj)
		{ }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.TextControl);
			AddFontProperties();
			AddProperties();
			AddMethods();
			AddEvents();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		void AddProperties()
		{
			pemTabIndex = new VariantProperty(KnownNti.TabIndex, new Variant(GetPropInt32("TABINDEX"), 10));
			AddMember(pemTabIndex);
		}

		private void AddEvents()
		{
			AddValidEvent();
			AddWhenEvent();
		}

		virtual internal void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.Spinner);
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

	public class basSpinnerTemplate : UiControlTemplate
	{
		internal basSpinnerTemplate() { }
		public basSpinnerTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basSpinner(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basSpinnerTemplate();
		}

		protected override void DoAddMembers()
		{
			//// default for visually added controls (VCX, SCX)
			//DefaultWidth = 100;
			//DefaultHeight = 23;

			// default for programmatically added controls
			DefaultWidth = 100;
			DefaultHeight = 21;
			UsedMembers = SupportedMembers.TextControl;

			base.DoAddMembers();
			AddFontProperties();
			AddProperty(KnownNti.TabIndex, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.TextControl;
			base.DoAddMembers(template);
			CloneMember(KnownNti.TabIndex, template);
		}
	}

}
