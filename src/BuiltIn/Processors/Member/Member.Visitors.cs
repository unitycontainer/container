using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        //public virtual object? Activate(ref MemberDependency dependency, object? data)
        //{
        //    PipelineContext local;

        //    switch (data)
        //    {
        //        case ResolveDelegate<PipelineContext> resolver:
        //            local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
        //            return local.GetValue(dependency.Info, resolver(ref local));

        //        case IResolve iResolve:
        //            local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
        //            return local.GetValue(dependency.Info, iResolve.Resolve(ref local));

        //        case IResolverFactory<TDependency> infoFactory:
        //            local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
        //            return local.GetValue(dependency.Info, infoFactory.GetResolver<PipelineContext>(dependency.Info)
        //                                                              .Invoke(ref local));
        //        case IResolverFactory<Type> typeFactory:
        //            local = new PipelineContext(ref dependency.Parent, ref dependency.Contract, data);
        //            return local.GetValue(dependency.Info, typeFactory.GetResolver<PipelineContext>(dependency.Contract.Type)
        //                                                              .Invoke(ref local));
        //        default:
        //            return data;
        //    }
        //}


        //protected ResolveDelegate<PipelineContext> FromInfoFactory()
    }
}
