using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basColumn : UiControl
	{
			public basColumn(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(
				SupportedMembers.Colors | SupportedMembers.ControlSource
			);
			AddMembers();
			DoCreateControl();
		}

		void AddMembers()
		{
			AddFontProperties();
			AddUserDefinedMembers();
		}

		virtual internal void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.Column);
			InitUiControl();
		}

		internal void ControlSourceValue()
		{
			ControlSourceProperty.LoadValue();
		}
	}

	internal class basColumnTemplate : UiControlTemplate
	{
		internal basColumnTemplate() { }
		internal basColumnTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basColumn(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basColumnTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 75;
			DefaultHeight = 100;
			UsedMembers = SupportedMembers.Colors | SupportedMembers.ControlSource;
			base.DoAddMembers();
			AddFontProperties();
			AddProperty(KnownNti.CurrentControl, "");
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = SupportedMembers.Colors | SupportedMembers.ControlSource;
			base.DoAddMembers(template);
		}
	}

}
