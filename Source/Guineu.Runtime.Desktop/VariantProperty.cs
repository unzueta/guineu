using System;
using System.Collections.Generic;
using System.Diagnostics;
using Guineu.ObjectEngine;
using Guineu.Expression;
using Guineu.Gui;
using Guineu.Util;

namespace Guineu
{
    internal class VariantProperty : PropertyMember
    {
        readonly Variant value;
        IControl owner;
    	readonly Boolean readOnly;

        internal Func<Variant, bool> Validator;

        #region Constructors
        public VariantProperty(KnownNti n)
        {
        	Nti = n;
        }
        public VariantProperty(KnownNti n, Variant value)
            : this(n)
        {
            Validate(value);
            this.value = value;
        }
        public VariantProperty(KnownNti n, PemStatus p)
            : this(n)
        {
            if (Enum<PemStatus>.IsSet(p, PemStatus.ReadOnly))
                readOnly = true;
        }
        #endregion

        void Validate(Variant val)
        {
            if (Validator != null)
                if (!Validator(val))
                    // (...) Error 1881 obj - Name - property evaluated to illegal expression
                    throw new ErrorException(ErrorCodes.InvalidArgument);
        }

        public void AssignParent(NestedClass ctrl)
        {
            owner = ctrl.View;
            if (!readOnly)
                Set(value);
        }
        public override void Set(Variant val)
        {
            if (readOnly)
                throw new ErrorException(ErrorCodes.PropertyIsReadOnly);

            Validate(val);
            owner.SetVariant(Nti, val);
        }

        public override Variant Get()
        {
            return owner.GetVariant(Nti);
        }

			[Flags]
        public enum PemStatus
        {
            ReadOnly = 1
        }
    }
}