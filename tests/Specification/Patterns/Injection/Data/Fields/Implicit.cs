using Regression;
using Regression.Implicit.Fields;
using System;


namespace Injection.Implicit.Fields
{
    public class Inherited_Import<TDependency> : BaselineTestType<TDependency> { }
    public class Inherited_Twice<TDependency> : Inherited_Import<TDependency> { }


    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        private TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
        protected TDependency Dummy()
        {
            Field = default;
            return Field;
        }
    }

    public class ProtectedTestType<TDependency>
        : PatternBaseType
    {
        protected TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        internal TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
        protected TDependency Dummy()
        {
            Field = default;
            return Field;
        }
    }

    #endregion
}

