using System.Drawing;
using System.Windows.Forms;

namespace Guineu.Gui.Desktop
{
    // TODO: Rename to ControlExtension
    static class PositionExtension
	{
		static public void MoveControl(this Control ctrl, ParameterCollection parms)
		{
			switch (parms.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					ctrl.Location = new Point(parms[0].Get(), ctrl.Top);
					break;
				case 2:
					ctrl.Location = new Point(parms[0].Get(), parms[1].Get());
					break;
				case 3:
					ctrl.Location = new Point(parms[0].Get(), parms[1].Get());
					ctrl.Size = new Size(parms[2].Get(), ctrl.Height);
					break;
				case 4:
					ctrl.Location = new Point(parms[0].Get(), parms[1].Get());
					ctrl.Size = new Size(parms[2].Get(), parms[3].Get());
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		static public void SetLeft(this Control ctrl, Variant value)
		{
			ctrl.Location = new Point(value, ctrl.Top);
		}
		static public void SetTop(this Control ctrl, Variant value)
		{
			ctrl.Location = new Point(ctrl.Left, value);
		}
		static public void SetWidth(this Control ctrl, Variant value)
		{
			ctrl.Size = new Size(value, ctrl.Height);
		}
		static public void SetHeight(this Control ctrl, Variant value)
		{
			ctrl.Size = new Size(ctrl.Width, value);
		}

		static public void AddControl(this Control me, ParameterCollection parms)
		{
            var ctrl = (parms[0].Get().ToNative()) as Control;
            if (ctrl == null)
                throw new ErrorException(ErrorCodes.ObjectClassInvalid);
            me.Controls.Add(ctrl);
            ctrl.BringToFront();
					
		}
	}
}
