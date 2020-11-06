using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null != context.Target, "Target should never be null");

            bool attribute = default; 
            ResolverOverride? @override;
            ReflectionInfo<TDependency> reflectionInfo = default;

            var injected   = GetInjected<InjectionMemberInfo<TMemberInfo>>(context.Registration);
            var injections = injected;

            foreach(var member in GetMembers(context.Type))
            {
                if (context.IsFaulted) return;

                attribute = FillImportInfo(Unsafe.As<TDependency>(member), ref reflectionInfo.Import);

                // Injection first
                while(null != injected)
                {
                    if (MatchRank.ExactMatch == injected.Match(member))
                    {
                        var info = ((IReflectionProvider<TDependency>)injected).FillReflectionInfo(ref reflectionInfo);

                        // Check for override
                        if (null != (@override = context.GetOverride(in reflectionInfo.Import)))
                        {
                            if (@override.Value is IReflectionProvider<TDependency> provider)
                            {
                                var providerInfo = provider.FillReflectionInfo(ref reflectionInfo);
                                Build(ref context, in reflectionInfo.Import, in reflectionInfo.Data);
                            }
                            else
                                Build(ref context, in reflectionInfo.Import, AsImportData(Unsafe.As<TDependency>(member), @override.Value));
                        }
                        else
                            Build(ref context, in reflectionInfo.Import, in reflectionInfo.Data);

                        goto ContinueToNext;
                    }
                    
                    injected = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injected.Next);
                }
                
                // Annotation second
                if (!attribute) goto ContinueToNext;

                var annotation = new ReflectionInfo<TMemberInfo>(member, reflectionInfo.Import.ContractType ?? MemberType(Unsafe.As<TDependency>(member)), 
                                                                         reflectionInfo.Import.ContractName, 
                                                                         reflectionInfo.Import.AllowDefault);
                // Check for override
                if (null != (@override = context.GetOverride(in annotation.Import)))
                {
                    if (@override.Value is IReflectionProvider<TDependency> provider)
                    {
                        var providerInfo = provider.FillReflectionInfo(ref reflectionInfo);
                        Build(ref context, in reflectionInfo.Import, in reflectionInfo.Data);
                    }
                    else
                        Build(ref context, in annotation.Import, AsImportData(Unsafe.As<TDependency>(member), @override.Value));
                }
                else
                    Build(ref context, in annotation.Import, in annotation.Data);

                // Rewind for the next member
                ContinueToNext: injected = injections;
            }
        }
    }
}
