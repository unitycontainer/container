using Unity.Injection;
using Unity.Storage;

namespace Unity.Builder
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


        public object? Resolve(Contract contract)
        {
            var context = new BuilderContext(ref contract, ref this);

            return Container.Resolve(ref context);
        }


        public object? Resolve(Contract contract, ref ErrorDescriptor errorInfo)
        {
            var context = new BuilderContext(ref contract, ref errorInfo, ref this);
            
            return Container.Resolve(ref context);
        }


        public object? FromPipeline(Contract contract, Delegate pipeline)
        {
            var context = new BuilderContext(ref contract, ref this);

            return ((ResolverPipeline)pipeline)(ref context);
        }

        public void Resolve<TMemberInfo>(ref InjectionInfoStruct<TMemberInfo> info)
        {
            ErrorDescriptor errorInfo = default;
            
            BuilderContext context = info.AllowDefault
                ? new BuilderContext(ref info.Contract, ref errorInfo, ref this)
                : new BuilderContext(ref info.Contract, ref this);

            info.InjectedValue[DataType.Value] = Container.Resolve(ref context);

            if (errorInfo.IsFaulted)
            {
                if (info.DefaultValue.IsValue)
                    info.InjectedValue[DataType.Value] = info.DefaultValue.Value;
                else
                    info.InjectedValue = default;
            }
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
