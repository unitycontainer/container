using System;
using System.Reflection;
using Unity.Storage;

namespace Unity.Policy
{
    public class DefaultPolicies : PolicySet
    {
        #region Constructors

        public DefaultPolicies(ResolveDelegateFactory factory)
            : base(typeof(ResolveDelegateFactory), factory)
        {
        }

        #endregion


        #region Public Members

        public ISelect<ConstructorInfo> CtorSelector { get; set; }

        public ISelect<PropertyInfo> PropertiesSelector { get; set; }

        public ISelect<MethodInfo> MethodsSelector { get; set; }

        public ISelect<FieldInfo> FieldsSelector { get; set; }

        public ResolveDelegateFactory ResolveDelegateFactory
        {
            get => (ResolveDelegateFactory)Value;
            set => Value = value;
        }

        #endregion


        #region PolicySet

        public override object Get(Type policyInterface)
        {
            switch (policyInterface)
            {
                case Type type when (typeof(ISelect<ConstructorInfo>) == type):
                    return CtorSelector;

                case Type type when (typeof(ISelect<PropertyInfo>) == type):
                    return PropertiesSelector;

                case Type type when (typeof(ISelect<MethodInfo>) == type):
                    return MethodsSelector;

                case Type type when (typeof(ISelect<FieldInfo>) == type):
                    return FieldsSelector;

                default:
                    return base.Get(policyInterface);
            }
        }

        public override void Set(Type policyInterface, object policy)
        {
            switch (policyInterface)
            {
                case ISelect<ConstructorInfo> constructor:
                    CtorSelector = constructor;
                    break;

                case ISelect<PropertyInfo> property:
                    PropertiesSelector = property;
                    break;

                case ISelect<MethodInfo> method:
                    MethodsSelector = method;
                    break;

                case ISelect<FieldInfo> field:
                    FieldsSelector = field;
                    break;

                default:
                    base.Set(policyInterface, policy);
                    break;
            }
        }

        #endregion
    }
}
