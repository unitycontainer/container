using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// An <see cref="InjectionMember"/> that configures the
    /// container to call a method as part of buildup.
    /// </summary>
    public class InjectionMethod : MethodBaseMember<MethodInfo>
    {
        #region Fields

        private readonly string _methodName;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given methods with the given parameters.
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="arguments">Parameter values for the method.</param>
        public InjectionMethod(string methodName, params object[] arguments)
            : base(arguments)
        {
            _methodName = methodName;
        }

        #endregion


        #region MethodBaseMember

        protected override void Validate(Type type)
        {
            base.Validate(type);

            // TODO: 5.9.0 Verify necessity 
            if (MemberInfo.IsStatic) ThrowIllegalMember(Constants.CannotInjectStaticMethod, type);
            if (MemberInfo.IsGenericMethodDefinition) ThrowIllegalMember(Constants.CannotInjectGenericMethod, type);
            if (MemberInfo.GetParameters().Any(param => param.IsOut)) ThrowIllegalMember(Constants.CannotInjectMethodWithOutParams, type);
            if (MemberInfo.GetParameters().Any(param => param.ParameterType.IsByRef)) ThrowIllegalMember(Constants.CannotInjectMethodWithRefParams, type);
        }

#if NETSTANDARD1_0
        public override bool Equals(MethodInfo other)
        {
            if (other.Name != _methodName) return false;

            var parameterTypes = other.GetParameters()
                                      .Select(p => p.ParameterType);

            if (Info.ContainsGenericParameters)
            {
                return _parameterTypes.Length == parameterTypes.Count();
            }

            return _parameterTypes.SequenceEqual(parameterTypes);
        }
#endif

        protected override IEnumerable<MethodInfo> DeclaredMembers(TypeInfo info)
        {
            if (null == info) return Enumerable.Empty<MethodInfo>();

            if (typeof(object) == info.DeclaringType)
            {
                return info.DeclaredMethods.Where(m => !m.IsStatic)
                           .Where(m => _methodName == m.Name);
            }

            return info.DeclaredMethods.Where(m => !m.IsStatic)
                       .Concat(DeclaredMembers(info.BaseType?.GetTypeInfo()))
                       .Where(m => _methodName == m.Name);
        }

        #endregion


        #region Implementation

        private void ThrowIllegalMember(string message, Type type)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    message,
                    type.GetTypeInfo().Name,
                    _methodName,
                    // TODO: 5.9.0
                    "string.Join(", ", _injectionParameterValues.Select(mp => mp.ParameterTypeName))"));
        }

        #endregion
    }
}
