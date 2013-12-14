using System;

namespace Guineu.Core.FileSystem
{
    interface IVirtualFileSystem
    {
        String GetFullPath(String file);
    }
}