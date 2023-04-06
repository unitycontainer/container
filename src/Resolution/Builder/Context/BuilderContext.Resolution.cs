using System.Diagnostics;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        #region Resolution

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


        #region Required

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

        public object? Resolve<TMemberInfo>(TMemberInfo member, ref Contract contract)
        {

            var context = new BuilderContext(ref contract, ref this);

            var @override = GetOverride(member, ref contract);
            if (@override is not null) return @override.Resolve(ref context);

            return Container.Resolve(ref context);
        }

        #endregion


        #region Optional

        public object? ResolveOptional<TMember>(TMember member, ref Contract contract, ResolverPipeline? pipeline)
        {
            Debug.Assert(pipeline is not null);

            BuilderContext context;

            var @override = GetOverride(member, ref contract);
            if (@override is not null)
            {
                context = new BuilderContext(ref contract, ref this);
                return @override.Resolve(ref context);
            }

            ErrorDescriptor errorInfo = default;
            context = new BuilderContext(ref contract, ref errorInfo, ref this);
            var instance = Container.Resolve(ref context);

            if (errorInfo.IsFaulted) return pipeline!(ref context);

            return instance;

        }

        public object? ResolveOptional<TMember>(TMember member, ref Contract contract, object? value)
        {
            BuilderContext context;

            var @override = GetOverride(member, ref contract);
            if (@override is not null)
            {
                context = new BuilderContext(ref contract, ref this);
                return @override.Resolve(ref context);
            }
            
            ErrorDescriptor errorInfo = default;
            context = new BuilderContext(ref contract, ref errorInfo, ref this);
            var instance = Container.Resolve(ref context);

            if (errorInfo.IsFaulted) return value;

            return instance;
        }

        #endregion

        public object? Resolve(Contract contract, ref ErrorDescriptor errorInfo)
        {
            var context = new BuilderContext(ref contract, ref errorInfo, ref this);

            return Container.Resolve(ref context);
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
