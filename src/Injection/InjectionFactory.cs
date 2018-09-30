using System;
using Unity.Builder;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity.Injection
{
    /// <summary>
    /// A class that lets you specify a factory method the container
    /// will use to create the object.
    /// </summary>
    /// <remarks>This factory allow using predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types.</remarks>
    public class InjectionFactory : InjectionMember, IInjectionFactory, IBuildPlanPolicy
    {
        #region Fields

        private readonly Func<IUnityContainer, Type, string, object> _factoryFunc;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="InjectionFactory"/> with
        /// the given factory function.
        /// </summary>
        /// <param name="factoryFunc">Factory function.</param>
        public InjectionFactory(Func<IUnityContainer, object> factoryFunc)
            : this((c, t, s) => factoryFunc(c))
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionFactory"/> with
        /// the given factory function.
        /// </summary>
        /// <param name="factoryFunc">Factory function.</param>
        public InjectionFactory(Func<IUnityContainer, Type, string, object> factoryFunc)
        {
            _factoryFunc = factoryFunc ?? throw new ArgumentNullException(nameof(factoryFunc));
        }

        #endregion


        #region IInjectionFactory

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Type of interface being registered. If no interface,
        /// this will be null.</param>
        /// <param name="implementationType">Type of concrete type being registered.
        /// This parameter is ignored in this implementation.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            policies.Set(serviceType, name, typeof(IBuildPlanPolicy), this);
        }

        #endregion


        #region IBuildPlanPolicy

        public void BuildUp<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            if (context.Existing == null)
            {
                context.Existing = _factoryFunc(context.Container, context.BuildKey.Type, context.BuildKey.Name);
                context.SetPerBuildSingleton();
            }
        }

        #endregion
    }
}
