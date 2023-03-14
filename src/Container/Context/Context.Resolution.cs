using System;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        #region Resolution


        public object? Resolve(Type type, string? name)
        {
            var stacked = new Contract(type, name);
            var context = new BuilderContext(ref stacked, ref this);

            return Container.Resolve(ref context);
        }


        public object? FromContract(Contract contract)
        {
            var context = new BuilderContext(ref contract, ref this);

            return Container.Resolve(ref context);
        }


        public object? FromContract(Contract contract, ref ErrorDescriptor errorInfo)
        {
            var context = new BuilderContext(ref contract, ref errorInfo, ref this);
            
            return Container.Resolve(ref context);
        }

        public object? FromPipeline(Contract contract, Delegate pipeline)
        {
            var context = new BuilderContext(ref contract, ref this);

            return ((ResolveDelegate<BuilderContext>)pipeline)(ref context);
        }

        #endregion


        #region Mapping

        public object? MapTo(Contract contract)
        {
            var context = new BuilderContext(ref contract, ref this, Registration is Lifetime.PerResolveLifetimeManager);

            Existing = Container.Resolve(ref context);

            return Existing;
        }

        #endregion

    }
}
