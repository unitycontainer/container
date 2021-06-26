using System;
using Regression;
using System.ComponentModel;
using static Dependency.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Dependency.Implicit.Methods
{
    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        [InjectionMethod]
        private void Method(TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class ProtectedTestType<TDependency>
        : PatternBaseType
    {
        [InjectionMethod]
        protected void Method(TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        [InjectionMethod]
        internal void Method(TDependency value) => Value = value;
        public override object Default => default(TDependency);
    }

    public class BaselineTestType_Ref<TDependency>
        : PatternBaseType where TDependency : class
    {
        [InjectionMethod]
        public virtual void Method(ref TDependency value)
            => throw new InvalidOperationException("should never execute");
    }

    public class BaselineTestType_Out<TDependency>
        : PatternBaseType where TDependency : class
    {
        [InjectionMethod]
        public virtual void Method(out TDependency value)
            => throw new InvalidOperationException("should never execute");
    }

    #endregion
}


namespace Dependency.Implicit.Methods.WithDefault
{
    public class Implicit_Parameter_Int_WithDefault : DependencyBaseType
    {
        [InjectionMethod]
        public virtual void Method(int value = Pattern.DefaultInt) => Value = value;

        public override object Default => Pattern.DefaultInt;
        public override object Injected => Pattern.InjectedInt;
        public override object Registered => Pattern.RegisteredInt;
        public override object Override => Pattern.OverriddenInt;
        public override Type ImportType => typeof(int);
    }

    public class Implicit_Parameter_String_WithDefault : DependencyBaseType
    {
        [InjectionMethod]
        public virtual void Method(string value = Pattern.DefaultString) => Value = value;

        public override object Default => Pattern.DefaultString;
        public override object Injected => Pattern.InjectedString;
        public override object Registered => Pattern.RegisteredString;
        public override object Override => Pattern.OverriddenString;

        public override Type ImportType => typeof(string);
    }

    public class Implicit_Derived_WithDefault : Implicit_Parameter_Int_WithDefault
    {
        private const int _default = 1111;

        [InjectionMethod]
        public override void Method(int value = _default) => base.Method(value);
#if BEHAVIOR_V5
        // BUG: See https://github.com/unitycontainer/container/issues/291
        public override object Default => Pattern.DefaultInt;
        public override object Injected => Pattern.DefaultInt;
#else
        public override object Default => _default;
#endif
    }
}


namespace Dependency.Implicit.Methods.WithDefaultAttribute
{
    public class Implicit_Int_WithDefaultAttribute : DependencyBaseType
    {
        [InjectionMethod]
        public virtual void Method([DefaultValue(Pattern.DefaultValueInt)] int value) => Value = value;

        public override object Default => Pattern.DefaultValueInt;
        public override object Injected => Pattern.InjectedInt;
        public override object Registered => Pattern.RegisteredInt;
        public override object Override => Pattern.OverriddenInt;
        public override Type ImportType => typeof(int);
    }

    public class Implicit_String_WithDefaultAttribute : DependencyBaseType
    {
        [InjectionMethod]
        public virtual void Method([DefaultValue(Pattern.DefaultValueString)] string value) => Value = value;

        public override object Default => Pattern.DefaultValueString;
        public override object Injected => Pattern.InjectedString;
        public override object Registered => Pattern.RegisteredString;
        public override object Override => Pattern.OverriddenString;
        public override Type ImportType => typeof(string);
    }

#if !BEHAVIOR_V5 // BUG: See https://github.com/unitycontainer/container/issues/291
    public class Implicit_Derived_WithDefaultAttribute : Implicit_Int_WithDefaultAttribute
    {
        private const int _default = 1111;

        [InjectionMethod]
        public override void Method([DefaultValue(_default)] int value = Pattern.DefaultValueInt) => base.Method(value);

        public override object Default => _default;
        public override Type ImportType => typeof(int);
    }
#endif
}


namespace Dependency.Implicit.Methods.WithDefaultAndAttribute
{
    public class Implicit_Int_WithDefaultAndAttribute : DependencyBaseType
    {
        [InjectionMethod]
        public virtual void Method([DefaultValue(Pattern.DefaultValueInt)] int value = Pattern.DefaultInt) => Value = value;

        public override object Injected => Pattern.InjectedInt;
        public override object Registered => Pattern.RegisteredInt;
        public override object Override => Pattern.OverriddenInt;
#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => Pattern.DefaultInt;
#else
        public override object Default => Pattern.DefaultValueInt;
#endif
        public override Type ImportType => typeof(int);
    }

    public class Implicit_String_WithDefaultAndAttribute : DependencyBaseType
    {
        [InjectionMethod]
        public virtual void Method([DefaultValue(Pattern.DefaultValueString)] string value = Pattern.DefaultString) => Value = value;

        public override object Injected => Pattern.InjectedString;
        public override object Registered => Pattern.RegisteredString;
        public override object Override => Pattern.OverriddenString;
#if BEHAVIOR_V5
        // Prior to v6 Unity did not support DefaultValueAttribute
        public override object Default => Pattern.DefaultString;
#else
        public override object Default => Pattern.DefaultValueString;
#endif
        public override Type ImportType => typeof(string);
    }

    public class Implicit_Derived_WithDefaultAndAttribute : Implicit_Int_WithDefaultAndAttribute
    {
        private const int _default = 1111;

        [InjectionMethod]
        public override void Method([DefaultValue(_default)] int value = Pattern.DefaultValueInt) => base.Method(value);
#if BEHAVIOR_V5
        // BUG: See https://github.com/unitycontainer/container/issues/291
        public override object Default => Pattern.DefaultInt;
        public override object Injected => Pattern.DefaultInt;
#else
        public override object Default => _default;
#endif
    }
}
