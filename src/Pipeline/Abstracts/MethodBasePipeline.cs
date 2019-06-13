using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    public abstract class MethodBasePipeline<TMemberInfo> : MemberPipeline<TMemberInfo, object[]>
                                        where TMemberInfo : MethodBase
    {
        #region Fields

        protected readonly UnityContainer Container;
        protected readonly ParametersProcessor Processor;

        #endregion


        #region Constructors

        protected MethodBasePipeline(Type attribute, UnityContainer container, ParametersProcessor? processor)
            : base(container)
        {
            Container = container;
            Markers = new[] { attribute };
            Processor = processor ?? new ParametersProcessor();
        }

        #endregion


        #region Markers

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

        protected virtual IEnumerable<ResolveDelegate<PipelineContext>> ParameterResolvers(ParameterInfo[] parameters, object? injectors)
        {
            if (null != injectors && injectors is object[] resolvers && 0 != resolvers.Length)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    yield return Processor.ParameterResolver(parameters[i], resolvers[i]);
                }
            }
            else
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    yield return Processor.ParameterResolver(parameters[i]);
                }
            }
        }

        protected virtual IEnumerable<Expression> ParameterExpressions(ParameterExpression[] expressions, ParameterInfo[] parameters, object? injectors)
        {
            if (null != injectors && injectors is object[] resolvers && 0 != resolvers.Length)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    yield return Processor.ParameterExpression(expressions[i], parameters[i], resolvers[i]);
                }
            }
            else
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    yield return Processor.ParameterExpression(expressions[i], parameters[i]);
                }
            }
        }

        protected ParameterExpression ToVariable(ParameterInfo info)
        {
            if (info.ParameterType.IsByRef)
                throw  new InvalidRegistrationException($"Invalid By Ref parameter '{info.Name}' ({info.ParameterType})", info);

            return Expression.Variable(info.ParameterType, info.Name);
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
    }
}
