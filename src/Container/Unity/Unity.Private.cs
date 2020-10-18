using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Lifetime;

namespace Unity
{
    public partial class UnityContainer
    {
        private void DisposeManager(RegistrationManager? manager)
        { 
        
        }

        internal Type ArrayTargetType(Type argType)
        {
            Type? next;
            Type? type = argType;

            do
            {
                if (type.IsGenericType)
                {
                    if (_scope.Contains(type)) return type!;

                    var definition = type.GetGenericTypeDefinition();
                    if (_scope.Contains(definition)) return definition;

                    next = type.GenericTypeArguments[0]!;
                    if (_scope.Contains(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType()!;
                    if (_scope.Contains(next)) return next;
                }
                else
                {
                    return type!;
                }
            }
            while (null != (type = next));

            return argType;
        }


        #region Default Lifetime Manager

        private ITypeLifetimeManager DefaultTypeLifetimeManager(Type type)
        {
            PartCreationPolicyAttribute? attribute;

            if (null == (attribute = (PartCreationPolicyAttribute?)type.GetCustomAttribute(typeof(PartCreationPolicyAttribute))))
                return new TransientLifetimeManager();

            return CreationPolicy.NonShared == attribute.CreationPolicy
                ? (ITypeLifetimeManager)new TransientLifetimeManager()
                : new ContainerControlledLifetimeManager();
        }

        private IInstanceLifetimeManager DefaultInstanceLifetimeManager(Type type)
        {
            PartCreationPolicyAttribute? attribute;

            if (null == (attribute = (PartCreationPolicyAttribute?)type.GetCustomAttribute(typeof(PartCreationPolicyAttribute))))
                return new ContainerControlledLifetimeManager();

            return CreationPolicy.NonShared == attribute.CreationPolicy
                ? (IInstanceLifetimeManager)new TransientLifetimeManager()
                : new ContainerControlledLifetimeManager();
        }

        private IFactoryLifetimeManager DefaultFactoryLifetimeManager(Type type)
        {
            PartCreationPolicyAttribute? attribute;

            if (null == (attribute = (PartCreationPolicyAttribute?)type.GetCustomAttribute(typeof(PartCreationPolicyAttribute))))
                return new TransientLifetimeManager();

            return CreationPolicy.NonShared == attribute.CreationPolicy
                ? (IFactoryLifetimeManager)new TransientLifetimeManager()
                : new ContainerControlledLifetimeManager();
        }

        #endregion
    }
}
