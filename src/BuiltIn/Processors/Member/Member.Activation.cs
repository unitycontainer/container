using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public virtual object? Build(ref PipelineContext context, TDependency info, object? data)
        {
            throw new NotImplementedException();
        }

        public virtual object? Build(ref PipelineContext context, TDependency info)
        {
            // Get annotation info
            var attribute = GetImportAttribute(info);

            // Get contract of the dependency
            Contract contract = (null == attribute)
                ? new Contract(DependencyType(info))
                : new Contract(attribute.ContractType ?? DependencyType(info), attribute.ContractName);

            ResolverOverride[] overrides;

            if (null != (overrides = context.Overrides))
            {
                ResolverOverride? candidate = null;
                MatchRank matchRank = MatchRank.NoMatch;

                for (var index = overrides.Length - 1; index >= 0; --index)
                {
                    var @override = overrides[index];

                    // Check if this parameter is overridden
                    if (@override.Equals(info))
                    {
                        var action = context.Start(info);
                        //var local = new PipelineContext(ref context, parameter, @override);

                        //return ReinterpretValue(ref local, @override.Resolve(ref local));
                    }

                    // Check if dependency override for the contract is provided
                    var match = @override.MatchTo(in contract);
                    if (MatchRank.NoMatch == match) continue;

                    if (match > matchRank)
                    {
                        matchRank = match;
                        candidate = @override;
                    }
                }

                // No exact matches but this one is compatible
                if (null != candidate)
                { 
                }
            }


            throw new NotImplementedException();
        }

    }
}
