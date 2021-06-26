using Regression;
using System;
using System.ComponentModel;
using static Dependency.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Dependency.Optional.Constructors
{
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


namespace Dependency.Optional.Constructors.WithDefault
{

#if !BEHAVIOR_V4 // v4 did not support optional value types

    public class Optional_Parameter_Int_WithDefault : DependencyBaseType
    {
        public Optional_Parameter_Int_WithDefault([OptionalDependency] int value = Pattern.DefaultInt) => Value = value;
        public override object Default => Pattern.DefaultInt;
        public override Type ImportType => typeof(int);
    }


    public class Optional_DerivedFromInt_WithDefault : Optional_Parameter_Int_WithDefault
    {
        private const int _default = 1111;

        public Optional_DerivedFromInt_WithDefault([OptionalDependency] int value = _default) : base(value) { }
        public override object Default => _default;
        public override Type ImportType => typeof(int);
    }

#endif


    public class Optional_Parameter_String_WithDefault : DependencyBaseType
    {
        public Optional_Parameter_String_WithDefault([OptionalDependency] string value = Pattern.DefaultString) => Value = value;

#if  BEHAVIOR_V4 // Unity v4 did not support default values
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultString;
#endif
        public override Type ImportType => typeof(string);
    }
  
}


namespace Dependency.Optional.Constructors.WithDefaultAttribute
{

#if !BEHAVIOR_V4 // v4 did not support optional value types

    public class Optional_Int_WithDefaultAttribute : DependencyBaseType
    {
        public Optional_Int_WithDefaultAttribute([OptionalDependency][DefaultValue(Pattern.DefaultValueInt)] int value) => Value = value;

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
        public Optional_WithDefaultAttribute_Int([DefaultValue(Pattern.DefaultValueInt)][OptionalDependency] int value) => Value = value;
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
        private const int _default = 1111;

        public Optional_Derived_WithDefaultAttribute([OptionalDependency][DefaultValue(_default)] int value) : base(value) { }
#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => 0;
#else
        public override object Default => _default;
#endif
        public override Type ImportType => typeof(int);
    }


#endif


    public class Optional_String_WithDefaultAttribute : DependencyBaseType
    {
        public Optional_String_WithDefaultAttribute([OptionalDependency][DefaultValue(Pattern.DefaultValueString)] string value) => Value = value;
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
        public Optional_WithDefaultAttribute_String([DefaultValue(Pattern.DefaultValueString)][OptionalDependency] string value) => Value = value;
#if BEHAVIOR_V4 || BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => null;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }
}


namespace Dependency.Optional.Constructors.WithDefaultAndAttribute
{
#if !BEHAVIOR_V4 // v4 did not support optional value types

    public class Optional_Int_WithDefaultAndAttribute : DependencyBaseType
    {
        public Optional_Int_WithDefaultAndAttribute([OptionalDependency][DefaultValue(Pattern.DefaultValueInt)] int value = Pattern.DefaultInt) => Value = value;

#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => Pattern.DefaultInt;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }


    public class Optional_WithDefaultAndAttribute_Int : DependencyBaseType
    {
        public Optional_WithDefaultAndAttribute_Int([DefaultValue(Pattern.DefaultValueInt)][OptionalDependency] int value = Pattern.DefaultInt) => Value = value;

#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => Pattern.DefaultInt;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }


    public class Optional_Derived_WithDefaultAndAttribute : Optional_Int_WithDefaultAndAttribute
    {
        private const int _default = 1111;

        public Optional_Derived_WithDefaultAndAttribute([OptionalDependency][DefaultValue(_default)] int value = Pattern.DefaultValueInt)
            : base(value) { }

#if BEHAVIOR_V5
        public override object Default => Pattern.DefaultValueInt;
#else
        public override object Default => _default;
#endif
        public override Type ImportType => typeof(int);
    }


#endif


    public class Optional_String_WithDefaultAndAttribute : DependencyBaseType
    {
        public Optional_String_WithDefaultAndAttribute([OptionalDependency][DefaultValue(Pattern.DefaultValueString)] string value = Pattern.DefaultString) => Value = value;

#if BEHAVIOR_V4     // Unity v4 did not support default values
        public override object Default => null;
#elif BEHAVIOR_V5   // Unity v5 did not support DefaultValueAttribute
        public override object Default => Pattern.DefaultString;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }


    public class Optional_WithDefaultAndAttribute_String : DependencyBaseType
    {
        public Optional_WithDefaultAndAttribute_String([DefaultValue(Pattern.DefaultValueString)][OptionalDependency] string value = Pattern.DefaultString) => Value = value;

#if BEHAVIOR_V4     // Unity v4 did not support default values
        public override object Default => null;
#elif BEHAVIOR_V5   // Unity v5 did not support DefaultValueAttribute
        public override object Default => Pattern.DefaultString;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }
}
