using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Injection;

namespace Unity
{
    public abstract partial class MemberPipeline<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            if (null != builder.SeedExpression) return builder.Express();

            var expressions = builder.Express();

            var selector = GetOrDefault(builder.Policies);
            var members = selector.Invoke(builder.Type, builder.InjectionMembers);

            return expressions.Concat(ExpressionsFromSelection(builder.Type, members));
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

        protected virtual Expression GetResolverExpression(TMemberInfo info, object? resolver) => throw new NotImplementedException();

        #endregion
    }
}
