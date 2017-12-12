using System;
using Unity.Container.Storage;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container.Registration
{
    public class InternalRegistration : LinkedMap<Type, IBuilderPolicy>, 
                                        IRegistration
    {
        #region Constructors

        public InternalRegistration(Type type, string name, LinkedNode<Type, IBuilderPolicy> next = null)
            : base(typeof(IResolvePolicy), null, next)
        {
            Name = string.IsNullOrEmpty(name) ? null : name;
            RegisteredType = type;
            Name = name;
        }

        #endregion


        #region  IRegistration

        public Type RegisteredType { get; }

        public string Name { get; }

        #endregion



        #region LinkedMap

        public override IBuilderPolicy this[Type key]
        {
            get
            {
                if (key == typeof(IResolvePolicy))
                    return GetResolvePolicy();

                return base[key];
            }
            set => base[key] = value;
        }

        #endregion



        #region Implementation

        protected virtual IBuilderPolicy GetResolvePolicy()
        {
            if (null != Value) return Value;

            return null;
        }

        #endregion
    }
}
