using System;
using Regression;
using Regression.Implicit.Fields;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Injection.Required.Fields
{
    public class Inherited_Import<TDependency> : BaselineTestType<TDependency> { }
    public class Inherited_Twice<TDependency> : Inherited_Import<TDependency> { }


    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] private TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
        protected TDependency Dummy() => (Field = default);
    }

    public class ProtectedTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] protected TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] internal TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
        protected TDependency Dummy() => (Field = default);
    }

    #endregion
}
