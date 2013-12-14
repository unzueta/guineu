using System.Text;
using Guineu.Expression;
using Microsoft.WindowsCE.Forms;

namespace Guineu.Functions
{
	/// <summary>
	/// HTTP access
	/// </summary>
	 partial class SYS8012
	{
		static partial	void DoLoadSipController()
		{
			if(Controller==null)
			Controller = new CompactSipController();
		}
	}

	class CompactSipController : ISipController
	{
		public bool Enabled
		{
			get
			{
				using (var ip = new Microsoft.WindowsCE.Forms.InputPanel())
					return ip.Enabled;
			}
			set
			{
				using (var ip = new Microsoft.WindowsCE.Forms.InputPanel())
					ip.Enabled = value;
			}
		}

		public string Current
		{
			get
			{
				using (var ip = new InputPanel())
					return ip.CurrentInputMethod.Name;
			}
			set
			{
				using (var ip = new InputPanel())
				{
					var nim = new Nti(value);
					foreach (InputMethod im in ip.InputMethods)
					{
						if (new Nti(im.Name) == nim)
							ip.CurrentInputMethod = im;
					}
				}
			}
		}

		public string Available
		{
			get {
				var str = new StringBuilder();
				using (var ip = new InputPanel())
				{
					foreach (InputMethod im in ip.InputMethods)
					{
						if (str.Length != 0)
							str.Append(", ");
						str.Append(im.Name);
					}
				}
				return str.ToString();
			}
		}
	}
}