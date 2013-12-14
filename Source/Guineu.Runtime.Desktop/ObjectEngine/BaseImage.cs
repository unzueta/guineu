using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal class BaseImage : UiControl
	{
		GenericProperty stretchProperty;
		GenericProperty pictureValProperty;
		ClickMethod clickEvent;

		public BaseImage(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.None);
			AddPictureProperty();
			AddPictureValProperty();
			AddStretchProperty();
			AddMethods();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		private void AddMethods()
		{
			clickEvent = new ClickMethod(this);
			Add(KnownNti.Click, clickEvent);
		}

		private void AddPictureValProperty()
		{
			pictureValProperty = new GenericProperty(KnownNti.PictureVal, new Variant(GetPropString("PICTUREVAL")));
			AddMember(pictureValProperty);
		}

		void AddStretchProperty()
		{
			stretchProperty = new GenericProperty(KnownNti.Stretch, new Variant(GetPropInt32("STRETCH"),10));
			AddMember(stretchProperty);
		}

		virtual internal void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.Image);
				InitUiControl();
			Bind();
		}

		void Bind()
		{
			pictureValProperty.AssignParent(View);
			stretchProperty.AssignParent(View);
			clickEvent.Bind(View);
		}
	}

	internal class BaseImageTemplate : UiControlTemplate
	{
		internal BaseImageTemplate() { }
		internal BaseImageTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new BaseImage(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new BaseImageTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 100;
			DefaultHeight = 17;
			UsedMembers = SupportedMembers.None;
			AddProperty(KnownNti.PictureVal, "");
			AddProperty(KnownNti.Stretch, 0);
			base.DoAddMembers();
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.None;
			base.DoAddMembers(template);
			CloneMember(KnownNti.Stretch, template);
			CloneMember(KnownNti.PictureVal, template);
		}

	}

}
