using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class MethodBaseProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Constructors

        protected MethodBaseProcessor(IPolicySet policySet, Type attribute)
            : base(policySet, attribute)
        {
        }

        #endregion


        #region Overrides

        protected override Type MemberType(TMemberInfo info) => info.DeclaringType;

        #endregion


        #region Implementation

        private object PreProcessResolver(ParameterInfo parameter, object resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<BuilderContext>)policy.Resolve;

                case IResolverFactory<ParameterInfo> factory:
                    return factory.GetResolver<BuilderContext>(parameter);

                case Type type:
                    return typeof(Type) == parameter.ParameterType
                        ? type : FromAttribute(parameter);
            }

            return resolver;
        }

        private object FromAttribute(ParameterInfo info)
        {
            var defaultValue = info.HasDefaultValue ? info.DefaultValue : null;
            foreach (var node in AttributeFactories)
            {
                if (null == node.Factory) continue;
                var attribute = info.GetCustomAttribute(node.Type);
                if (null == attribute) continue;

                // If found match, use provided factory to create expression
                return node.Factory(attribute, info, defaultValue);
            }

            return info;
        }

        #endregion


        #region Attribute Factory

        protected override ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object value = null)
        {
            return (ref BuilderContext context) => context.Resolve(((ParameterInfo)info).ParameterType, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected override ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object value = null)
        {
            return (ref BuilderContext context) =>
            {
                try
                {
                    return context.Resolve(((ParameterInfo)info).ParameterType, ((DependencyResolutionAttribute)attribute).Name);
                }
                catch
                {
                    return value;
                }
            };
        }

        #endregion
    }
}
