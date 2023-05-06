using System.Diagnostics;
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

        public object? Resolve<TMemberInfo>(TMemberInfo member, ref Contract contract)
        {
            var context = new BuilderContext(ref contract, ref this);

            var @override = context.GetOverride(member, ref contract);
            if (@override.IsValue) return @override.Value;

            return Container.Resolve(ref context);
        }

        public object? Resolve<TMemberInfo>(TMemberInfo member, ref Contract contract, ref ErrorDescriptor errorInfo)
        {
            BuilderContext context = new BuilderContext(ref contract, ref errorInfo, ref this);

            var @override = context.GetOverride(member, ref contract);
            if (@override.IsValue) return @override.Value;

            return Container.Resolve(ref context);
        }

        public object? ResolveOrDefault<TMemberInfo>(TMemberInfo member, ref Contract contract, object? @default)
        {
            ErrorDescriptor errorInfo = default;
            BuilderContext context = new BuilderContext(ref contract, ref errorInfo, ref this);

            var @override = context.GetOverride(member, ref contract);
            if (@override.IsValue) return @override.Value;
            
            var instance = Container.Resolve(ref context);

            if (errorInfo.IsFaulted) 
                return @default is ResolverPipeline pipeline
                    ? pipeline(ref context)
                    : @default;

            return instance;
        }

        #endregion


        #region Injection

        public object? OverridePipeline<TMember>(TMember member, ref Contract contract, ResolverPipeline? pipeline)
        {
            Debug.Assert(pipeline is not null);

            ErrorDescriptor errorInfo = default;
            BuilderContext context = new BuilderContext(ref contract, ref errorInfo, ref this);

            var @override = context.GetOverride(member, ref contract);
            if (@override.IsValue) return @override.Value;
                
            return pipeline!(ref context);
        }

        public object? InjectValue<TMember>(TMember member, ref Contract contract, object? value)
        {
            var context = new BuilderContext(ref contract, ref this);
            var @override = context.GetOverride(member, ref contract);
            if (@override.IsValue) return @override.Value;

            return value;
        }

        #endregion


        #region Implementation

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