using System;


namespace Regression.Implicit.Properties
{
    #region Baseline

    public class BaselineTestType<TDependency> : PatternBaseType
    {
        public TDependency Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class BaselineTestTypeNamed<TDependency> : PatternBaseType
    {
        public TDependency Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class BaselineTestType<TItem1, TItem2> : PatternBaseType
    {
        public TItem1 Item1 { get; set; }
        public TItem2 Item2 { get; set; }

        public override object Value => Item1;
        public override object Default => Item2;
    }

    #endregion


    #region Object

    public class ObjectTestType : PatternBaseType
    {
        public object Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
    }

    #endregion


    #region No Public Members

    public class NoPublicMember<TDependency>
    {
        private TDependency Property { get; set; }
    }

    #endregion


    #region Array

    public class BaselineArrayType<TDependency> : PatternBaseType
    {
        public TDependency[] Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class ObjectArrayType : PatternBaseType
    {
        public object[] Property { get; set; }
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
