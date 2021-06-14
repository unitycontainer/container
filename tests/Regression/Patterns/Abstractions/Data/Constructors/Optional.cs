#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif


namespace Regression.Optional.Constructors
{
    #region Baseline

    public class BaselineTestType<TDependency> : PatternBaseType
    {
        [InjectionConstructor] public BaselineTestType([OptionalDependency] TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class BaselineTestTypeNamed<TDependency>
        : PatternBaseType
    {
        public BaselineTestTypeNamed([OptionalDependency(PatternBase.Name)] TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class BaselineTestType<TItem1, TItem2> : PatternBaseType
    {
        [InjectionConstructor]
        public BaselineTestType([OptionalDependency] TItem1 item1, [OptionalDependency] TItem2 item2)
        {
            Value = item1;
            Default = item2;
        }
    }

    #endregion


    #region Object

    public class ObjectTestType : PatternBaseType
    {
        [InjectionConstructor] public ObjectTestType([OptionalDependency] object value) => Value = value;
    }

    #endregion


    #region No Public Members

    public class NoPublicMember<TDependency>
    {
        [InjectionConstructor]
        private NoPublicMember([OptionalDependency] TDependency value) { }
    }

    #endregion


    #region Array

    public class BaselineArrayType<TDependency> : PatternBaseType
    {
        [InjectionConstructor] public BaselineArrayType([OptionalDependency] TDependency[] value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class ObjectArrayType : PatternBaseType
    {
        [InjectionConstructor] public ObjectArrayType([OptionalDependency] object[] value) => Value = value;
    }

    #endregion


    #region Consumer

    public class BaselineConsumer<TDependency> : PatternBaseType
    {
        public readonly BaselineTestType<TDependency> Item1;
        public readonly BaselineTestTypeNamed<TDependency> Item2;

        public BaselineConsumer(BaselineTestType<TDependency> item1, BaselineTestTypeNamed<TDependency> item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override object Value => Item1.Value;
        public override object Default => Item2.Value;
    }

    #endregion
}
