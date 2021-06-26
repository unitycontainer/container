using System;
using Regression;
using System.ComponentModel;
using static Dependency.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Dependency.Required.Properties
{
    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] private TDependency Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class ProtectedTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] protected TDependency Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] internal TDependency Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    #endregion
}


namespace Dependency.Required.Properties.WithDefault
{
    // Unity does not support implicit default values on properties
    // When resolved it will throw if not registered
    //
    //public class Required_WithDefault : PatternBaseType
    //{
    //    [Dependency] public int Property { get; set; } = PatternBase.DefaultInt;
    //}
}


namespace Dependency.Required.Properties.WithDefaultAttribute
{

#if !BEHAVIOR_V5 // Unity v5 did not support DefaultValueAttribute on properties
    public class Required_Property_Int_WithDefaultAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueInt)] public int Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Property_WithDefaultAttribute_Int : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueInt)] [Dependency] public int Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Property_String_WithDefaultAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueString)] public string Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Property_WithDefaultAttribute_String : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueString)] [Dependency] public string Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Property_Derived_WithDefaultAttribute
        : Required_Property_Int_WithDefaultAttribute
    {
    }
#endif
}


namespace Dependency.Required.Properties.WithDefaultAndAttribute
{
#if !BEHAVIOR_V5 // Unity v5 did not support DefaultValueAttribute on properties

    public class Required_Property_Int_WithDefaultAndAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueInt)] public int Property { get; set; } = Pattern.DefaultInt;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Property_WithDefaultAndAttribute_Int : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueInt)] [Dependency] public int Property { get; set; } = Pattern.DefaultInt;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Property_String_WithDefaultAndAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueString)] public string Property { get; set; } = Pattern.DefaultString;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Property_WithDefaultAndAttribute_String : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueString)] [Dependency] public string Property { get; set; } = Pattern.DefaultString;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Property_Derived_WithDefaultAndAttribute : Required_Property_Int_WithDefaultAndAttribute
    {
    }
#endif
}

