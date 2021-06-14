using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Regression.Required.Properties
{
    #region Baseline

    public class BaselineTestType<TDependency> : PatternBaseType
    {
        [Dependency] public TDependency Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class BaselineTestTypeNamed<TDependency> : PatternBaseType
    {
        [Dependency(PatternBase.Name)] public TDependency Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class BaselineTestType<TItem1, TItem2> : PatternBaseType
    {
        [Dependency] public TItem1 Item1 { get; set; }
        [Dependency] public TItem2 Item2 { get; set; }

        public override object Value => Item1;
        public override object Default => Item2;
    }

    #endregion


    #region No Public Members

    public class NoPublicMember<TDependency>
    {
        [Dependency]
        private TDependency Property { get; set; }
    }

    #endregion


    #region Object

    public class ObjectTestType : PatternBaseType
    {
        [Dependency] public object Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
    }

    #endregion


    #region Array

    public class BaselineArrayType<TDependency> : PatternBaseType
    {
        [Dependency] public TDependency[] Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class ObjectArrayType : PatternBaseType
    {
        [Dependency] public object[] Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
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
