using Regression;
using System;
using System.Reflection;

namespace Selection
{
    public abstract partial class Pattern : Regression.PatternBase
    {
        #region Fields

        public static readonly BindingFlags PatternDefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        protected static Type BaselineTestType;
        protected static Type BaselineTestType_IV;

        #endregion


        #region Scaffolding

        public static void Pattern_Initialize(string name, Assembly assembly = null)
        {
            PatternBaseInitialize(name, assembly);

            BaselineTestType = GetTestType("BaselineTestType`3");
            BaselineTestType_IV = GetTestType("BaselineTestType`4");
        }

        #endregion


        #region Base Types

        public abstract class SelectionBaseType : PatternBaseType
        {
            protected SelectionBaseType() => Value = new object[0];

            protected SelectionBaseType(Func<Type, MemberInfo[]> func)
            {
                var members = func(GetType());

                Default = members;
                Value = new object[members.Length];
            }

            public object[] Data => (object[])Value;

            public virtual object this[int index] => ((object[])Value)[index];

            public virtual bool IsSuccessful => true;
        }

        public abstract class SelectionBaseType<TMemberInfo> : SelectionBaseType
            where TMemberInfo : MemberInfo
        {
            protected SelectionBaseType(Func<Type, MemberInfo[]> func)
                : base(func) { }

            public MemberInfo[] Members => (MemberInfo[])Default;
        }

        public class ConstructorSelectionBase : SelectionBaseType<ConstructorInfo>
        {
            public ConstructorSelectionBase()
                : base(t => t.GetConstructors(PatternDefaultFlags))
            { }
        }

        public class MethodSelectionBase : SelectionBaseType<MethodInfo>
        {
            public MethodSelectionBase()
                : base(t => t.GetMethods(PatternDefaultFlags))
            { }
        }

        public class FieldSelectionBase : SelectionBaseType<FieldInfo>
        {
            public FieldSelectionBase()
                : base(t => t.GetFields(PatternDefaultFlags))
            { }

            public override object this[int index] => ((FieldInfo[])Default)[index].GetValue(this);
        }

        public class PropertySelectionBase : SelectionBaseType<PropertyInfo>
        {
            public PropertySelectionBase()
                : base(t => t.GetProperties(PatternDefaultFlags))
            { }
        }

        #endregion
    }
}
