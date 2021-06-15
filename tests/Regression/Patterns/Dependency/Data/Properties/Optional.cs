using System;
using Regression;
using System.ComponentModel;
using static Dependency.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Dependency.Optional.Properties
{
    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        [OptionalDependency] private TDependency Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class ProtectedTestType<TDependency>
        : PatternBaseType
    {
        [OptionalDependency] protected TDependency Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        [OptionalDependency] internal TDependency Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    #endregion
}


namespace Dependency.Optional.Properties.WithDefault
{
#if !BEHAVIOR_V4 // v4 did not support optional value types
    public class Optional_Property_Int_WithDefault : DependencyBaseType
    {
        [OptionalDependency] public int Property { get; set; } = Pattern.DefaultInt;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V5 // Unity v5 did not support default values for Properties
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_DerivedFromInt_WithDefault : Optional_Property_Int_WithDefault
    {
    }
#endif

    public class Optional_Property_String_WithDefault : DependencyBaseType
    {
        [OptionalDependency] public string Property { get; set; } = Pattern.DefaultString;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V4 || BEHAVIOR_V5  // Unity v4 and v5 did not support default values for properties
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultString;
#endif
        public override Type ImportType => typeof(string);
    }

    public class Optional_DerivedFromString_WithDefault : Optional_Property_String_WithDefault
    {
    }
}


namespace Dependency.Optional.Properties.WithDefaultAttribute
{

#if !BEHAVIOR_V4 // v4 did not support optional value types
    public class Optional_Int_WithDefaultAttribute : DependencyBaseType
    {
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueInt)] public int Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_WithDefaultAttribute_Int : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueInt)] [OptionalDependency] public int Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_Derived_WithDefaultAttribute
        : Optional_Int_WithDefaultAttribute
    {
    }

#endif

    public class Optional_String_WithDefaultAttribute : DependencyBaseType
    {
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueString)] public string Property { get; set; }
        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
#if BEHAVIOR_V4 || BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }

    public class Optional_WithDefaultAttribute_String : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueString)] [OptionalDependency] public string Property { get; set; }

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }
#if BEHAVIOR_V4 || BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }

}


namespace Dependency.Optional.Properties.WithDefaultAndAttribute
{

#if !BEHAVIOR_V4 // v4 did not support optional value types

    public class Optional_Int_WithDefaultAndAttribute : DependencyBaseType
    {
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueInt)] public int Property { get; set; } = Pattern.DefaultInt;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V5 // Unity v5 did not support default values for properties
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_WithDefaultAndAttribute_Int : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueInt)] [OptionalDependency] public int Property { get; set; } = Pattern.DefaultInt;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V5 // Unity v5 did not support default values for properties
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_Derived_WithDefaultAndAttribute : Optional_Int_WithDefaultAndAttribute
    {
    }

#endif

    public class Optional_String_WithDefaultAndAttribute : DependencyBaseType
    {
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueString)] public string Property { get; set; } = Pattern.DefaultString;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V4 ||  BEHAVIOR_V5 // Unity v4 and v5 did not support default values for properties
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }

    public class Optional_WithDefaultAndAttribute_String : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueString)] [OptionalDependency] public string Property { get; set; } = Pattern.DefaultString;

        public override object Value { get => Property; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V4 ||  BEHAVIOR_V5 // Unity v4 and v5 did not support default values for properties
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }

}
