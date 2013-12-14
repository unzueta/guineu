using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Guineu.Gui.Desktop
{
	class ImageHandling
	{
		private PictureBox host;

		public ImageHandling(PictureBox ctrl)
		{
			host = ctrl;
		}

		public Image GetPicture()
		{
			String picture = Picture;
			if (String.IsNullOrEmpty(PictureVal))
				if (String.IsNullOrEmpty(picture))
					return null;
				else
				{
					Stream s = GuineuInstance.FileMgr.Open(
							picture,
							FileMode.Open,
							FileAccess.Read,
							FileShare.Read
					);
					using (s)
						return new Bitmap(s);
				}
			using (Stream s = new MemoryStream(GuineuInstance.CurrentCp.GetBytes(PictureVal)))
				return new Bitmap(s);
		}

		public void ShowPicture()
		{
			var img = GetPicture();
			if (Stretch == 0)
			{
				host.Size = img.Size;
				host.SizeMode = PictureBoxSizeMode.Normal;
			}
			else
				host.SizeMode = PictureBoxSizeMode.StretchImage;
			host.Image = img;
		}

		// TODO: PictureVal can be an object (IPicture)
		public String PictureVal { get; set; }
		public String Picture { get; set; }
		public Int32 Stretch { get; set; }
	}
}
