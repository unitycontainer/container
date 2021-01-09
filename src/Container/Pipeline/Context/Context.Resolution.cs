using System;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        #region Resolution


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

            Existing = Container.Resolve(ref context);

            return Existing;
        }

        #endregion

    }
}
