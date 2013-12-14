using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class basComboBox : UiControl
	{
		ClickMethod clickEvent;
		ListInteractiveChangeMethod _InteractiveChange;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		ListCountProperty pemListCount;
		ListIndexProperty pemListIndex;
		DisplayValueProperty pemDisplayValue;
		VariantProperty pemTabIndex;
		VariantProperty PemRowSourceType;
		VariantProperty PemRowSource;

		public basComboBox(ObjectTemplate obj)
			: base(obj)
		{ }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.ListControl);
			AddFontProperties();
			pemListCount = new ListCountProperty();
			AddMember(pemListCount);
			pemListIndex = new ListIndexProperty(GetPropInt32("LISTINDEX"));
			AddMember(pemListIndex);
			pemDisplayValue = new DisplayValueProperty(GetPropString("DISPLAYVALUE"));
			AddMember(pemDisplayValue);
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

			PemRowSourceType = new VariantProperty(KnownNti.RowSourceType, new Variant(GetPropInt32("ROWSOURCETYPE"),10));
			AddMember(PemRowSourceType);

			PemRowSource = new VariantProperty(KnownNti.RowSource, new Variant(GetPropString("ROWSOURCE")));
			AddMember(PemRowSource);
		}

		private void AddEvents()
		{
			AddValidEvent();
			AddWhenEvent();
		}

		private void AddMethods()
		{
			clickEvent = new ClickMethod(this);
			_InteractiveChange = new ListInteractiveChangeMethod(this);

			Add(KnownNti.AddItem, new AddItemMethod(this));
			Add(KnownNti.Clear, new ListClearMethod(this));
			Add(KnownNti.Click, clickEvent);
			Add(KnownNti.InteractiveChange, _InteractiveChange);
			Add(KnownNti.RemoveItem, new RemoveItemMethod(this));

			gotFocusEvent = new GotFocusEvent(this);
			Add(KnownNti.GotFocus, gotFocusEvent);

			lostFocusEvent = new LostFocusEvent(this);
			Add(KnownNti.LostFocus, lostFocusEvent);
			Add(KnownNti.SetFocus, new SetFocusMethod(this));
		}

		virtual internal void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.ComboBox);
			InitUiControl();
			Bind();
		}

		private void Bind()
		{
			clickEvent.Bind(View);
			_InteractiveChange.Bind();
			gotFocusEvent.Bind(View);
			lostFocusEvent.Bind(View);
			pemListCount.AssignParent(this);
			pemListIndex.AssignParent(this);
			pemDisplayValue.AssignParent(this);
			pemTabIndex.AssignParent(this);
			PemRowSource.AssignParent(this);
			PemRowSourceType.AssignParent(this);
		}
	}

	public class basComboBoxTemplate : UiControlTemplate
	{
		internal basComboBoxTemplate() { }
		public basComboBoxTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basComboBox(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basComboBoxTemplate();
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
			AddProperty(KnownNti.RowSourceType, 0);
			AddProperty(KnownNti.RowSource,"");
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.ListControl;
			base.DoAddMembers(template);
			CloneMember(KnownNti.ListCount, template);
			CloneMember(KnownNti.ListIndex, template);
			CloneMember(KnownNti.DisplayValue, template);
			CloneMember(KnownNti.TabIndex, template);
		}
	}

}
