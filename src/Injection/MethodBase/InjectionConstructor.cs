using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder.Selection;
using Unity.Policy;
using Unity.Injection;
using Unity.Utility;

namespace Unity
{
    /// <summary>
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructor : InjectionMember, 
                                        IConstructorSelectorPolicy
    {
        #region Fields

        private readonly Type[] _types;
        private ConstructorInfo _constructor;
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
            _parameterValues = (parameterValues ?? throw new ArgumentNullException(nameof(parameterValues)))
                .Select(InjectionParameterValue.ToParameter)
                .ToArray();
        }

        #endregion


        #region InjectionMember

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Interface registered, ignored in this implementation.</param>
        /// <param name="mappedToType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            var typeToCreate = mappedToType;
            var constructors = typeToCreate.GetTypeInfo()
                                           .DeclaredConstructors
                                           .Where(c => c.IsStatic == false && c.IsPublic);
            if (null != _parameterValues)
            {
                _constructor = constructors.FirstOrDefault(info => _parameterValues.Matches(info.GetParameters().Select(p => p.ParameterType))) ??
                       throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Constants.NoSuchConstructor,
                               typeToCreate.FullName, string.Join(", ", _parameterValues.Select(p => p.ParameterTypeName).ToArray())));
            }
            else if (null != _types)
            {
                _constructor = constructors.FirstOrDefault(info => info.GetParameters().ParametersMatch(_types)) ??
                       throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                           Constants.NoSuchConstructor, typeToCreate.FullName, string.Join(", ", _types.Select(t => t.Name))));

                _parameterValues = _constructor.GetParameters().Select(ToResolvedParameter).ToArray();
            }
            else
            {
                _constructor = constructors.FirstOrDefault(info => 0 == info.GetParameters().Length) ??
                       throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                           Constants.NoSuchConstructor, typeToCreate.FullName, string.Empty));

                _parameterValues = new InjectionParameterValue[0];
            }

            policies.Set(registeredType, name, typeof(IConstructorSelectorPolicy), this);
        }

        public override bool BuildRequired => true;

        #endregion


        #region IConstructorSelectorPolicy

        object IConstructorSelectorPolicy.SelectConstructor<TContext>(ref TContext context)
        {
            SelectedConstructor result;

            var typeInfo = context.Type.GetTypeInfo();
            var methodHasOpenGenericParameters = _constructor.GetParameters()
                .Select(p => p.ParameterType.GetTypeInfo())
                .Any(i => i.IsGenericType && i.ContainsGenericParameters);

            var ctorTypeInfo = _constructor.DeclaringType.GetTypeInfo();

            if (!methodHasOpenGenericParameters && !(ctorTypeInfo.IsGenericType && ctorTypeInfo.ContainsGenericParameters))
            {
                result = new SelectedConstructor(_constructor);
            }
            else
            {
                var closedCtorParameterTypes = _constructor.GetClosedParameterTypes(typeInfo.GenericTypeArguments);
                var constructor = typeInfo.DeclaredConstructors.Single(c => !c.IsStatic && c.GetParameters().ParametersMatch(closedCtorParameterTypes));
                result = new SelectedConstructor(constructor);
            }

            foreach (var parameterValue in _parameterValues)
            {
                var resolver = parameterValue.GetResolverPolicy(context.Type);
                result.AddParameterResolver(resolver);
            }

            return result;
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
