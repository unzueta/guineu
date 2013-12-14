using System;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	partial class basListBox : UiControl
	{
		ClickMethod clickEvent;
		ListInteractiveChangeMethod interactiveChangeMethod;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		ListCountProperty listCountProperty;
		ListIndexProperty listIndexProperty;
		DisplayValueProperty displayValueProperty;
		VariantProperty tabIndexProperty;
		readonly GenericEvent keyPressEvent;

		public basListBox(ObjectTemplate obj)
			: base(obj)
		{
			keyPressEvent = new GenericEvent(this, KnownNti.KeyPress);
		}

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.ListControl);
			AddFontProperties();
			listCountProperty = new ListCountProperty();
			AddMember(listCountProperty);
			listIndexProperty = new ListIndexProperty(GetPropInt32("LISTINDEX"));
			AddMember(listIndexProperty);
			
			displayValueProperty = new DisplayValueProperty(GetPropString("DISPLAYVALUE"));
			AddMember(displayValueProperty);

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

		private void AddMethods()
		{
			clickEvent = new ClickMethod(this);
			interactiveChangeMethod = new ListInteractiveChangeMethod(this);

			Add(KnownNti.AddItem, new AddItemMethod(this));
			Add(KnownNti.Clear, new ListClearMethod(this));
			Add(KnownNti.Click, clickEvent);
			Add(KnownNti.InteractiveChange, interactiveChangeMethod);
			Add(KnownNti.RemoveItem, new RemoveItemMethod(this));
			gotFocusEvent = new GotFocusEvent(this);
			Add(KnownNti.GotFocus, gotFocusEvent);

			lostFocusEvent = new LostFocusEvent(this);
			Add(KnownNti.LostFocus, lostFocusEvent);
			Add(KnownNti.SetFocus, new SetFocusMethod(this));
		}

		virtual internal void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.ListBox);
			InitUiControl();
			Bind();
		}

		private void Bind()
		{
			clickEvent.Bind(View);
			interactiveChangeMethod.Bind();
			gotFocusEvent.Bind(View);
			lostFocusEvent.Bind(View);
			listCountProperty.AssignParent(this);
			listIndexProperty.AssignParent(this);
			displayValueProperty.AssignParent(this);
			tabIndexProperty.AssignParent(this);
			View.EventHandler += NotifyEvent;
		}
	
		void NotifyEvent(EventData e)
		{
			switch (e.Event)
			{
				case KnownNti.KeyPress:
					using (var ctx = new CallingContext(GuineuInstance.Context, this, e))
						keyPressEvent.Execute(ctx, e.Parameters);
					break;
			}
		}
	}

	public class basListBoxTemplate : UiControlTemplate
	{
		internal basListBoxTemplate() { }
		public basListBoxTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basListBox(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basListBoxTemplate();
		}

		protected override void DoAddMembers()
		{
			//// default for visually added controls (VCX, SCX)
			//DefaultWidth = 100;
			//DefaultHeight = 23;

			// default for programmatically added controls
			DefaultWidth = 100;
			DefaultHeight = 170;
			UsedMembers = SupportedMembers.ListControl;

			base.DoAddMembers();
			AddFontProperties();
			AddProperty(KnownNti.ListCount, 0);
			AddProperty(KnownNti.ListIndex, 0);
			AddProperty(KnownNti.DisplayValue, "");
			AddProperty(KnownNti.TabIndex, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.ListControl;
			base.DoAddMembers(template);
			CloneMember(KnownNti.ListCount, template);
			CloneMember(KnownNti.ListIndex, template);
			CloneMember(KnownNti.TabIndex, template);
			CloneMember(KnownNti.DisplayValue, template);
		}
	}

}
