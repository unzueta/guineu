using System;
using System.Diagnostics;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class basTextbox : UiControl
	{
		ClickMethod clickEvent;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		VariantProperty pemTabIndex;
		readonly GenericEvent keyPressEvent;

		public basTextbox(ObjectTemplate obj)
			: base(obj)
		{
			keyPressEvent = new GenericEvent(this, KnownNti.KeyPress);
		}

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.TextControl);
			AddFontProperties();
			AddReadonlyProperty();
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
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.Textbox);
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
			View.EventHandler += NotifyEvent;
		}

		void NotifyEvent(EventData e)
		{
			switch (e.Event)
			{
				case KnownNti.ControlSource:
					Debug.Assert(e.Parameters.Count == 1);
					if (e.Parameters.Count < 1)
						throw new ArgumentException("KnownNti.ControlSource requires at least on parameter.");
					ControlSourceProperty.SetValue(e.Parameters[0].Get());
					break;
			
				case KnownNti.KeyPress:
					using (var ctx = new CallingContext(GuineuInstance.Context, this, e))
						keyPressEvent.Execute(ctx, e.Parameters);
					break;
			}
		}

		Variant Validating()
		{
			// Call Valid event
			// if OK
			//  Update ControlSource
			// else
			//  Display error message
			return new Variant();
		}
	}

	public class basTextBoxTemplate : UiControlTemplate
	{
		internal basTextBoxTemplate() { }
		public basTextBoxTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basTextbox(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basTextBoxTemplate();
		}

		protected override void DoAddMembers()
		{
			//// default for visually added controls (VCX, SCX)
			//_DefaultWidth = 100;
			//_DefaultHeight = 23;

			// default for programmatically added controls
			DefaultWidth = 100;
			DefaultHeight = 21;
			UsedMembers = SupportedMembers.TextControl;

			base.DoAddMembers();
			AddFontProperties();
			AddProperty(KnownNti.ReadOnly, false);
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
