using Guineu.Expression;


namespace Guineu.Functions
{
    class FERROR : ExpressionBase
    {
        override internal void Compile(Compiler comp)
        {
            FixedInt = true;
        }
        override internal Variant GetVariant(CallingContext context)
        {
            return new Variant(GetInt(context), 10);
        }
        internal override int GetInt(CallingContext context)
        {
            var llfmngr = GuineuInstance.FFilesManager;
            var retVal = llfmngr.Ferror();
            return retVal;
        }
    }
}
