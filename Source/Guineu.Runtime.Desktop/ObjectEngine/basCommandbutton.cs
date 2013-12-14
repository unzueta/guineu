using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	partial class basCommandButton : UiControl
	{
		ClickMethod clickEvent;
		PictureProperty pictureProperty;
		DownPictureProperty downPicture;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		GenericProperty wordWrapProperty;
		VariantProperty pemTabIndex;

		public basCommandButton(ObjectTemplate obj)
			: base(obj)
		{ }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.ButtonControl);
			AddFontProperties();
			AddMethods();
			AddImageProperties();
			AddWordWrapProperty();
			AddProperties();
			AddEvents();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		void AddProperties()
		{
			pemTabIndex = new VariantProperty(KnownNti.TabIndex, new Variant(GetPropInt32("TABINDEX"), 10));
			AddMember(pemTabIndex);
		}
		private void AddWordWrapProperty()
		{
			wordWrapProperty = new GenericProperty(KnownNti.WordWrap, GetPropVariant(KnownNti.WordWrap));
			AddMember(wordWrapProperty);
		}

		private void AddEvents()
		{
			AddValidEvent();
			AddWhenEvent();
		}

		void AddImageProperties()
		{
			pictureProperty = new PictureProperty(new Variant(GetPropString("PICTURE")));
			AddMember(pictureProperty);
			downPicture = new DownPictureProperty(GetPropString("DOWNPICTURE"));
			AddMember(downPicture);
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

		virtual internal void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.CommandButton);
			InitUiControl();
			InitImageControl();
			Bind();
		}

		private void InitImageControl()
		{
			pictureProperty.AssignParent(View);
			downPicture.AssignParent(this);
		}

		private void Bind()
		{
			clickEvent.Bind(View);
			gotFocusEvent.Bind(View);
			lostFocusEvent.Bind(View);
			wordWrapProperty.AssignParent(View);
			pemTabIndex.AssignParent(this);
		}

	}

	public class basCommandButtonTemplate : UiControlTemplate
	{
		internal basCommandButtonTemplate() { }
		public basCommandButtonTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basCommandButton(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basCommandButtonTemplate();
		}
		
		protected override void DoAddMembers()
		{
			//// default for visually added buttons (VCX, SCX)
			//DefaultWidth = 84;
			//DefaultHeight = 27;

			// default for programmatically added buttons
			DefaultWidth = 100;
			DefaultHeight = 17;
			AddProperty(KnownNti.Picture, "");
			AddProperty(KnownNti.DownPicture, "");
			AddProperty(KnownNti.WordWrap, false);
			base.DoAddMembers();
			AddFontProperties();
			AddProperty(KnownNti.TabIndex, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			base.DoAddMembers(template);
			Add(KnownNti.DownPicture, template.GetMember(KnownNti.DownPicture).Clone());
			CloneMember(KnownNti.WordWrap, template);
			if (template.GetMember(KnownNti.Click) != null)
			{
				Add(KnownNti.Click, template.GetMember(KnownNti.Click).Clone());
			}
			CloneMember(KnownNti.TabIndex, template);
		}
	}

}
