using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Selection Processing

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> ResolversFromSelection(Type type, IEnumerable<object> members)
        {
            HashSet<TMemberInfo> memberSet = new HashSet<TMemberInfo>();

            foreach (var member in members)
            {
                switch (member)
                {
                    // MemberInfo
                    case TMemberInfo info:
                        if (!memberSet.Add(info)) continue;
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
                        var selection = injectionMember.MemberInfo(type);
                        if (!memberSet.Add(selection)) continue;
                        yield return GetResolverDelegate(selection, injectionMember.Data);
                        break;

                    // Unknown
                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        #endregion


        #region Implementation

        protected abstract ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object resolver);

        #endregion
    }
}
