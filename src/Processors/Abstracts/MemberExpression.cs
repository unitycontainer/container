using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Selection Processing

        protected virtual IEnumerable<Expression> ExpressionsFromSelection(Type type, IEnumerable<object> members)
        {
            foreach (var member in members)
            {

                switch (member)
                {
                    // TMemberInfo
                    case TMemberInfo info:
                        object value = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) 
                                     ?? DependencyAttribute.Instance; 

                        yield return GetResolverExpression(info, value);
                        break;
                    
                        // Injection Member
                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        yield return GetResolverExpression(injectionMember.MemberInfo(type), 
                                                           injectionMember.Data);
                        break;

                    // Unknown
                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        #endregion


        #region Implementation

        protected abstract Expression GetResolverExpression(TMemberInfo info, object? resolver);

        #endregion
    }
}
