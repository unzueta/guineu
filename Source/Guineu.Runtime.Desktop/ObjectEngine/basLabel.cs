using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class basLabel : UiControl
	{
		VariantProperty pemTabIndex;

		public basLabel(ObjectTemplate obj)
			: base(obj)
		{ }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.LabelControl);
			AddBackStyleProperty();
			AddFontProperties();
			AddProperties();
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
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.Label);
			InitUiControl();
			Bind();
		}
		private void Bind()
		{
			pemTabIndex.AssignParent(this);
		}
	}

	public class basLabelTemplate : UiControlTemplate
	{
		internal basLabelTemplate() { }
		public basLabelTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basLabel(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basLabelTemplate();
		}
		protected override void DoAddMembers()
		{
			//// default for visually added controls (VCX, SCX)
			//DefaultWidth = 40;
			//DefaultHeight = 17;

			// default for programmatically added controla
			DefaultWidth = 100;
			DefaultHeight = 17; 
			base.DoAddMembers();
			AddProperty(KnownNti.BackStyle, 1);
			AddFontProperties();
			AddProperty(KnownNti.TabIndex, 0);
		}
		protected override void DoAddMembers(IMemberList template)
		{
			base.DoAddMembers(template);
			CloneMember(KnownNti.TabIndex, template);
		}
	}

}
