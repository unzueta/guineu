namespace Guineu.Core.FileSystem
{
    public class VirtualFileSystem : IVirtualFileSystem
    {
        public string GetFullPath(string file)
        {
            return file;
        }
    }
}
