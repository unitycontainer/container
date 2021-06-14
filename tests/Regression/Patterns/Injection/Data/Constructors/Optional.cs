using Regression;
using Regression.Implicit.Constructors;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Injection.Optional.Constructors
{
    public class Inherited_Import<TDependency> : BaselineTestType<TDependency>
    {
        [InjectionConstructor] public Inherited_Import([OptionalDependency] TDependency value) : base(value) { }
    }

    public class Inherited_Twice<TDependency> : Inherited_Import<TDependency>
    {
        [InjectionConstructor] public Inherited_Twice([OptionalDependency] TDependency value) : base(value) { }
    }


    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        private PrivateTestType([OptionalDependency] TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class ProtectedTestType<TDependency>
        : PatternBaseType
    {
        protected ProtectedTestType([OptionalDependency] TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        internal InternalTestType([OptionalDependency] TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class BaselineTestType_Ref<TDependency>
        : PatternBaseType where TDependency : class
    {
        public BaselineTestType_Ref([OptionalDependency] ref TDependency _)
            => throw new InvalidOperationException("should never execute");
    }

    public class BaselineTestType_Out<TDependency>
        : PatternBaseType where TDependency : class
    {
        public BaselineTestType_Out([OptionalDependency] out TDependency _)
            => throw new InvalidOperationException("should never execute");
    }

    #endregion
}
