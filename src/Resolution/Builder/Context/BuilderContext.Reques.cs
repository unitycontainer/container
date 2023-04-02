using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        public struct RequestInfo
        {
            #region Fields

            public ErrorDescriptor ErrorInfo;
            public ResolverOverride[] Overrides;

            #endregion


            #region Constructors

            internal RequestInfo(ResolverOverride[]? overrides)
            {
                ErrorInfo = default;
                Overrides = overrides ?? Array.Empty<ResolverOverride>();
            }

            #endregion


            #region Public

            public bool IsFaulted => ErrorInfo.IsFaulted;

            internal PerResolveOverride PerResolve
            {
                set
                {
                    var current = Overrides;

                    Overrides = new ResolverOverride[Overrides.Length + 1];
                    Overrides[current.Length] = value;

                    if (0 < current.Length) current.CopyTo(Overrides, 0);
                }
            }

            #endregion


            #region Context Factories

            /// <summary>
            /// Create <see cref="BuilderContext"/> 
            /// </summary>
            /// <param name="container"><see cref="UnityContainer"/> the contexts is scoped to</param>
            /// <param name="contract"><see cref="Contract"/> of the resolved instance</param>
            /// <returns><see cref="BuilderContext"/> initialized with request's data</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BuilderContext Context(UnityContainer container, ref Contract contract)
                => new BuilderContext(container, ref contract, ref this);

            /// <summary>
            /// Create <see cref="BuilderContext"/> 
            /// </summary>
            /// <param name="container"><see cref="UnityContainer"/> the contexts is scoped to</param>
            /// <param name="contract"><see cref="Contract"/> of the resolved instance</param>
            /// <param name="manager"><see cref="RegistrationManager"/> holding the registration</param>
            /// <returns><see cref="BuilderContext"/> initialized with request's data</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BuilderContext Context(UnityContainer container, ref Contract contract, RegistrationManager manager)
                => new BuilderContext(container, ref contract, manager, ref this);

            #endregion
        }
    }
}
