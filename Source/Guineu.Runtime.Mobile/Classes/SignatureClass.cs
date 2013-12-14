using System;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Classes
{
	class SignatureClass : UiControl
	{
		public SignatureClass(ObjectTemplate obj) : base(obj) { }

		VariantProperty pemValue;
		VariantProperty pemPenWidth;
		VariantProperty pemBorderStyle;
		VariantProperty pemSignature;

		GenericProperty borderColorProperty;

		protected override void DoInitializeInstance()
		{
			AddUserInterfaceControlProperties(
				SupportedMembers.ControlSource | SupportedMembers.Colors | SupportedMembers.ControlSource);
			
			pemValue = new VariantProperty(KnownNti.Value, new Variant(GetPropString("VALUE")));
			AddMember(pemValue);

			pemSignature = new VariantProperty(KnownNti.Signature,VariantProperty.PemStatus.ReadOnly);
			AddMember(pemSignature);

			pemPenWidth = new VariantProperty(KnownNti.PenWidth, new Variant(GetPropInt32("PENWIDTH"),10));
			AddMember(pemPenWidth);

			pemBorderStyle = new VariantProperty(KnownNti.BorderStyle, new Variant(GetPropInt32("BORDERSTYLE"), 1));
			AddMember(pemBorderStyle);
	
			borderColorProperty = new GenericProperty(KnownNti.BorderColor, GetPropVariant(KnownNti.BorderColor));
			AddMember(borderColorProperty);

			AddPictureProperty();

			DoCreateControl();
		}

		virtual internal void DoCreateControl()
		{
			View = GuineuInstance.WinMgr.CreateControl(KnownNti.Signature);
			InitUiControl();
			pemValue.AssignParent(this);
			pemSignature.AssignParent(this);
			pemPenWidth.AssignParent(this);
			pemBorderStyle.AssignParent(this);
			borderColorProperty.AssignParent(View);
		}
	}

	internal class SignatureClassTemplate : UiControlTemplate
	{
		const SupportedMembers WhichMembers = SupportedMembers.ControlSource | SupportedMembers.Colors | SupportedMembers.ControlSource;

		internal SignatureClassTemplate() { }
		internal SignatureClassTemplate(String name) : base(name) { }
		internal SignatureClassTemplate(Nti n)
			: base(n.ToString().ToUpper(System.Globalization.CultureInfo.InvariantCulture))
		{ }

		protected override ObjectBase DoCreateInstance()
		{
			return new SignatureClass(this);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new SignatureClassTemplate();
		}

		protected override void DoAddMembers()
		{
			DefaultWidth = 100;
			DefaultHeight = 45;
			UsedMembers = WhichMembers;
			AddProperty(KnownNti.PictureVal, "");
			AddProperty(KnownNti.Picture, "");
			AddProperty(KnownNti.Value, "");
			AddProperty(KnownNti.Signature, "");
			AddProperty(KnownNti.PenWidth, 1);
			AddProperty(KnownNti.BorderStyle, 1);
			// TODO: is 1 the correct value?
			AddProperty(KnownNti.BorderColor, 1);
			base.DoAddMembers();
		}

		protected override void DoAddMembers(IMemberList template)
		{
			UsedMembers = WhichMembers;
			base.DoAddMembers(template);
			CloneMember(KnownNti.Value, template);
			CloneMember(KnownNti.PenWidth, template);
			CloneMember(KnownNti.Signature, template);
			CloneMember(KnownNti.BorderStyle, template);
			CloneMember(KnownNti.BorderColor, template);
		}

	}

}
