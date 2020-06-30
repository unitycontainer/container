using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that creates a new instance of 
    /// the registered <see cref="Type"/> once per each thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per thread lifetime means a new instance of the registered <see cref="Type"/>
    /// will be created once per each thread. In other words, if a Resolve{T}() method 
    /// is called on a thread the first time, it will return a new object. Each
    /// subsequent call to Resolve{T}(), or when the dependency mechanism injects 
    /// instances of the type into other classes on the same thread, the container 
    /// will return the same object.
    /// </para>
    /// <para>
    /// This LifetimeManager does not dispose the instances it holds.
    /// </para>
    /// </remarks>
    public class PerThreadLifetimeManager : LifetimeManager,
                                            IFactoryLifetimeManager,
                                            ITypeLifetimeManager
    {
        #region Fields

        private ThreadLocal<object?> _value = new ThreadLocal<object?>(() => NoValue);

        #endregion


        #region Constructors

        public PerThreadLifetimeManager(params InjectionMember[] members)
            : base(members)
        {

        }
        
        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? GetValue(ICollection<IDisposable> lefetime)
        {
            return _value.Value;
        }

        /// <inheritdoc/>
        public override void SetValue(object? newValue, ICollection<IDisposable> lefetime)
        {
            _value.Value = newValue;
        }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerThreadLifetimeManager();
        }



        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerThread";

        #endregion
    }
}
