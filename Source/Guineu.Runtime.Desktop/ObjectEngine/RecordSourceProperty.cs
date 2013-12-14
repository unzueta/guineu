using System;
using Guineu.Data;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	//TODO: This class should inherit from Member to avoid having to use new
	class RecordSourceProperty : StringProperty
	{
	    readonly String initialValue;
		UiControl owner;
        protected string Name { get; set; }

		public RecordSourceProperty(String value)
		{
			initialValue = value;
			Name = "RECORDSOURCE";
		}


	    public void AssignParent(UiControl ctrl)
		{
			owner = ctrl;
			if (!String.IsNullOrEmpty(initialValue))
			{
				ICursor recordSource = GetCursor(new Nti(initialValue));
				((IGuiGrid) owner.View).GuiRecordSource = recordSource;
			}
		}

		public override Variant Get()
		{
			ICursor cursor = ((IGuiGrid)owner.View).GuiRecordSource;
			if(cursor == null)
				return new Variant("");
			return new Variant(cursor.Alias.ToString());
		}

		public override void Set(Variant value)
		{
			ICursor recordSource;
			String alias = value;
			if (String.IsNullOrEmpty(alias))
				recordSource = null;
			else
				recordSource = GetCursor(new Nti(alias));
			((IGuiGrid)owner.View).GuiRecordSource = recordSource;
		}

	    static ICursor GetCursor(Nti recordSource)
		{
			ICursor cursor = GuineuInstance.CallingContext.DataSession.GetExistingCursor(recordSource);
			return cursor;
		}

	}

}
