using System;
using Regression;
using System.ComponentModel;
using static Dependency.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Dependency.Optional.Fields
{
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


namespace Dependency.Optional.Fields.WithDefault
{

#if !BEHAVIOR_V4 // v4 did not support optional value types
    public class Optional_Field_Int_WithDefault : DependencyBaseType
    {
        [OptionalDependency] public int Field = Pattern.DefaultInt;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }

#if  BEHAVIOR_V5 // Unity v5 did not support default values for fields
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_DerivedFromInt_WithDefault : Optional_Field_Int_WithDefault
    {
    }

#endif


    public class Optional_Field_String_WithDefault : DependencyBaseType
    {
        [OptionalDependency] public string Field = Pattern.DefaultString;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }

#if  BEHAVIOR_V4 || BEHAVIOR_V5 // Unity v4 and v5 did not support default values for fields
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultString;
#endif
        public override Type ImportType => typeof(string);
    }

    public class Optional_DerivedFromString_WithDefault : Optional_Field_String_WithDefault
    {
    }
}


namespace Dependency.Optional.Fields.WithDefaultAttribute
{

#if !BEHAVIOR_V4 // v4 did not support optional value types

    public class Optional_Int_WithDefaultAttribute : DependencyBaseType
    {
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueInt)] public int Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
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
        [DefaultValue(Pattern.DefaultValueInt)] [OptionalDependency] public int Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_Derived_WithDefaultAttribute : Optional_Int_WithDefaultAttribute
    {
    }

#endif

    public class Optional_String_WithDefaultAttribute : DependencyBaseType
    {
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueString)] public string Field;
        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
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
        [DefaultValue(Pattern.DefaultValueString)] [OptionalDependency] public string Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
#if BEHAVIOR_V4 || BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }
}


namespace Dependency.Optional.Fields.WithDefaultAndAttribute
{

#if !BEHAVIOR_V4 // v4 did not support optional value types

    public class Optional_Int_WithDefaultAndAttribute : DependencyBaseType
    {
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueInt)] public int Field = Pattern.DefaultInt;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V5   // Unity v5 did not support default value for fields
        public override object Default => 0;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Optional_WithDefaultAndAttribute_Int : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueInt)] [OptionalDependency] public int Field = Pattern.DefaultInt;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V5   // Unity v5 did not support default value for fields
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
        [OptionalDependency] [DefaultValue(Pattern.DefaultValueString)] public string Field = Pattern.DefaultString;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }

#if BEHAVIOR_V4 ||  BEHAVIOR_V5 // Unity v4 and v5 did not support default values for fields
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }

    public class Optional_WithDefaultAndAttribute_String : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueString)] [OptionalDependency] public string Field = Pattern.DefaultString;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }


#if BEHAVIOR_V4 ||  BEHAVIOR_V5 // Unity v4 and v5 did not support default values for fields
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }

}
