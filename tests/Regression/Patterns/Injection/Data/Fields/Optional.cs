using System;
using Regression;
using Regression.Implicit.Fields;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Injection.Optional.Fields
{
    public class Inherited_Import<TDependency> : BaselineTestType<TDependency> { }
    public class Inherited_Twice<TDependency> : Inherited_Import<TDependency> { }


    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        [OptionalDependency] private TDependency Field;

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
        [OptionalDependency] protected TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        [OptionalDependency] internal TDependency Field;

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
