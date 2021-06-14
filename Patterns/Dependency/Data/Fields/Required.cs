using System;
using Regression;
using System.ComponentModel;
using static Dependency.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Dependency.Required.Fields
{
    #region Validation

    public class PrivateTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] private TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
        protected TDependency Dummy() => (Field = default);
    }

    public class ProtectedTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] protected TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
    }

    public class InternalTestType<TDependency>
        : PatternBaseType
    {
        [Dependency] internal TDependency Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => default(TDependency);
        protected TDependency Dummy() => (Field = default);
    }

    #endregion
}


namespace Dependency.Required.Fields.WithDefault
{
    // Unity does not support implicit default values on properties
    // When resolved it will throw if not registered
    //
    //public class Required_WithDefault : PatternBaseType
    //{
    //    [Dependency] public TDependency Field;
    //}
}


namespace Dependency.Required.Fields.WithDefaultAttribute
{
#if !BEHAVIOR_V5 // Unity v5 did not support DefaultValueAttribute on fields

    public class Required_Field_Int_WithDefaultAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueInt)] public int Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Field_WithDefaultAttribute_Int : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueInt)] [Dependency] public int Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Field_String_WithDefaultAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueString)] public string Field;
        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Field_WithDefaultAttribute_String : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueString)] [Dependency] public string Field;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Derived_WithDefaultAttribute : Required_Field_Int_WithDefaultAttribute
    {
    }

#endif
}


namespace Dependency.Required.Fields.WithDefaultAndAttribute
{
#if !BEHAVIOR_V5 // Unity v5 did not support DefaultValueAttribute on fields

    public class Required_Field_Int_WithDefaultAndAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueInt)] public int Field = Pattern.DefaultInt;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Field_WithDefaultAndAttribute_Int : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueInt)] [Dependency] public int Field = Pattern.DefaultInt;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueInt;
        public override Type ImportType => typeof(int);
    }

    public class Required_Field_String_WithDefaultAndAttribute : DependencyBaseType
    {
        [Dependency] [DefaultValue(Pattern.DefaultValueString)] public string Field = Pattern.DefaultString;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Field_WithDefaultAndAttribute_String : DependencyBaseType
    {
        [DefaultValue(Pattern.DefaultValueString)] [Dependency] public string Field = Pattern.DefaultString;

        public override object Value { get => Field; protected set => throw new NotSupportedException(); }
        public override object Default => Pattern.DefaultValueString;
        public override Type ImportType => typeof(string);
    }

    public class Required_Derived_WithDefaultAndAttribute : Required_Field_Int_WithDefaultAndAttribute
    {
    }
#endif
}
