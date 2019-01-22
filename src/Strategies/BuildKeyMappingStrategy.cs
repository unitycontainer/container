using System;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Registration;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy
    {
        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, Type type, InternalRegistration registration, params InjectionMember[] injectionMembers)
        {
            if (!(registration is ContainerRegistration containerRegistration)) return null != registration.Map;

            // Validate input  
            if (null == containerRegistration.Type || type == containerRegistration.Type) return false;

            // Set mapping policy
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (type.GetTypeInfo().IsGenericTypeDefinition && 
                containerRegistration.Type.GetTypeInfo().IsGenericTypeDefinition && 
                null == containerRegistration.Map)
#else
            if (type.IsGenericTypeDefinition && containerRegistration.Type.IsGenericTypeDefinition && null == containerRegistration.Map)
#endif
            {
                containerRegistration.Map = (Type t) =>
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
                    var targetTypeInfo = t.GetTypeInfo();
#else
                    var targetTypeInfo = t;
#endif
                    if (targetTypeInfo.IsGenericTypeDefinition)
                    {
                        // No need to perform a mapping - the source type is an open generic
                        return containerRegistration.Type;
                    }

                    if (targetTypeInfo.GenericTypeArguments.Length != containerRegistration.Type.GetTypeInfo().GenericTypeParameters.Length)
                        throw new ArgumentException("Invalid number of generic arguments in types: {registration.MappedToType} and {t}");

                    try
                    {
                        return containerRegistration.Type.MakeGenericType(targetTypeInfo.GenericTypeArguments);
                    }
                    catch (ArgumentException ae)
                    {
                        throw new MakeGenericTypeFailedException(ae);
                    }
                };
            }

            return true;
        }

        #endregion


        #region Build

        /// <summary>
        /// Called during the chain of responsibility for a build operation.  Looks for the <see cref="IBuildKeyMappingPolicy"/>
        /// and if found maps the build key for the current operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            var map = ((InternalRegistration)context.Registration).Map;
            if (null != map) context.Type = map(context.Type);

            if (!((InternalRegistration)context.Registration).BuildRequired &&
                ((UnityContainer)context.Container).RegistrationExists(context.Type, context.Name))
            {
                context.Existing = context.Resolve(context.Type, context.Name);
                context.BuildComplete = true;
            }
        }

        #endregion
    }
}
