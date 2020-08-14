using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    public class PerResolveLifetimeManager : LifetimeManager, 
                                             IInstanceLifetimeManager, 
                                             IFactoryLifetimeManager,
                                             ITypeLifetimeManager
    {
        #region Constructors

        public PerResolveLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? GetValue(ICollection<IDisposable> lefetime) 
            => NoValue;

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.EveryTime;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new PerResolveLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerResolve";

        #endregion
    }
}
