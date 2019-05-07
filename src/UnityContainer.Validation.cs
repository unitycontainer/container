using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Validation

        private Func<Type?, Type?, Type> ValidateType = (Type? typeFrom, Type? typeTo) =>
        {
            return typeFrom ??
                   typeTo ??
                   throw new ArgumentException($"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must not be 'null'");
        };

        private Func<IEnumerable<Type>?, Type, Type[]?> ValidateTypes = (IEnumerable<Type>? types, Type type) =>
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == types) return null;

            var array = types.Where(t => null != t).ToArray();
            return 0 == array.Length ? null : array;
        };

        #endregion


        #region Diagnostic Validation

        private Type DiagnosticValidateType(Type? typeFrom, Type? typeTo)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var infoFrom = typeFrom?.GetTypeInfo();
            var infoTo   = typeTo?.GetTypeInfo();
            if (null != infoFrom && !infoFrom.IsGenericType && 
                null != infoTo   && !infoTo.IsGenericType   && !infoFrom.IsAssignableFrom(infoTo))
#else
            if (null != typeFrom && !typeFrom.IsGenericType &&
                null != typeTo   && !typeTo.IsGenericType   && !typeFrom.IsAssignableFrom(typeTo))
#endif
                throw new ArgumentException($"The type {typeTo} cannot be assigned to variables of type {typeFrom}.");

            return typeFrom ??
                   typeTo ??
                   throw new ArgumentException($"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must be not 'null'");
        }

        private Type[]? DiagnosticValidateTypes(IEnumerable<Type>? types, Type type)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == types) return null;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var infoTo = type.GetTypeInfo();
#endif
            var array = types.Select(t =>
                          {
                              if (null == t) throw new ArgumentException($"Enumeration contains null value.", "interfaces");

#if NETSTANDARD1_0 || NETCOREAPP1_0
                              var infoFrom = t?.GetTypeInfo();
                              if (null != infoFrom && !infoFrom.IsGenericType && 
                                  null != infoTo   && !infoTo.IsGenericType   && !infoFrom.IsAssignableFrom(infoTo))
#else
                              if (null != t && !t.IsGenericType &&
                                  null != type && !type.IsGenericType && !t.IsAssignableFrom(type))
#endif
                                  throw new ArgumentException($"The type {type} cannot be assigned to variables of type {t}.");

                              return t;
                          })
                         .ToArray();
            return 0 == array.Length ? null : array;
        }

        #endregion
    }
}
