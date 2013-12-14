using System;
using System.IO;

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
					 FileShare.ReadWrite,
					 16,
					 FileOptions.RandomAccess
				 );
		}
	}
}
