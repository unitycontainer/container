using System;
using System.Reflection;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    public partial class ParametersProcessor
    {
        private const string _error = "Invalid 'ref' or 'out' parameter '{0}' ({1})";

        public ResolveDelegate<PipelineContext> ParameterResolver(ParameterInfo parameter) 
            => ParameterResolverFactory(parameter, FromAttribute(parameter));

        public virtual ResolveDelegate<PipelineContext> ParameterResolver(ParameterInfo parameter, object injector) 
            => ParameterResolverFactory(parameter, PreProcessResolver(parameter, injector));

        protected virtual ResolveDelegate<PipelineContext> ParameterResolverFactory(ParameterInfo parameter, object resolver)
        {
            if (parameter.ParameterType.IsByRef)
                throw new InvalidRegistrationException(string.Format(_error, parameter.Name, parameter.ParameterType), parameter);
#if NET40
            if (parameter.DefaultValue is DBNull)
#else
            if (!parameter.HasDefaultValue)
#endif
            {
                return (ref PipelineContext context) => context.Resolve(parameter, resolver);
            }
            else
            {
                // Check if has default value
#if NET40
                var defaultValue = !(parameter.DefaultValue is DBNull) ? parameter.DefaultValue : null;
#else
                var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
#endif
                return (ref PipelineContext context) =>
                {
                    try
                    {
                        return context.Resolve(parameter, resolver);
                    }
                    catch
                    {
                        return defaultValue;
                    }
                };
            }
        }



        #region Attribute Resolver Factories

        protected virtual ResolveDelegate<PipelineContext> DependencyResolverFactory(Attribute attribute, ParameterInfo info, object? value = null)
        {
            return (ref PipelineContext context) => context.Resolve(info.ParameterType, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected virtual ResolveDelegate<PipelineContext> OptionalDependencyResolverFactory(Attribute attribute, ParameterInfo info, object? value = null)
        {
            return (ref PipelineContext context) =>
            {
                try
                {
                    return context.Resolve(info.ParameterType, ((DependencyResolutionAttribute)attribute).Name);
                }
                catch (Exception ex) when (!(ex is CircularDependencyException))
                {
                    return value;
                }
            };
        }

        #endregion

    }
}
