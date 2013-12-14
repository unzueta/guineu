using System;
using System.Collections.Generic;
using Guineu.Properties;

namespace Guineu.Expression
{
    class CMONTH : ExpressionBase
    {
        ExpressionBase dateExpression;

        override internal void Compile(Compiler comp)
        {
            // Prüfen Anzahl der Parameter     
            List<ExpressionBase> param = comp.GetParameterList();
            switch (param.Count)
            {
                case 0:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 1:
                    dateExpression = param[0];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }

        }

        override internal Variant GetVariant(CallingContext context)
        {
            // Parameter prüfen. Wird .NULL. als Parameter übergeben, 
            // kann gleich mit eine .NULL. zurückgegeben werden. 
            Variant p1 = dateExpression.GetVariant(context);
            if (p1.IsNull)
                return p1; 

            // Ermitteln des Rückgabewertes und weiter reichen 
            return new Variant(GetString(context));
        }

        internal override string GetString(CallingContext context)
        {
            Variant p1 = dateExpression.GetVariant(context);

            // Prüfen auf Null. Wenn im Parameter NULL übergeben wurden, 
            // dann wird ein Leerstring zurück gegeben.
            if (p1.IsNull)
                return "";

            // Datentyp des Parameters prüfen 
            if (!((p1.Type == VariantType.Date) || (p1.Type == VariantType.DateTime)))
                throw new ErrorException(ErrorCodes.DataTypeInvalid);

            // Wenn das Programm bis hier her kommt, dann wird 
            // versucht den Monat zu ermitteln. 
            string retVal = "";
            int month = ((DateTime)p1).Month;

            switch (month)
            {
                case 1: retVal = Resources.F_CMonth_January;
                    break;
                case 2: retVal = Resources.F_CMonth_February;
                    break;
                case 3: retVal = Resources.F_CMonth_March;
                    break;
                case 4: retVal = Resources.F_CMonth_April;
                    break;
                case 5: retVal = Resources.F_CMonth_May;
                    break;
                case 6: retVal = Resources.F_CMonth_June;
                    break;
                case 7: retVal = Resources.F_CMonth_July;
                    break;
                case 8: retVal = Resources.F_CMonth_August;
                    break;
                case 9: retVal = Resources.F_CMonth_September;
                    break;
                case 10: retVal = Resources.F_CMonth_October;
                    break;
                case 11: retVal = Resources.F_CMonth_November;
                    break;
                case 12: retVal = Resources.F_CMonth_December;
                    break;
            }

            return retVal;
        }
    }
}