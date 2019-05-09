using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    public abstract partial class MemberPipeline<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region PipelineBuilder

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            if (null != builder.Seed) return builder.Pipeline();

            var pipeline = builder.Pipeline();
            var selector = GetOrDefault(builder.Registration);
            var members = selector.Invoke(builder.Type, builder.Registration);
            var resolvers = ResolversFromSelection(builder.Type, members).ToArray();

            return 0 == resolvers.Length 
                ? pipeline 
                : (ref BuilderContext context) =>
                {
                    // Initialize Fields
                    foreach (var resolver in resolvers) resolver(ref context);

                    // Process rest of the initialization
                    return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
                };
        }


        #endregion


        #region Selection Processing

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> ResolversFromSelection(Type type, IEnumerable<object> members)
        {
            foreach (var member in members)
            {
                switch (member)
                {
                    // MemberInfo
                    case TMemberInfo info:
                        object value = DependencyAttribute.Instance;
                        foreach (var node in AttributeFactories)
                        {
                            var attribute = GetCustomAttribute(info, node.Type);
                            if (null == attribute) continue;

                            value = null == node.Factory ? (object)attribute : node.Factory(attribute, info, null);
                            break;
                        }
                        yield return GetResolverDelegate(info, value);
                        break;

                    // Injection Member
                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        yield return GetResolverDelegate(injectionMember.MemberInfo(type), injectionMember.Data);
                        break;

                    case Exception exception:
                        Debug.Assert(exception is InvalidRegistrationException, "Must be InvalidRegistrationException");
                        yield return (ref BuilderContext c) => throw exception;
                        yield break;

                    // Unknown
                    default:
                        yield return (ref BuilderContext c) => 
                            throw new InvalidRegistrationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                        yield break;
                }
            }
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object? resolver) 
            => throw new NotImplementedException();

        #endregion
    }
}
