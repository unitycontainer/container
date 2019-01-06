using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public delegate ResolveDelegate<BuilderContext> ResolutionParameterAttributeFactory(Attribute attribute, object info, object resolver, object defaultValue);

    public abstract partial class MethodBaseProcessor<TMemberInfo>
    {
        #region Parameter Factory

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateParameterResolvers(ParameterInfo[] parameters, object[] injectors = null)
        {
            object[] resolvers = null != injectors && 0 == injectors.Length ? null : injectors;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers ? parameter : PreProcessResolver(parameter, resolvers[i]);

                // Check if has default value
                var defaultValue = parameter.HasDefaultValue
                    ? parameter.DefaultValue
                    : null;

                // Check for registered attributes first
                var expression = FromAttribute(parameter, defaultValue, resolver);
                if (null == expression)
                {
                    // Check if has default value
                    if (!parameter.HasDefaultValue)
                    {
                        // Plain vanilla case
                        expression = (ref BuilderContext context) => context.Resolve(parameter, null, resolver);
                    }
                    else
                    {
                        expression = (ref BuilderContext context) =>
                        {
                            try
                            {
                                return context.Resolve(parameter, null, resolver);
                            }
                            catch
                            {
                                return defaultValue;
                            }
                        };
                    }
                }

                yield return expression;
            }

            ResolveDelegate<BuilderContext> FromAttribute(ParameterInfo param, object defaultValue, object data)
            {
                foreach (var node in AttributeFactories)
                {
                    if (null == node.ResolutionFactory) continue;
                    var attribute = param.GetCustomAttribute(node.Type);
                    if (null == attribute) continue;

                    // If found match, use provided factory to create expression
                    return ((ResolutionParameterAttributeFactory)node.ResolutionFactory)(attribute, param, data, defaultValue);
                }

                return null;
            }
        }

        #endregion


        #region Attribute Factory

        private static ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null)
        {
            var parameter = (ParameterInfo)info;

            if (!parameter.HasDefaultValue)
                return (ref BuilderContext context) => context.Resolve((ParameterInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver);
            else
            {
                return (ref BuilderContext context) =>
                {
                    try
                    {
                        return context.Resolve((ParameterInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver);
                    }
                    catch
                    {
                        return defaultValue;
                    }
                };
            }
        }

        private static ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null)
        {
            var parameter = (ParameterInfo)info;

            return (ref BuilderContext context) =>
            {
                try
                {
                    return context.Resolve((ParameterInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver ?? OptionalDependencyAttribute.Instance);
                }
                catch
                {
                    return defaultValue;
                }
            };
        }

        #endregion
    }
}
