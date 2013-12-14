using System;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	public abstract class UiControl : NestedClass
	{

		//===================================================================================
		// Properties that all visible controls use.
		//===================================================================================
		GenericProperty captionProperty;
		GenericProperty leftProperty;
		GenericProperty topProperty;
		GenericProperty widthProperty;
		GenericProperty heightProperty;
		GenericProperty backColorProperty;
		GenericProperty foreColorProperty;
		GenericProperty visibleProperty;
		EnabledProperty enabledProperty;
		GenericProperty backStyleProperty;
		PictureProperty pictureProperty;
		ValidEvent validEvent;
		WhenEvent whenEvent;
		GenericProperty fontBoldProperty;
		GenericProperty fontItalicProperty;
		GenericProperty fontStrikeThruProperty;
		GenericProperty fontUnderlineProperty;
		GenericProperty fontSizeProperty;
		GenericProperty fontNameProperty;
		GenericProperty readonlyProperty;

		GenericProperty valueProperty;
		internal ControlSourceProperty ControlSourceProperty;

		internal FocusManager FocusManager;


		protected UiControl(ObjectTemplate obj) : base(obj) { }

		public void Refresh()
		{
			RefreshView();
			RefreshModelObjects();
		}
		void RefreshView()
		{
			if (ControlSourceProperty != null)
				ControlSourceProperty.LoadValue();

			// Später entfernen, dann darf aber eine unbekannte Methode nichts mehr auslösen
			// Prüfen, ob es wirklich notwendig ist, Refresh aufzurufen. Müßte die Control
			// Source das nicht automatisch aktualisieren
			if(View is IGuiGrid)
			View.CallMethod(KnownNti.Refresh);
		}
		void RefreshModelObjects()
		{
			foreach (ObjectBase obj in Controls)
			{
				var ctrl = obj as UiControl;
				if (ctrl != null)
					ctrl.Refresh();
			}
		}

		public void AddDotNetControl(IControl ctrl)
		{
			if (ctrl == null)
				throw new ArgumentNullException("ctrl");

			var y = new Variant(ctrl);
			View.CallMethod(KnownNti.AddObject, new ParameterCollection(new[] { y }));
		}

		protected void AddUserInterfaceControlProperties(SupportedMembers members)
		{
			if ((members & SupportedMembers.Caption) != 0)
			{
				captionProperty = new GenericProperty(KnownNti.Caption, GetPropVariant(KnownNti.Caption));
				AddMember(captionProperty);
			}

			leftProperty = new GenericProperty(KnownNti.Left, new Variant(GetPropInt32("LEFT"), 10));
			AddMember(leftProperty);

			topProperty = new GenericProperty(KnownNti.Top, new Variant(GetPropInt32("TOP"), 10));
			AddMember(topProperty);

			widthProperty = new GenericProperty(KnownNti.Width, new Variant(GetPropInt32("WIDTH"), 10));
			AddMember(widthProperty);

			heightProperty = new GenericProperty(KnownNti.Height, new Variant(GetPropInt32("HEIGHT"), 10));
			AddMember(heightProperty);

			if ((members & SupportedMembers.Colors) != 0)
			{
				backColorProperty = new GenericProperty(KnownNti.BackColor, GetPropVariant(KnownNti.BackColor));
				AddMember(backColorProperty);
				foreColorProperty = new GenericProperty(KnownNti.ForeColor, GetPropVariant(KnownNti.ForeColor));
				AddMember(foreColorProperty);
			}

			if ((members & SupportedMembers.Value) != 0)
			{
				AddValueProperties();
			}

			if ((members & SupportedMembers.ControlSource) != 0)
			{
				ControlSourceProperty = new ControlSourceProperty(GetPropString("CONTROLSOURCE"));
				AddMember(ControlSourceProperty);
			}

			visibleProperty = new GenericProperty(KnownNti.Visible, new Variant(GetPropBoolean("VISIBLE")));
			AddMember(visibleProperty);
			enabledProperty = new EnabledProperty(GetPropBoolean("ENABLED"));
			AddMember(enabledProperty);

			Add(KnownNti.Refresh, new RefreshMethod(this));
		}

		protected void AddPictureProperty()
		{
			pictureProperty = new PictureProperty(new Variant(GetPropString("PICTURE")));
			AddMember(pictureProperty);
		}
		protected void AddBackStyleProperty()
		{
			backStyleProperty = new GenericProperty(KnownNti.BackStyle, new Variant(GetPropInt32("BACKSTYLE"), 10));
			AddMember(backStyleProperty);
		}
		protected void AddReadonlyProperty()
		{
			readonlyProperty = new GenericProperty(KnownNti.ReadOnly, GetPropVariant(KnownNti.ReadOnly));
			AddMember(readonlyProperty);
		}
		protected void AddFontProperties()
		{
			fontBoldProperty = new GenericProperty(KnownNti.FontBold, new Variant(GetPropBoolean("FONTBOLD")));
			AddMember(fontBoldProperty);
			fontItalicProperty = new GenericProperty(KnownNti.FontItalic, new Variant(GetPropBoolean("FONTITALIC")));
			AddMember(fontItalicProperty);
			fontUnderlineProperty = new GenericProperty(KnownNti.FontUnderline, new Variant(GetPropBoolean("FONTUNDERLINE")));
			AddMember(fontUnderlineProperty);
			fontStrikeThruProperty = new GenericProperty(KnownNti.FontStrikeThru, new Variant(GetPropBoolean("FONTSTRIKETHRU")));
			AddMember(fontStrikeThruProperty);
			fontSizeProperty = new GenericProperty(KnownNti.FontSize, new Variant(GetPropInt32("FONTSIZE"), 10));
			AddMember(fontSizeProperty);
			fontNameProperty = new GenericProperty(KnownNti.FontName, new Variant(GetPropString("FONTNAME")));
			AddMember(fontNameProperty);
		}
		protected void AddValidEvent()
		{
			validEvent = new ValidEvent(this);
			Add(KnownNti.Valid, validEvent);
		}
		protected void AddWhenEvent()
		{
			whenEvent = new WhenEvent(this);
			Add(KnownNti.When, whenEvent);
		}

		/// <summary>
		/// Adds properties required for Value and ControlSource
		/// </summary>
		protected void AddValueProperties()
		{
			var val = GetPropVariant(KnownNti.Value);
			valueProperty = new GenericProperty(KnownNti.Value, val);
			AddMember(valueProperty);
		}

		protected void InitUiControl()
		{
			// Newly added controls are placed in the upper left corner
			var host = Parent as UiControl;
			if (host != null)
			{
				FocusManager = host.FocusManager;
				host.AddDotNetControl(View);
				View.CallMethod(KnownNti.Move, new ParameterCollection { new Variant(0, 10), new Variant(0, 10) });
			}

			if (fontNameProperty != null)
				View.SetVariant(KnownNti.FontName, new Variant("Arial"));
			if (fontSizeProperty != null)
				View.SetVariant(KnownNti.FontSize, new Variant(9, 10));

			// Initialize control according to its settings
			if (captionProperty != null)
				captionProperty.AssignParent(View);
			leftProperty.AssignParent(View);
			topProperty.AssignParent(View);
			widthProperty.AssignParent(View);
			heightProperty.AssignParent(View);
			visibleProperty.AssignParent(View);
			enabledProperty.AssignParent(this);

			if (valueProperty != null)
				valueProperty.AssignParent(View);
			if (ControlSourceProperty != null)
				ControlSourceProperty.AssignParent(this);
			if (backColorProperty != null)
				backColorProperty.AssignParent(View);
			if (foreColorProperty != null)
				foreColorProperty.AssignParent(View);
			if (backStyleProperty != null)
				backStyleProperty.AssignParent(View);
			if (pictureProperty != null)
				pictureProperty.AssignParent(View);
			if (fontBoldProperty != null)
				fontBoldProperty.AssignParent(View);
			if (fontItalicProperty != null)
				fontItalicProperty.AssignParent(View);
			if (fontStrikeThruProperty != null)
				fontStrikeThruProperty.AssignParent(View);
			if (fontUnderlineProperty != null)
				fontUnderlineProperty.AssignParent(View);
			if (fontSizeProperty != null)
				fontSizeProperty.AssignParent(View);
			if (fontNameProperty != null)
				fontNameProperty.AssignParent(View);
			if (readonlyProperty != null)
				readonlyProperty.AssignParent(View);

			BindEvents();

		}

		void BindEvents()
		{
			if (validEvent != null)
				validEvent.Bind(View);
			if (validEvent != null)
				whenEvent.Bind(View);
		}


		/// <summary>
		/// Creates a new child control for containers that allow the creation of contained
		/// controls by increasing the number of elements, such as grids, page frames, etc.
		/// </summary>
		/// <returns></returns>
		protected virtual UiControl CreateChildControl()
		{
			return null;
		}

		public Int32 ControlCount
		{
			get { return Controls.Count; }
			set
			{
				if (Controls.Count < value)
				{
					for (Int32 val = Controls.Count; val < value; val++)
					{
						UiControl newCtrl = CreateChildControl();
						Controls.Add(newCtrl);
					}
				}
				if (Controls.Count > value)
				{
				}
			}
		}
	}

	public abstract partial class UiControlTemplate : ObjectTemplate
	{
		internal UiControlTemplate() { }
		internal UiControlTemplate(String name) : base(name) { }

		protected Int32 DefaultWidth;
		protected Int32 DefaultHeight;
		protected Boolean DefaultVisible = true;
		protected Variant DefaultValue = new Variant("");

		protected SupportedMembers UsedMembers = SupportedMembers.All;

		protected override void DoAddMembers()
		{
			if ((UsedMembers & SupportedMembers.Caption) != 0)
			{
				AddCaptionProperty(null);
			}
			if ((UsedMembers & SupportedMembers.Value) != 0)
			{
				AddProperty(KnownNti.Value, DefaultValue);
			}
			if ((UsedMembers & SupportedMembers.ControlSource) != 0)
			{
				AddProperty(KnownNti.ControlSource, "");
			}
			AddProperty(KnownNti.Tag, "");
			AddProperty(KnownNti.Left, 0);
			AddProperty(KnownNti.Top, 0);
			AddProperty(KnownNti.Width, DefaultWidth);
			AddProperty(KnownNti.Height, DefaultHeight);
			AddProperty(KnownNti.Visible, DefaultVisible);
			AddProperty(KnownNti.Enabled, true);
			PlatformAddMembers();
		}

		protected override void DoAddMembers(IMemberList template)
		{
			if ((UsedMembers & SupportedMembers.Caption) != 0)
			{
				AddCaptionProperty(template.GetMember(KnownNti.Caption) as PropertyMember);
			}
			if ((UsedMembers & SupportedMembers.Value) != 0)
			{
				Add(KnownNti.Value, template.GetMember(KnownNti.Value).Clone());
			}
			if ((UsedMembers & SupportedMembers.ControlSource) != 0)
			{
				Add(KnownNti.ControlSource, template.GetMember(KnownNti.ControlSource).Clone());
			}
			if ((UsedMembers & SupportedMembers.Colors) != 0)
			{
				Add(KnownNti.ForeColor, template.GetMember(KnownNti.ForeColor).Clone());
			}
			CloneMember(KnownNti.BackColor, template);

			Add(KnownNti.Tag, template.GetMember(KnownNti.Tag).Clone());
			Add(KnownNti.Left, template.GetMember(KnownNti.Left).Clone());
			Add(KnownNti.Top, template.GetMember(KnownNti.Top).Clone());
			Add(KnownNti.Width, template.GetMember(KnownNti.Width).Clone());
			Add(KnownNti.Height, template.GetMember(KnownNti.Height).Clone());
			Add(KnownNti.Visible, template.GetMember(KnownNti.Visible).Clone());
			Add(KnownNti.Enabled, template.GetMember(KnownNti.Enabled).Clone());
			CloneMember(KnownNti.BackStyle, template);
			CloneMember(KnownNti.Picture, template);
			CloneMember(KnownNti.WindowType, template);
			var pemPageCount = template.GetMember(KnownNti.PageCount) as PageCountPropertyTemplate;
			if (pemPageCount != null)
			{
				var pemPageCountClone = (PageCountPropertyTemplate)pemPageCount.Clone();
				Add(KnownNti.PageCount, pemPageCountClone);
				pemPageCountClone.AssignParent(this);
			}
			var pemColumnCount = template.GetMember(KnownNti.ColumnCount) as ColumnCountPropertyTemplate;
			if (pemColumnCount != null)
			{
				var pemColumnCountClone = (ColumnCountPropertyTemplate)pemColumnCount.Clone();
				Add(KnownNti.ColumnCount, pemColumnCountClone);
				pemColumnCountClone.AssignParent(this);
			}
			CloneMember(KnownNti.FontBold, template);
			CloneMember(KnownNti.FontUnderline, template);
			CloneMember(KnownNti.FontStrikeThru, template);
			CloneMember(KnownNti.FontItalic, template);
			CloneMember(KnownNti.FontSize, template);
			CloneMember(KnownNti.FontName, template);
			CloneMember(KnownNti.ReadOnly, template);
		}


		private void AddCaptionProperty(PropertyMember prop)
		{
			String name = Nti.ToString();
			if (name.Length == 1)
			{
				name = name.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
			}
			else
			{
				name = name.Substring(0, 1).ToUpper(System.Globalization.CultureInfo.InvariantCulture) + name.Substring(1).ToLower(System.Globalization.CultureInfo.InvariantCulture);
			}
			var newProp = new PropertyMember(new Variant(name));

			// If the caption property has changed from its default, use that value.
			if (prop != null && !prop.IsDefault)
			{
				newProp.Set(prop.Get());
			}
			Add(KnownNti.Caption, newProp);
		}

		protected void AddFontProperties()
		{
			AddProperty(KnownNti.FontBold, false);
			AddProperty(KnownNti.FontUnderline, false);
			AddProperty(KnownNti.FontStrikeThru, false);
			AddProperty(KnownNti.FontItalic, false);
			AddProperty(KnownNti.FontName, "Arial");
			AddProperty(KnownNti.FontSize, 9);
		}

		Int32 memberControlCount;

		public Int32 ControlCount
		{
			get { return memberControlCount; }
			set
			{
				if (memberControlCount < value)
					for (Int32 val = memberControlCount; val < value; val++)
					{
						AddChildControl();
					}
				if (memberControlCount > value)
				{
				}
				memberControlCount = value;
			}
		}

		protected virtual void AddChildControl()
		{
		}
	}

	[Flags]
	public enum SupportedMembers
	{
		None = 0,
		Caption = 1,
		Value = 2,
		ControlSource = 4,
		Colors = 8,
		All = Caption | Value | Colors,
		LabelControl = Caption | Colors,
		TextControl = Value | ControlSource | Colors,
		ListControl = TextControl,
		ButtonControl = Caption | Colors,
		CheckControl = LabelControl | Value,
		FormControl = Caption | Colors,
	}

}