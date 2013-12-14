using System;
using Guineu.Gui;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basGrid : UiControl
	{
		ColumnCountProperty columnCountProperty;
		RecordSourceProperty recordSourceProperty;
		GenericProperty gridLineColorProperty;
		VariantProperty tabIndexProperty;

		public basGrid(ObjectTemplate obj) : base(obj) { }

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.Colors | SupportedMembers.Value);
			AddColumnCountProperty();
			AddRecordSourceProperty();
			AddGridLineColorProperty();
			AddProperties();
			AddMethods();
			AddUserDefinedMembers();
			DoCreateControl();
			var grd = View as IGuiGrid;
			if (grd != null) grd.GuiPreInit();
		}

		void AddProperties()
		{
			tabIndexProperty = new VariantProperty(KnownNti.TabIndex, new Variant(GetPropInt32("TABINDEX"), 10));
			AddMember(tabIndexProperty);
		}

		void AddMethods()
		{
			Add(KnownNti.SetFocus, new SetFocusMethod(this));
		}

		protected override void DoInitializeInstanceAfter(ParameterCollection p)
		{
			var grd = View as IGuiGrid;
			if (grd != null) grd.GuiPostInit();
			base.DoInitializeInstanceAfter(p);
		}

		void AddColumnCountProperty()
		{
			columnCountProperty = new ColumnCountProperty(GetPropInt32("COLUMNCOUNT"));
			AddMember(columnCountProperty);
		}

		void AddGridLineColorProperty()
		{
			gridLineColorProperty = new GenericProperty(KnownNti.GridLineColor, GetPropVariant(KnownNti.GridLineColor));
			AddMember(gridLineColorProperty);
		}

		void AddRecordSourceProperty()
		{
			recordSourceProperty = new RecordSourceProperty(GetPropString("RECORDSOURCE"));
			AddMember(recordSourceProperty);
		}

		virtual internal void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.Grid);
			InitUiControl();
			Bind();
		}

		private void Bind()
		{
			columnCountProperty.AssignParent(this);
			recordSourceProperty.AssignParent(this);
			gridLineColorProperty.AssignParent(View);
			tabIndexProperty.AssignParent(this);
		}

	}

	public class basGridTemplate : UiControlTemplate
	{
		internal basGridTemplate() { }
		public basGridTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basGrid(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basGridTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 320;
			DefaultHeight = 200;
			var pemColumnCount = new ColumnCountPropertyTemplate(0);
			AddMember(pemColumnCount);
			base.DoAddMembers();
			pemColumnCount.AssignParent(this);
			AddProperty(KnownNti.RecordSource, "");
			AddProperty(KnownNti.GridLineColor, 0);
			AddProperty(KnownNti.TabIndex, 0);
		}
		
		protected override void DoAddMembers(IMemberList template)
		{
			base.DoAddMembers(template);
			CloneMember(KnownNti.RecordSource, template);
			CloneMember(KnownNti.GridLineColor, template);
			CloneMember(KnownNti.TabIndex, template);
		}

		protected override void AddChildControl()
		{
			String name = "COLUMN1";
			for (Int32 ctrl = 1; ctrl <= Controls.Count + 1; ctrl++)
			{
				name = "COLUMN" + ctrl;
				if (GetMember(new Nti(name)) == null)
					break;
			}
			GuineuInstance.ObjectFactory.AddObject(this, GuineuInstance.CallingContext, KnownNti.Column, new Nti(name));
		}
	}

}
