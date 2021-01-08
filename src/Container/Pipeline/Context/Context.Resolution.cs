using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        #region Resolution

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Resolve()
        {
            return Container.Resolve(ref this);
        }


        public object? Resolve(Type type, string? name)
        {
            var contract = new Contract(type, name);
            var context = new BuilderContext(ref contract, ref this);

            return Container.Resolve(ref context);
        }


        public object? Resolve(ref Contract contract)
        {
            var context = new BuilderContext(ref contract, ref this);

            return Container.Resolve(ref context);
        }


        public object? Resolve(ref Contract contract, ref ErrorInfo errorInfo)
        {
            var context = new BuilderContext(ref contract, ref errorInfo, ref this);
            
            return Container.Resolve(ref context);
        }

        #endregion


        #region Mapping

        public object? MapTo(ref Contract contract)
        {
            var context = new BuilderContext(ref contract, ref this, Registration is Lifetime.PerResolveLifetimeManager);

            return Container.Resolve(ref context);
        }

        #endregion

    }
}
