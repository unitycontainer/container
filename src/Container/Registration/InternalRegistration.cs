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

        public InternalRegistration(Type type, string name)
        {
            RegisteredType = type;
            Name = name;
        }

        #endregion


        #region  IRegistration

        public Type RegisteredType { get; }

        public string Name { get; }

        #endregion
    }
}
