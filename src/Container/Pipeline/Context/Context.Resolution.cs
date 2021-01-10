using System;
using Unity.Extension;

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


        public object? FromContract(in Contract contract)
        {
            var stacked = contract;
            var context = new BuilderContext(ref stacked, ref this);

            return Container.Resolve(ref context);
        }


        public object? FromContract(ref Contract contract, ref ErrorInfo errorInfo)
        {
            var context = new BuilderContext(ref contract, ref errorInfo, ref this);
            
            return Container.Resolve(ref context);
        }

        public object? FromPipeline(ref Contract contract, Delegate pipeline)
        {
            var context = new BuilderContext(ref contract, ref this);

            return ((ResolveDelegate<BuilderContext>)pipeline)(ref context);
        }

        #endregion


        #region Mapping

        public object? FromMapTo(in Contract contract)
        {
            var stacked = contract;
            var context = new BuilderContext(ref stacked, ref this, Registration is Lifetime.PerResolveLifetimeManager);

            Existing = Container.Resolve(ref context);

            return Existing;
        }

        #endregion

    }
}
