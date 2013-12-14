namespace Guineu.Package
{
    public class Package
    {
        public Package(/* GuineuInstance instance*/ )
        {
        }

        public void RegisterPackage()
        {
            RegisterCommands(GuineuInstance.CommandFactory);
        }

        internal virtual void RegisterCommands(CommandFactory commandFactory) { }
    }
}
