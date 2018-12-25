using System;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Registration;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown.
    /// </summary>
    public abstract class BuilderStrategy
    {

        #region Build

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        public virtual void PreBuildUp(ref BuilderContext context)
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public virtual void PostBuildUp(ref BuilderContext context)
        {
        }

        #endregion


        #region Registration and Analysis

        /// <summary>
        /// Analyze registered type
        /// </summary>
        /// <param name="container">Reference to hosting container</param>
        /// <param name="type"></param>
        /// <param name="registration">Reference to registration</param>
        /// <param name="injectionMembers"></param>
        /// <returns>Returns true if this strategy will participate in building of registered type</returns>
        public virtual bool RequiredToBuildType(IUnityContainer container, Type type, InternalRegistration registration, params InjectionMember[] injectionMembers)
        {
            return true;
        }

        /// <summary>
        /// Analyzes registered type
        /// </summary>
        /// <param name="container">Reference to hosting container</param>
        /// <param name="registration">Reference to registration</param>
        /// <returns>Returns true if this strategy will participate in building of registered type</returns>
        public virtual bool RequiredToResolveInstance(IUnityContainer container, InternalRegistration registration)
        {
            return false;
        }

        #endregion


        #region Implementation

        public static TPolicyInterface GetPolicyOrDefault<TPolicyInterface>(ref BuilderContext context, Type type, string name)
        {
            return (TPolicyInterface)(GetNamedPolicy(ref context, type, name) ??
                                      GetNamedPolicy(ref context, type, string.Empty));

            object GetNamedPolicy(ref BuilderContext c, Type t, string n)
            {
                return (c.Get(t, n, typeof(TPolicyInterface)) ?? (
#if NETCOREAPP1_0 || NETSTANDARD1_0
                    t.GetTypeInfo().IsGenericType
#else
                    t.IsGenericType
#endif
                    ? c.Get(t.GetGenericTypeDefinition(), n, typeof(TPolicyInterface)) ?? c.Get(null, null, typeof(TPolicyInterface))
                    : c.Get(null, null, typeof(TPolicyInterface))));
            }
        }


        public static TPolicyInterface GetPolicy<TPolicyInterface>(ref BuilderContext context, Type type, string name)
        {
            return (TPolicyInterface)
            (context.Get(type, name, typeof(TPolicyInterface)) ?? (
#if NETCOREAPP1_0 || NETSTANDARD1_0
                type.GetTypeInfo().IsGenericType
#else
                type.IsGenericType
#endif
                ? context.Get(type.GetGenericTypeDefinition(), name, typeof(TPolicyInterface)) ?? context.Get(null, null, typeof(TPolicyInterface))
                : context.Get(null, null, typeof(TPolicyInterface))));
        }


        #endregion
    }
}
