using System;
using System.Drawing;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	public abstract partial class UiControlTemplate
	{
		private static Int32 GetVfprgb(Color color)
		{
			byte r = color.R, g = color.G, b = color.B;
			return r + 256 * g + 256 * 256 * b;
		}

		private void PlatformAddMembers()
		{
			if ((UsedMembers & SupportedMembers.Colors) != 0)
			{
				if (this is BasCheckBoxTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.ControlText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Control));
				}
				else if (this is basComboBoxTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.WindowText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Window));
				}
				else if (this is basCommandButtonTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.ControlText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Control));
				}
				else if (this is basContainerTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.ControlText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Control));
				}
				else if (this is basFormTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.ControlText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Control));
				}
				else if (this is BaseImageTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.ControlText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Control));
				}
				else if (this is basLabelTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.ControlText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Control));
				}
				else if (this is basListBoxTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.WindowText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Window));
				}
				else if (this is basTextBoxTemplate)
				{
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.WindowText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Window));
				}
				else
				{
					// TODO: Doesn't work this way, because the color is type specific
					AddProperty(KnownNti.ForeColor, GetVfprgb(SystemColors.ControlText));
					AddProperty(KnownNti.BackColor, GetVfprgb(SystemColors.Control));
				}
			}
		}
	}
}