using System;
using System.Collections.Generic;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Container Context

        public partial class ScopeContext : ContainerContext
        {
            #region Fields

            private UnityContainer _owner;

            #endregion


            #region Constructor

            public ScopeContext(UnityContainer owner) 
                => _owner = owner;

            #endregion


            #region Public Members

            public override string? Name 
            { 
                get => _owner._name; 
                set => _owner._name = value; 
            }

            public override IPolicyList Policies => _owner._scope;

            /// <inheritdoc />
            public override ICollection<IDisposable> Lifetime => _owner._scope.Lifetime;

            #endregion


            #region Default Lifetime Managers

            /// <inheritdoc />
            public override LifetimeManager DefaultTypeLifetime
            {
                get => _owner._typeLifetime;
                set
                {
                    if (!(value is ITypeLifetimeManager))
                        throw new ArgumentException("Default Type LifetimeManager must implement ITypeLifetimeManager");

                    _owner._typeLifetime = value;
                }
            }

            /// <inheritdoc />
            public override LifetimeManager DefaultFactoryLifetime
            {
                get => _owner._factoryLifetime;
                set
                {
                    if (!(value is IFactoryLifetimeManager))
                        throw new ArgumentException("Default Factory LifetimeManager must implement IFactoryLifetimeManager");

                    _owner._factoryLifetime = value;
                }
            }

            /// <inheritdoc />
            public override LifetimeManager DefaultInstanceLifetime
            {
                get => _owner._instanceLifetime;
                set
                {
                    if (!(value is IInstanceLifetimeManager))
                        throw new ArgumentException("Default Type LifetimeManager must implement IInstanceLifetimeManager");

                    _owner._instanceLifetime = value;
                }
            }

            #endregion
        }

        #endregion
    }
}
