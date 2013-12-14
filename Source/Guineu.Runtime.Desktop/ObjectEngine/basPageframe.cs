using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basPageframe : UiControl
	{
		PageCountProperty pemPageCount;
		ActivePageProperty pemActivePage;
		VariantProperty pemTabIndex;

		public basPageframe(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.None);
			AddPageCountProperty();
			AddProperties();
			AddUserDefinedMembers();
			DoCreateControl();
		}

		void AddProperties()
		{
			pemTabIndex = new VariantProperty(KnownNti.TabIndex, new Variant(GetPropInt32(KnownNti.TabIndex), 10));
			AddMember(pemTabIndex);
		}
		void AddPageCountProperty()
		{
			pemPageCount = new PageCountProperty(GetPropInt32(KnownNti.PageCount));
			AddMember(pemPageCount);
			pemActivePage = new ActivePageProperty(GetPropInt32(KnownNti.ActivePage));
			AddMember(pemActivePage);
		}

		virtual internal void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.PageFrame);
			InitUiControl();
			Bind();
		}

		protected override void DoInitializeInstanceAfter(ParameterCollection p)
		{
			base.DoInitializeInstanceAfter(p);

			// Workaround: This code is necessary to display the contents of the first
			// page of a page frame on a mobile device. Without this code, the page
			// remains empty until the user switches the page manually.
			var o = View as TabControl;
			if (o != null)
			{
				o.SelectedIndex = -1;
				o.SelectedIndex = 0;
			}
		}

		private void Bind()
		{
			pemPageCount.AssignParent(this);
			pemActivePage.AssignParent(this);
			pemTabIndex.AssignParent(this);
		}

	}

	public class basPageframeTemplate : UiControlTemplate
	{
		internal basPageframeTemplate() { }
		public basPageframeTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basPageframe(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basPageframeTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 375;
			DefaultHeight = 254;
			var pemPageCount = new PageCountPropertyTemplate(0);
			AddMember(pemPageCount);
			base.DoAddMembers();
			pemPageCount.AssignParent(this);
			AddProperty(KnownNti.ActivePage, 0);
			AddProperty(KnownNti.TabIndex, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			base.DoAddMembers(template);
			CloneMember(KnownNti.ActivePage, template);
			CloneMember(KnownNti.TabIndex, template);
		}

		protected override void AddChildControl()
		{
			String name = "PAGE1";
			for (Int32 ctrl = 1; ctrl <= Controls.Count + 1; ctrl++)
			{
				name = "PAGE" + ctrl;
				if (GetMember(new Nti(name)) == null)
					break;
			}
			GuineuInstance.ObjectFactory.AddObject(this, GuineuInstance.CallingContext, KnownNti.Page, new Nti(name));
		}
	}

}
