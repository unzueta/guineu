using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Core;

namespace Guineu.Data.Dbf
{
	public partial class IndexFile
	{
		private void OpenIndexFile(String filename)
		{
			stream = GuineuInstance.FileMgr.Open(
					 filename,
					 FileMode.Open,
					 FileAccess.ReadWrite,
					 FileShare.ReadWrite
				 );
		}
	}
}
