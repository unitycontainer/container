using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Injection;
using Unity.Registration;

namespace Unity
{
    public abstract partial class MemberPipeline<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region PipelineBuilder

        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<Pipeline> enumerator,
                                                      Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        #endregion


        #region Selection Processing

        protected virtual IEnumerable<Expression> ExpressionsFromSelection(Type type, IEnumerable<object> members)
        {
            foreach (var member in members)
            {

                switch (member)
                {
                    // TMemberInfo
                    case TMemberInfo info:
                        object value = DependencyAttribute.Instance; 
                        foreach (var node in AttributeFactories)
                        {
                            var attribute = GetCustomAttribute(info, node.Type);
                            if (null == attribute) continue;

                            value = null == node.Factory ? (object)attribute : node.Factory(attribute, info, null);
                            break;
                        }

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
