using System;
using System.IO;
using Guineu.Data.Engines.Dbf.Index;
using Guineu.Util;

namespace Guineu.Data.Dbf
{
	/// <summary>
	/// Provides access to a CDX index file
	/// </summary>
	public partial class IndexFile : IDisposable
	{
		Stream stream;
		readonly Object streamLock;
		readonly IndexNodeReader nodes;
		readonly IndexHeader header;

		public IndexFile(String fileName)
		{
			streamLock = new Object();
			OpenIndexFile(fileName);
			nodes = new IndexNodeReader(stream,this);
			lock (streamLock)
			{
				stream.Seek(0, SeekOrigin.Begin);
				if (header == null)
					header = new IndexHeader(StreamUtil.Read(stream, 1024));
			}
		}

		public IndexTag GetTag(String name)
		{
			var root = new IndexTag(nodes, 0);
			KeyItem key = root.KeyItem.New(name);
			SeekResult tagNode = root.Seek(key, SeekOptions.Exact);
			if (tagNode.Found)
				return new IndexTag(nodes, (Int32)tagNode.Record);
			
			return null;
		}

		/// <summary>
		/// Reads the first 16 bytes of an index file to update root block and changed flag.
		/// </summary>
		internal void QuickRefreshHeader()
		{
			lock (streamLock)
			{
				stream.Seek(0, SeekOrigin.Begin);
				header.Refresh(StreamUtil.Read(stream, 16));
			}
		}

		#region Dispose pattern
		private bool disposed;

        public void Dispose()
        {
            Dispose(true);
       			GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
                if(disposing)
                {
									stream.Close();
									header.Dispose();
                }
            disposed = true;
				}
		#endregion
	}

	[Flags]
	enum IndexOptions
	{
		Unique = 1,
		
		/// <summary>
		/// Index expression can be .NULL. All non-NULL values are preceded
		/// with an underscore, e.g. "_PARIS" for UPPER(City).
		/// </summary>
		/// <remarks>This value is not documented in the VFP help file. 
		/// I had to figure it out myself.</remarks>
		Null = 2,
		HasForClause = 8,
		CompactIndexFormat = 32,
		CompundIndexHeader = 64
	}
}
