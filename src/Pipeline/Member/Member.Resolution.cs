using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    public abstract partial class MemberPipeline<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            if (null != builder.SeedMethod) return builder.Pipeline();

            var pipeline = builder.Pipeline();
            var members = (IReadOnlyCollection<object>)Select(builder.Type, builder.InjectionMembers);

            Debug.Assert(null != members);

            var i = 0;
            var resolvers = new ResolveDelegate<PipelineContext>[members.Count];

            foreach (var member in members)
            { 
                switch (member)
                {
                    // MemberInfo
                    case TMemberInfo info:
                        resolvers[i++] = GetResolverDelegate(info);
                        break;

                    // Injection Member
                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        resolvers[i++] = GetResolverDelegate(injectionMember.MemberInfo(builder.Type), injectionMember.Data);
                        break;

                    case Exception exception:
                        Debug.Assert(exception is InvalidRegistrationException, "Must be InvalidRegistrationException");
                        resolvers[i++] = (ref PipelineContext c) => throw exception;
                        break;

                    // Unknown
                    default:
                        throw new InvalidRegistrationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }

            return 0 == resolvers.Length 
                ? pipeline 
                : (ref PipelineContext context) =>
                {
                    // Initialize Fields
                    foreach (var resolver in resolvers) resolver(ref context);

                    // Process rest of the initialization
                    return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
                };
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<PipelineContext> GetResolverDelegate(TMemberInfo info)
            => throw new NotImplementedException();

        protected virtual ResolveDelegate<PipelineContext> GetResolverDelegate(TMemberInfo info, object? data) 
            => throw new NotImplementedException();

        #endregion
    }
}
