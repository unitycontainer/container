using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public partial class ParametersProcessor
    {
        #region Fields

        protected static readonly Expression CallNewGuidExpr = Expression.Call(typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid)))!;

        protected static readonly PropertyInfo DataPropertyInfo = typeof(Exception).GetTypeInfo().GetDeclaredProperty(nameof(Exception.Data))!;

        protected static readonly MethodInfo AddMethodInfo = typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add))!;

        protected static readonly ParameterExpression ExceptionExpr = Expression.Variable(typeof(Exception), "exception");

        protected static readonly MemberExpression ExceptionDataExpr = Expression.MakeMemberAccess(ExceptionExpr, DataPropertyInfo);

        #endregion

        
        #region Constructors

        public ParametersProcessor()
        {
            AttributeFactories = new[]
            {
                new AttributeFactory(typeof(DependencyAttribute),         (a)=>((DependencyResolutionAttribute)a).Name, DependencyResolverFactory),
                new AttributeFactory(typeof(OptionalDependencyAttribute), (a)=>((DependencyResolutionAttribute)a).Name, OptionalDependencyResolverFactory),
            };
        }

        #endregion


        #region Public Members

        // TODO: Requires optimization
        public void AddFactories(IEnumerable<AttributeFactory> factories)
        {
            AttributeFactories = AttributeFactories.Concat(factories).ToArray();
        }

        public AttributeFactory[] AttributeFactories { get; private set; }

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

        #endregion


        #region Nested Types

        public struct AttributeFactory
        {
            public readonly Type Type;
            public Func<Attribute, ParameterInfo, object?, ResolveDelegate<PipelineContext>> Factory;
            public Func<Attribute, string?> Name;

            public AttributeFactory(Type type, Func<Attribute, string?> getName, Func<Attribute, ParameterInfo, object?, ResolveDelegate<PipelineContext>> factory)
            {
                Type = type;
                Name = getName;
                Factory = factory;
            }
        }

        #endregion

    }
}
