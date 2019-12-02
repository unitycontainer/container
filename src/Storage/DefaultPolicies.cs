using System;
using System.Diagnostics;
using System.Reflection;

namespace Unity.Storage
{
    public class DefaultPolicies : PolicySet
    {
        #region Fields

        private MemberSelector<ConstructorInfo>? _selectConstructor;
        private MemberSelector<PropertyInfo>? _selectProperty;
        private MemberSelector<MethodInfo>? _selectMethod;
        private MemberSelector<FieldInfo>? _selectField;

        #endregion


        #region Constructors

        public DefaultPolicies(UnityContainer owner)
            : base(owner)
        {
        }
        
        #endregion


        #region PolicySet

        public override object? Get(Type policyInterface)
        {
            return policyInterface switch
            {
                Type type when typeof(MemberSelector<ConstructorInfo>) == type => SelectConstructor,
                Type type when typeof(MemberSelector<PropertyInfo>) == type => SelectProperty,
                Type type when typeof(MemberSelector<MethodInfo>) == type => SelectMethod,
                Type type when typeof(MemberSelector<FieldInfo>) == type => SelectField,

                _ => base.Get(policyInterface)
            };
        }

        public override void Set(Type policyInterface, object policy)
        {
            switch (policyInterface)
            {
                case Type type when typeof(MemberSelector<ConstructorInfo>) == type:
                    SelectConstructor = (MemberSelector<ConstructorInfo>)policy;
                    break;

                case Type type when typeof(MemberSelector<PropertyInfo>) == type:
                    SelectProperty = (MemberSelector<PropertyInfo>)policy;
                    break;

                case Type type when typeof(MemberSelector<MethodInfo>) == type:
                    SelectMethod = (MemberSelector<MethodInfo>)policy;
                    break;

                case Type type when typeof(MemberSelector<FieldInfo>) == type:
                    SelectField = (MemberSelector<FieldInfo>)policy;
                    break;

                default:
                    base.Set(policyInterface, policy);
                    break;
            }
        }

        #endregion


        #region Default Policies

        public MemberSelector<ConstructorInfo> SelectConstructor
        {
            get
            {
                Debug.Assert(null != _selectConstructor);
                return _selectConstructor!;
            }

            private set => _selectConstructor = value ??
               throw new ArgumentNullException("Constructor Selector must be non null");
        }

        public MemberSelector<PropertyInfo> SelectProperty
        {
            get
            {
                Debug.Assert(null != _selectProperty);
                return _selectProperty!;
            }

            private set => _selectProperty = value ??
               throw new ArgumentNullException("Property Selector must be non null");
        }

        public MemberSelector<MethodInfo> SelectMethod
        {
            get
            {
                Debug.Assert(null != _selectMethod);
                return _selectMethod!;
            }

            private set => _selectMethod = value ??
               throw new ArgumentNullException("Method Selector must be non null");
        }

        public MemberSelector<FieldInfo> SelectField
        {
            get
            {
                Debug.Assert(null != _selectField);
                return _selectField!;
            }

            private set => _selectField = value
               ?? throw new ArgumentNullException("Field Selector must be non null");
        }

        #endregion
    }
}
