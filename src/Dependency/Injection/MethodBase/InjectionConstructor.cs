using System;
using System.Globalization;
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

        private readonly Type[] _types;
        private readonly object[] _parameters;
        private InjectionParameterValue[] _parameterValues;


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
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="parameterValues">The values for the parameters, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public InjectionConstructor(params object[] parameterValues)
        {
            _parameters = parameterValues;
            _parameterValues = (parameterValues ?? throw new ArgumentNullException(nameof(parameterValues)))
                .Select(InjectionParameterValue.ToParameter)
                .ToArray();
        }

        public InjectionConstructor(ConstructorInfo info, params object[] parameterValues)
        {
            Info = info;
            _parameters = parameterValues;
            _parameterValues = (parameterValues ?? throw new ArgumentNullException(nameof(parameterValues)))
                .Select(InjectionParameterValue.ToParameter)
                .ToArray();
        }

        #endregion


        #region InjectionMember

        public override InjectionMember OnType<T>() => OnType(typeof(T));

        public override InjectionMember OnType(Type targetType)
        {
            var typeToCreate = targetType;
            var constructors = typeToCreate.GetTypeInfo()
                .DeclaredConstructors
                .Where(c => c.IsStatic == false && c.IsPublic);
            if (null != _parameterValues)
            {
                Info = constructors.FirstOrDefault(info => _parameterValues.Matches(info.GetParameters().Select(p => p.ParameterType))) ??
                               throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Constants.NoSuchConstructor,
                                   typeToCreate.FullName, string.Join(", ", _parameterValues.Select(p => p.ParameterTypeName).ToArray())));
            }
            else if (null != _types)
            {
                Info = constructors.FirstOrDefault(info => info.GetParameters().ParametersMatch(_types)) ??
                               throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                   Constants.NoSuchConstructor, typeToCreate.FullName, string.Join(", ", _types.Select(t => t.Name))));

                _parameterValues = Info.GetParameters().Select(ToResolvedParameter).ToArray();
            }
            else
            {
                Info = constructors.FirstOrDefault(info => 0 == info.GetParameters().Length) ??
                               throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                   Constants.NoSuchConstructor, typeToCreate.FullName, string.Empty));

                _parameterValues = new InjectionParameterValue[0];
            }

            return this;
        }

        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            OnType(mappedToType);
        }

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

        public override object[] GetParameters()
        {
            return _parameterValues;
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


        #region Implementation

        private InjectionParameterValue ToResolvedParameter(ParameterInfo parameter)
        {
            return new ResolvedParameter(parameter.ParameterType, parameter.GetCustomAttributes(false)
                                                                           .OfType<DependencyAttribute>()
                                                                           .FirstOrDefault()?.Name);
        }

        #endregion
    }
}
