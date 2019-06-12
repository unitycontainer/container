using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    public abstract partial class ParametersPipeline<TMemberInfo> : MemberPipeline<TMemberInfo, object[]>
                                                where TMemberInfo : MethodBase
    {
        #region Fields

        protected readonly UnityContainer Container;

        #endregion


        #region Constructors

        protected ParametersPipeline(Type attribute, UnityContainer container)
            : base(container)
        {
            Container = container;
            Markers = new[] { attribute };
        }

        #endregion


        #region Public Members

        public void AddMarkers(IEnumerable<Type> attributes)
        {
            Markers = Markers.Concat(attributes).ToArray();
        }

        public Type[] Markers { get; private set; }

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
                    return (ResolveDelegate<PipelineContext>)policy.Resolve;

                case IResolverFactory<ParameterInfo> factory:
                    return factory.GetResolver<PipelineContext>(parameter);

                case Type type:
                    return 
                        typeof(Type) == parameter.ParameterType
                          ? type 
                          : type == parameter.ParameterType 
                              ? FromAttribute(parameter) 
                              : FromType(type);
            }

            return resolver;
        }

        private object FromType(Type type)
        {
            return (ResolveDelegate<PipelineContext>)((ref PipelineContext context) => context.Resolve(type, (string?)null));
        }

        private object FromAttribute(ParameterInfo info)
        {
#if NET40
            var defaultValue = !(info.DefaultValue is DBNull) ? info.DefaultValue : null;
#else
            var defaultValue = info.HasDefaultValue ? info.DefaultValue : null;
#endif
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

        protected bool CanResolve(ParameterInfo info)
        {
            foreach (var node in AttributeFactories)
            {
                if (null == node.Factory) continue;
                var attribute = info.GetCustomAttribute(node.Type);
                if (null == attribute) continue;

                // If found match, use provided factory to create expression
                return Container.CanResolve(info.ParameterType, node.Name(attribute));
            }

            return Container.CanResolve(info.ParameterType, null);
        }

        #endregion


        #region Attribute Factories

        protected override ResolveDelegate<PipelineContext> DependencyResolverFactory(Attribute attribute, object info, object? value = null)
        {
            return (ref PipelineContext context) => context.Resolve(((ParameterInfo)info).ParameterType, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected override ResolveDelegate<PipelineContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object? value = null)
        {
            return (ref PipelineContext context) =>
            {
                try
                {
                    return context.Resolve(((ParameterInfo)info).ParameterType, 
                      ((DependencyResolutionAttribute)attribute).Name);
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
