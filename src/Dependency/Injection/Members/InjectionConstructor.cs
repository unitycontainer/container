using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructor : MethodBaseMember<ConstructorInfo>
    {
        #region Fields

        private InjectionParameterValue[] _parameterValues;
        protected override string Designation { get; } = "constructor";

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a default constructor.
        /// </summary>
        public InjectionConstructor()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameter types.
        /// </summary>
        /// <param name="types">The types of the parameters of the constructor.</param>
        public InjectionConstructor(params Type[] types)
            : base(types)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="parameterValues">The values for the parameters, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public InjectionConstructor(params object[] parameterValues)
            : base(parameterValues)
        {
            _parameterValues = (parameterValues ?? throw new ArgumentNullException(nameof(parameterValues)))
                .Select(InjectionParameterValue.ToParameter)
                .ToArray();
        }

        public InjectionConstructor(ConstructorInfo info, params object[] parameterValues)
            : base(parameterValues)
        {
            Info = info;
            _parameterValues = (parameterValues ?? throw new ArgumentNullException(nameof(parameterValues)))
                .Select(InjectionParameterValue.ToParameter)
                .ToArray();
        }

        #endregion


        #region InjectionMember

        public override bool BuildRequired => true;

        #endregion


        #region MethodBaseMember

        public override ConstructorInfo GetInfo(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var methodHasOpenGenericParameters = Info.GetParameters()
                .Select(p => p.ParameterType.GetTypeInfo())
                .Any(i => i.IsGenericType && i.ContainsGenericParameters);

            var ctorTypeInfo = Info.DeclaringType.GetTypeInfo();

            if (!methodHasOpenGenericParameters && !(ctorTypeInfo.IsGenericType && ctorTypeInfo.ContainsGenericParameters))
                return  Info;

            var closedCtorParameterTypes = Info.GetClosedParameterTypes(typeInfo.GenericTypeArguments);
            return typeInfo.DeclaredConstructors.Single(c => !c.IsStatic && c.GetParameters().ParametersMatch(closedCtorParameterTypes));
        }

        public override bool Equals(ConstructorInfo other)
        {
#if NETSTANDARD1_0
            return true; // TODO: Implement properly
#else
            return other?.MetadataToken == Info.MetadataToken;
#endif
        }

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(TypeInfo info)
        {
            return info.DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic);
        }

        #endregion


        #region IExpressionFactory

        public NewExpression GetExpression<TContext>(Type type)
            where TContext : IResolveContext
        {
            return Expression.New(Info);
        }

        #endregion


        #region Cast To ConstructorInfo

        public static explicit operator ConstructorInfo(InjectionConstructor ctor)
        {
            return ctor.Info;
        }

        #endregion
    }
}
