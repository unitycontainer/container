using System.Diagnostics.Contracts;
using Unity.Container;
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

            return ((ResolverPipeline)pipeline)(ref context);
        }

        public void Resolve<TMemberInfo>(ref InjectionInfoStruct<TMemberInfo> info)
        {
            ErrorDescriptor errorInfo = default;
            Contract contract = new(info.ContractType, info.ContractName);
            
            BuilderContext context = info.AllowDefault
                ? new BuilderContext(ref contract, ref errorInfo, ref this)
                : new BuilderContext(ref contract, ref this);

            info.DataValue[DataType.Value] = Container.Resolve(ref context);

            if (errorInfo.IsFaulted)
            {
                if (info.DefaultValue.IsValue)
                    info.DataValue[DataType.Value] = info.DefaultValue.Value;
                else
                    info.DataValue = default;
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
