using Guineu.Classes;

namespace Guineu.ObjectEngine
{
	public sealed partial class ObjectFactory
	{
		partial void RegisterBaseClasses()
		{
			classes.RegisterBaseClass(new BasCheckBoxTemplate("CHECKBOX"));
			classes.RegisterBaseClass(new basCommandButtonTemplate("COMMANDBUTTON"));
			classes.RegisterBaseClass(new basContainerTemplate("CONTAINER"));
			classes.RegisterBaseClass(new basComboBoxTemplate("COMBOBOX"));
			classes.RegisterBaseClass(new basCustomTemplate("CUSTOM"));
			classes.RegisterBaseClass(new basFormTemplate("FORM"));
			classes.RegisterBaseClass(new BaseImageTemplate("IMAGE"));
			classes.RegisterBaseClass(new basLabelTemplate("LABEL"));
			classes.RegisterBaseClass(new basPageTemplate("PAGE"));
			classes.RegisterBaseClass(new basPageframeTemplate("PAGEFRAME"));
			classes.RegisterBaseClass(new basListBoxTemplate("LISTBOX"));
			classes.RegisterBaseClass(new basTextBoxTemplate("TEXTBOX"));
			classes.RegisterBaseClass(new EdtiboxClassTemplate("EDITBOX"));
			classes.RegisterBaseClass(new basShapeTemplate("SHAPE"));
			classes.RegisterBaseClass(new basSpinnerTemplate("SPINNER"));
			classes.RegisterBaseClass(new TimerClassTemplate("TIMER"));
		}
	}
}