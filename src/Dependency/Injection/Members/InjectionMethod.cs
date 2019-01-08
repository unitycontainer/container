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
    public class InjectionMethod : MethodBase<MethodInfo>
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given methods with the given parameters.
        /// </summary>
        /// <param name="name">Name of the method to call.</param>
        /// <param name="arguments">Parameter values for the method.</param>
        public InjectionMethod(string name, params object[] arguments)
            : base(name, null == arguments || 0 == arguments.Length 
                  ? InjectionMethodAttribute.Instance : arguments)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
#if NETCOREAPP1_0 || NETSTANDARD1_0
            if (null == type) return Enumerable.Empty<MethodInfo>();

            var info = type.GetTypeInfo();
            if (typeof(object) == info.DeclaringType)
            {
                return info.DeclaredMethods.Where(m => !m.IsStatic)
                           .Where(m => Name == m.Name);
            }

            return info.DeclaredMethods.Where(m => !m.IsStatic)
                       .Concat(DeclaredMembers(info.BaseType))
                       .Where(m => Name == m.Name);
#else
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                       .Where(m => Name == m.Name);
#endif
        }

        protected override void ValidateInjectionMember(Type type)
        {
            base.ValidateInjectionMember(type);

            // TODO: 5.9.0 Verify necessity 
            if (MemberInfo.IsStatic) ThrowIllegalMember("The method {0}.{1}({2}) is static. Static methods cannot be injected.", type);
            if (MemberInfo.IsGenericMethodDefinition) ThrowIllegalMember("The method {0}.{1}({2}) is an open generic method. Open generic methods cannot be injected.", type);
            if (MemberInfo.GetParameters().Any(param => param.IsOut)) ThrowIllegalMember("The method {0}.{1}({2}) has at least one out parameter. Methods with out parameters cannot be injected.", type);
            if (MemberInfo.GetParameters().Any(param => param.ParameterType.IsByRef)) ThrowIllegalMember("The method {0}.{1}({2}) has at least one ref parameter.Methods with ref parameters cannot be injected.", type);
        }

#if NETSTANDARD1_0

        public override bool Equals(MethodInfo other)
        {
            if (null == other || other.Name != Name) return false;

            var parameterTypes = other.GetParameters()
                                      .Select(p => p.ParameterType)
                                      .ToArray();

            if (MemberInfo.ContainsGenericParameters)
                return Data.Length == parameterTypes.Length;

            return MemberInfo.GetParameters()
                             .Select(p => p.ParameterType)
                             .SequenceEqual(parameterTypes);
        }

#endif
        #endregion


        #region Implementation

        private void ThrowIllegalMember(string message, Type type)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    message,
                    type.GetTypeInfo().Name,
                    // TODO: 5.9.0
                    Name, "string.Join(", ", _injectionParameterValues.FromType(mp => mp.ParameterTypeName))"));
        }

        #endregion
    }
}
