using System;
using System.Reflection;
using Unity.Composition;
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

        public CompositionDelegate ComposeMethod { get; set; }

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
            return policyInterface switch
            {
                Type type when typeof(ISelect<ConstructorInfo>) == type => CtorSelector,
                Type type when typeof(ISelect<PropertyInfo>) == type => PropertiesSelector,
                Type type when typeof(ISelect<MethodInfo>) == type => MethodsSelector,
                Type type when typeof(ISelect<FieldInfo>) == type => FieldsSelector,
                Type type when typeof(ResolveDelegateFactory) == type => ResolveDelegateFactory,
                Type type when typeof(CompositionDelegate) == type => ComposeMethod,

                _ => base.Get(policyInterface)
            };
        }

        public override void Set(Type policyInterface, object policy)
        {
            switch (policyInterface)
            {
                case Type type when typeof(ISelect<ConstructorInfo>) == type:
                    CtorSelector = (ISelect<ConstructorInfo>)policy;
                    break;

                case Type type when typeof(ISelect<PropertyInfo>) == type:
                    PropertiesSelector = (ISelect<PropertyInfo>)policy;
                    break;

                case Type type when typeof(ISelect<MethodInfo>) == type:
                    MethodsSelector = (ISelect<MethodInfo>)policy;
                    break;

                case Type type when typeof(ISelect<FieldInfo>) == type:
                    FieldsSelector = (ISelect<FieldInfo>)policy;
                    break;

                case Type type when typeof(ResolveDelegateFactory) == type:
                    ResolveDelegateFactory = (ResolveDelegateFactory)policy;
                    break;

                case Type type when typeof(CompositionDelegate) == type:
                    ComposeMethod = (CompositionDelegate)policy;
                    break;

                default:
                    base.Set(policyInterface, policy);
                    break;
            }
        }

        #endregion
    }
}
