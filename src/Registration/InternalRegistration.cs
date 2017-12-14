using System;
using Unity.Container.Storage;
using Unity.Policy;

namespace Unity.Registration
{
    public class InternalRegistration : LinkedMap<Type, IBuilderPolicy>, 
                                        IRegistration
    {
        #region Constructors

        public InternalRegistration(Type type, string name, LinkedNode<Type, IBuilderPolicy> next = null)
            : base(typeof(IResolverPolicy), null, next)
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
                if (key == typeof(IResolverPolicy))
                    return Value ?? GetResolvePolicy();

                return base[key];
            }
            set => base[key] = value;
        }

        #endregion



        #region Implementation

        protected virtual IBuilderPolicy GetResolvePolicy()
        {
            return Value;
        }

        #endregion
    }
}
