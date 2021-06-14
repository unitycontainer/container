#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Regression.Optional.Methods
{
    #region Baseline

    public class BaselineTestType<TDependency> : PatternBaseType
    {
        [InjectionMethod] public void Method([OptionalDependency] TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class BaselineTestTypeNamed<TDependency>
        : PatternBaseType
    {
        [InjectionMethod]
        public virtual void Method([OptionalDependency(PatternBase.Name)] TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class BaselineTestType<TItem1, TItem2> : PatternBaseType
    {
        [InjectionMethod]
        public virtual void Method([OptionalDependency] TItem1 item1, [OptionalDependency] TItem2 item2)
        {
            Value = item1;
            Default = item2;
        }
    }

    #endregion


    #region Object

    public class ObjectTestType : PatternBaseType
    {
        [InjectionMethod] public void Method([OptionalDependency] object value) => Value = value;
    }

    #endregion


    #region No Public Members

    public class NoPublicMember<TDependency>
    {
        [InjectionMethod]
        private void Method([OptionalDependency] TDependency value) { }
    }

    #endregion


    #region Array

    public class BaselineArrayType<TDependency> : PatternBaseType
    {
        [InjectionMethod] public void Method([OptionalDependency] TDependency[] value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class ObjectArrayType : PatternBaseType
    {
        [InjectionMethod] public void Method([OptionalDependency] object[] value) => Value = value;
    }

    #endregion


    #region Consumer

    public class BaselineConsumer<TDependency> : PatternBaseType
    {
        public BaselineTestType<TDependency> Item1 { get; private set; }
        public BaselineTestTypeNamed<TDependency> Item2 { get; private set; }

        [InjectionMethod]
        public void Method(BaselineTestType<TDependency> item1, BaselineTestTypeNamed<TDependency> item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override object Value => Item1.Value;
        public override object Default => Item2.Value;
    }

    #endregion
}
