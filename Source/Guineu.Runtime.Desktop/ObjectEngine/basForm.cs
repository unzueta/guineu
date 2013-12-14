using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal partial class basForm : UiControl
	{
		ClickMethod clickEvent;
		GotFocusEvent gotFocusEvent;
		LostFocusEvent lostFocusEvent;
		UnloadEvent pemUnloadEvent;
		VariantProperty pemScrollBars;
		GenericProperty controlBoxProperty;
	
		public basForm(ObjectTemplate obj) : base(obj) 
		{
			EnableThisform();
		}

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(SupportedMembers.FormControl);
			AddPictureProperty();
			AddFontProperties();
			AddProperties();
			AddMethods();
			AddUserDefinedMembers();
			DoCreateControl();
			var okToLoad = ExecuteLoadMethod();
		}

		void AddProperties()
		{
			pemScrollBars = new VariantProperty(KnownNti.ScrollBars, new Variant(GetPropInt32("SCROLLBARS"), 10))
			{
				Validator = p => (p >= 0 && p <= 3)
			};
			controlBoxProperty = new GenericProperty(KnownNti.ControlBox, new Variant(GetPropBoolean(KnownNti.ControlBox)));
			AddMember(pemScrollBars);
		}
		
		Boolean ExecuteLoadMethod()
		{
			// (...) Executing methods needs to be done in a central place. Also
			// (...) still need to handle DODEFAULT()
			ObjectBase obj = GetCodeBase(KnownNti.Load);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(KnownNti.Load);
				using (var ctx = new CallingContext(GuineuInstance.Context, this))
				{
					var parms = new ParameterCollection();
					Variant result = ctx.Context.ExecuteInNewContext(m.Code, parms, this);
					if (result.Type == VariantType.Logical)
						return result;
				}
			}
			return true;
		}
	

		void AddMethods()
		{
			Add(KnownNti.Release, new FormReleaseMethod(this));
			Add(KnownNti.Show, new FormShowMethod(this));

			clickEvent = new ClickMethod(this);
			Add(KnownNti.Click, clickEvent);
			
			gotFocusEvent = new GotFocusEvent(this);
			Add(KnownNti.GotFocus, gotFocusEvent);

			lostFocusEvent = new LostFocusEvent(this);
			Add(KnownNti.LostFocus, lostFocusEvent);

			pemUnloadEvent = new UnloadEvent(this);
			Add(KnownNti.Unload, pemUnloadEvent);
		}

		virtual internal void DoCreateControl()
		{
		    View = GuineuInstance.WinMgr.CreateControl(KnownNti.Form);
      FocusManager = new FocusManager();
			InitUiControl();
			Bind();
		}

		private void Bind()
		{
			clickEvent.Bind(View);
			gotFocusEvent.Bind(View);
			lostFocusEvent.Bind(View);
			pemUnloadEvent.Bind();
			pemScrollBars.AssignParent(this);
			controlBoxProperty.AssignParent(View);
		}

	}

	public class basFormTemplate : UiControlTemplate
	{
		internal basFormTemplate() { }
		public basFormTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new basForm(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new basFormTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 375;
			DefaultHeight = 250;
			DefaultVisible = false;
			//TODO: Check if we need to add SHOW and RELEASE to the template and how we would do it.
			base.DoAddMembers();
			AddProperty(KnownNti.Picture, "");
			AddProperty(KnownNti.WindowType, 0);
			AddProperty(KnownNti.ScrollBars, 0);
			AddProperty(KnownNti.ControlBox, true);
			AddFontProperties();
		}

		protected override void DoAddMembers(IMemberList template)
		{
			base.DoAddMembers(template);
			CloneMember(KnownNti.ScrollBars, template);
			CloneMember(KnownNti.ControlBox, template);
		}
	}

}
