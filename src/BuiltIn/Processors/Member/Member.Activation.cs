using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null != context.Target);

            // Type to build
            Type type = context.Type;
            var members = GetMembers(type);

            ///////////////////////////////////////////////////////////////////
            // No members
            if (0 == members.Length) return;

            Span<bool> set = stackalloc bool[members.Length];
            DependencyInfo<TDependency> dependency = default;

            ///////////////////////////////////////////////////////////////////
            // Initialize injected members
            for (var injected = GetInjected(context.Registration); null != injected; injected = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injected.Next))
            {
                int index;

                using var injection = context.Start(injected);

                if (-1 == (index = injected.SelectFrom(members)))
                {
                    injection.Error($"Injected member '{injected}' doesn't match any {typeof(TDependency).Name} on type {type}");
                    return;
                }

                if (set[index]) continue;
                else set[index] = true;

                dependency.Info   = Unsafe.As<TDependency>(members[index]);
                dependency.Import = GetImportAttribute(Unsafe.As<TMemberInfo>(dependency.Info));

                Activate(ref dependency, injected.Data);
            }

            ///////////////////////////////////////////////////////////////////
            // Initialize annotated members
            for (var index = 0; index < members.Length; index++)
            {
                if (set[index]) continue;

                var info   = Unsafe.As<TDependency>(members[index]);
                var import = GetImportAttribute(Unsafe.As<TMemberInfo>(info));
                
                if (null == import) continue;

                else set[index] = true;

                Activate(ref dependency);
            }
        }

        public virtual object? Activate(ref DependencyInfo<TDependency> dependency, object? data)
        {
            throw new NotImplementedException();
            //PipelineContext local;

            //switch (data)
            //{
            //    case ResolveDelegate<PipelineContext> resolver:
            //        local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
            //        return local.GetValue(dependency.Info, resolver(ref local));

            //    case IResolve iResolve:
            //        local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
            //        return local.GetValue(dependency.Info, iResolve.Resolve(ref local));

            //    case IResolverFactory<TDependency> infoFactory:
            //        local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
            //        return local.GetValue(dependency.Info, infoFactory.GetResolver<PipelineContext>(dependency.Info)
            //                                                          .Invoke(ref local));
            //    case IResolverFactory<Type> typeFactory:
            //        local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
            //        return local.GetValue(dependency.Info, typeFactory.GetResolver<PipelineContext>(dependency.Contract.Type)
            //                                                          .Invoke(ref local));
            //    default:
            //        return data;
            //}
        }


        public abstract object? Activate(ref DependencyInfo<TDependency> dependency);
    }
}
