using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// An <see cref="InjectionMember"/> that configures the
    /// container to call a method as part of buildup.
    /// </summary>
    public class InjectionMethod : InjectionMember, 
                                   IMemberWithParameters<MethodInfo>,
                                   IEquatable<MethodInfo>
    {
        #region Fields

        private MethodInfo _info;
        private Type[] _parameterTypes;
        private readonly string _methodName;
        private readonly object[] _methodParameters;
        private readonly List<InjectionParameterValue> _injectionParameterValues;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given methods with the given parameters.
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="methodParameters">Parameter values for the method.</param>
        public InjectionMethod(string methodName, params object[] methodParameters)
        {
            _methodName = methodName;
            _methodParameters = methodParameters;
            _injectionParameterValues = InjectionParameterValue.ToParameters(methodParameters).ToList();
        }

        #endregion


        public object[] GetResolvers() => _injectionParameterValues.Cast<object>().ToArray();


        #region InjectionMember

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Type of interface registered, ignored in this implementation.</param>
        /// <param name="mappedToType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            var type = mappedToType ?? registeredType;

            foreach (var method in type.GetMethodsHierarchical())
            {
                if (MethodNameMatches(method, _methodName))
                {
                    if (_injectionParameterValues.Matches(method.GetParameters().Select(p => p.ParameterType)))
                    {
                        _info = method;
                        _parameterTypes = method.GetParameters()
                                                .Select(p => p.ParameterType)
                                                .ToArray();
                    }
                }
            }


            //if (methodInfo == null)
            //{
            //    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
            //        Constants.NoSuchMethod, implementationType.GetTypeInfo().Name,
            //        _methodName, string.Join(", ", _methodParameters.Select(mp => mp.ParameterTypeName))));
            //}

            //if (methodInfo.IsStatic)
            //{
            //    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
            //        Constants.CannotInjectStaticMethod, implementationType.GetTypeInfo().Name,
            //        _methodName, string.Join(", ", _methodParameters.Select(mp => mp.ParameterTypeName))));
            //}

            //if (methodInfo.IsGenericMethodDefinition)
            //{
            //    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
            //        Constants.CannotInjectGenericMethod, implementationType.GetTypeInfo().Name,
            //        _methodName, string.Join(", ", _methodParameters.Select(mp => mp.ParameterTypeName))));
            //}

            //if (methodInfo.GetParameters().Any(param => param.IsOut))
            //{
            //    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
            //        Constants.CannotInjectMethodWithOutParams, implementationType.GetTypeInfo().Name,
            //        _methodName, string.Join(", ", _methodParameters.Select(mp => mp.ParameterTypeName))));
            //}

            //if (methodInfo.GetParameters().Any(param => param.ParameterType.IsByRef))
            //{
            //    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
            //        Constants.CannotInjectMethodWithRefParams, implementationType.GetTypeInfo().Name,
            //        _methodName, string.Join(", ", _methodParameters.Select(mp => mp.ParameterTypeName))));
            //}


            // TODO: if (methodInfo.IsGenericMethodDefinition ||
            //    parameters.Any(param => param.IsOut || param.ParameterType.IsByRef))

            if (_info == null) ThrowIllegalInjectionMethod(Constants.NoSuchMethod, type);
            if (_info.IsStatic) ThrowIllegalInjectionMethod(Constants.CannotInjectStaticMethod, type);
            if (_info.IsGenericMethodDefinition) ThrowIllegalInjectionMethod(Constants.CannotInjectGenericMethod, type);
            if (_info.GetParameters().Any(param => param.IsOut)) ThrowIllegalInjectionMethod(Constants.CannotInjectMethodWithOutParams, type);
            if (_info.GetParameters().Any(param => param.ParameterType.IsByRef)) ThrowIllegalInjectionMethod(Constants.CannotInjectMethodWithRefParams, type);
        }

        public override bool BuildRequired => true;

        #endregion


        #region IMemberWithParameters<MethodInfo>


        public MethodInfo MemberInfo(Type type)
        {
            var info = _info.DeclaringType.GetTypeInfo();
            var methodHasOpenGenericParameters = _info.GetParameters()
                                                      .Select(p => p.ParameterType.GetTypeInfo())
                                                      .Any(i => i.IsGenericType && i.ContainsGenericParameters);

            if (!methodHasOpenGenericParameters && !(info.IsGenericType && info.ContainsGenericParameters))
            {
                return _info;
            }

            var closedMethodParameterTypes = _info
                .GetClosedParameterTypes(type.GetTypeInfo().GenericTypeArguments);

            return type.GetMethodsHierarchical()
                       .Single(m => m.Name.Equals(_info.Name) &&
                                    m.GetParameters().ParametersMatch(closedMethodParameterTypes));
        }

        public object[] GetParameters()
        {
            return _methodParameters;
        }

        #endregion


        #region IEquatable

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MethodInfo other)
                return Equals(other);

            return base.Equals(obj);
        }

        public bool Equals(MethodInfo other)
        {
#if NETSTANDARD1_0
            if (other.Name != _methodName) return false;

            var parameterTypes = other.GetParameters()
                                      .Select(p => p.ParameterType);

            if (_info.ContainsGenericParameters)
            {
                return _parameterTypes.Length == parameterTypes.Count();
            }

            return _parameterTypes.SequenceEqual(parameterTypes);
#else
            return other?.MetadataToken == _info.MetadataToken;
#endif
        }

        #endregion


        #region Cast To MethodInfo

        public static explicit operator MethodInfo(InjectionMethod method)
        {
            return method._info;
        }

#endregion


#region Implementation

        /// <summary>
        /// A small function to handle name matching. You can override this
        /// to do things like case insensitive comparisons.
        /// </summary>
        /// <param name="targetMethod">MethodInfo for the method you're checking.</param>
        /// <param name="nameToMatch">Name of the method you're looking for.</param>
        /// <returns>True if a match, false if not.</returns>
        protected virtual bool MethodNameMatches(MemberInfo targetMethod, string nameToMatch)
        {
            return (targetMethod ?? throw new ArgumentNullException(nameof(targetMethod))).Name == nameToMatch;
        }

        private void ThrowIllegalInjectionMethod(string message, Type type)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    message,
                    type.GetTypeInfo().Name,
                    _methodName,
                    string.Join(", ", _injectionParameterValues.Select(mp => mp.ParameterTypeName))));
        }

#endregion
    }
}
