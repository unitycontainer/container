using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Composition;
using Unity.Policy;

namespace Unity.Storage
{
    public class DefaultPolicies : PolicySet, 
                                   ISelect<ConstructorInfo>,
                                   ISelect<PropertyInfo>,
                                   ISelect<MethodInfo>,
                                   ISelect<FieldInfo>
    {
        #region Fields

        private ISelect<ConstructorInfo> _selectConstructor;
        private ISelect<PropertyInfo> _selectProperty;
        private ISelect<MethodInfo> _selectMethod;
        private ISelect<FieldInfo> _selectField;
        
        #endregion


        #region Constructors

        public DefaultPolicies(UnityContainer owner)
            : base(owner)
        {
            _selectConstructor = this;
            _selectProperty = this;
            _selectMethod = this;
            _selectField = this;
        }
        
        #endregion


        #region PolicySet

        public override object? Get(Type policyInterface)
        {
            return policyInterface switch
            {
                Type type when typeof(ISelect<ConstructorInfo>) == type => SelectConstructor,
                Type type when typeof(ISelect<PropertyInfo>) == type => SelectProperty,
                Type type when typeof(ISelect<MethodInfo>) == type => SelectMethod,
                Type type when typeof(ISelect<FieldInfo>) == type => SelectField,

                _ => base.Get(policyInterface)
            };
        }

        public override void Set(Type policyInterface, object policy)
        {
            switch (policyInterface)
            {
                case Type type when typeof(ISelect<ConstructorInfo>) == type:
                    SelectConstructor = (ISelect<ConstructorInfo>)policy;
                    break;

                case Type type when typeof(ISelect<PropertyInfo>) == type:
                    SelectProperty = (ISelect<PropertyInfo>)policy;
                    break;

                case Type type when typeof(ISelect<MethodInfo>) == type:
                    SelectMethod = (ISelect<MethodInfo>)policy;
                    break;

                case Type type when typeof(ISelect<FieldInfo>) == type:
                    SelectField = (ISelect<FieldInfo>)policy;
                    break;

                default:
                    base.Set(policyInterface, policy);
                    break;
            }
        }

        #endregion


        #region Default Policies

        public ISelect<ConstructorInfo> SelectConstructor
        {
            get => _selectConstructor;
            private set => _selectConstructor = value ?? 
                throw new ArgumentNullException("Constructor Selector must not be null");
        }

        public ISelect<PropertyInfo> SelectProperty
        {
            get => _selectProperty;
            private set => _selectProperty = value ?? 
                throw new ArgumentNullException("Property Selector must not be null");
        }

        public ISelect<MethodInfo> SelectMethod
        {
            get => _selectMethod;
            private set => _selectMethod = value ?? 
                throw new ArgumentNullException("Method Selector must not be null");
        }

        public ISelect<FieldInfo> SelectField
        {
            get => _selectField;
            private set => _selectField = value 
                ?? throw new ArgumentNullException("Field Selector must not be null");
        }

        #endregion


        #region Implementation

        public IEnumerable<object> Select(Type type, IPolicySet registration) => throw new NotImplementedException();

        #endregion
    }
}
