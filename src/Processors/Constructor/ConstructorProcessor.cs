using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Registration;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : ParametersProcessor<ConstructorInfo>
    {
        #region Fields

        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();

        #endregion


        #region Constructors

        public ConstructorProcessor()
        {
            SelectMethod = SmartSelector;
        }

        #endregion


        #region Public Properties

        public Func<UnityContainer, Type, ConstructorInfo[], object?> SelectMethod { get; set; }

        #endregion


        #region Overrides

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(Type type) => type.SupportedConstructors();

        #endregion


        #region Selection

        protected override object Select(Type type, InternalRegistration registration)
        {
            // Select Injected Members
            if (null != registration.InjectionMembers)
            {
                foreach (var injectionMember in registration.InjectionMembers)
                {
                    if (injectionMember is InjectionMember<ConstructorInfo, object[]>)
                    {
                        return injectionMember;
                    }
                }
            }

            // Enumerate to array
            var constructors = DeclaredMembers(type).ToArray();

            // No constructors
            if (0 == constructors.Length)
            {
                return new InvalidOperationException($"No public constructor is available for type {type.FullName}.",
                    new InvalidRegistrationException());
            }

            // Just one constructor
            if (1 == constructors.Length) return constructors[0];

            // Check for decorated constructors
            foreach (var constructor in constructors)
            {
                if (!constructor.IsDefined(typeof(InjectionConstructorAttribute), true))
                    continue;

                return constructor;
            }

            return SelectMethod(registration.Owner, type, constructors) ??
                new InvalidOperationException($"Unable to select constructor for type {type.FullName}.",
                    new InvalidRegistrationException());
        }

        /// <summary>
        /// Selects default constructor
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be built</param>
        /// <param name="members">All public constructors this type implements</param>
        /// <returns></returns>
        public object? LegacySelector(UnityContainer container, Type type, ConstructorInfo[] members)
        {
            Array.Sort(members, (x, y) => y?.GetParameters().Length ?? 0 - x?.GetParameters().Length ?? 0);

            switch (members.Length)
            {
                case 0:
                    return null;

                case 1:
                    return members[0];

                default:
                    var paramLength = members[0].GetParameters().Length;
                    if (members[1].GetParameters().Length == paramLength)
                    {
                        return new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "The type {0} has multiple constructors of length {1}. Unable to disambiguate.",
                                type.GetTypeInfo().Name,
                                paramLength), new InvalidRegistrationException());
                    }
                    return members[0];
            }
        }

        protected virtual object? SmartSelector(UnityContainer container, Type type, ConstructorInfo[] constructors)
        {
            Array.Sort(constructors, (left, right) =>
            {
                var result = right.GetParameters().Length.CompareTo(left.GetParameters().Length);

                if (result == 0)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    return right.GetParameters()
                                .Sum(p => p.ParameterType.GetTypeInfo().IsInterface ? 1 : 0)
                                .CompareTo(left.GetParameters().Sum(p => p.ParameterType.GetTypeInfo().IsInterface ? 1 : 0));
#else
                    return right.GetParameters()
                                .Sum(p => p.ParameterType.IsInterface ? 1 : 0)
                                .CompareTo(left.GetParameters().Sum(p => p.ParameterType.IsInterface ? 1 : 0));
#endif
                }
                return result;
            });

            foreach (var ctorInfo in constructors)
            {
                var parameters = ctorInfo.GetParameters();
#if NET40
                if (parameters.All(p => (null != p.DefaultValue && !(p.DefaultValue is DBNull)) || CanResolve(container, p)))
#else
                if (parameters.All(p => p.HasDefaultValue || CanResolve(container, p)))
#endif
                {
                    return ctorInfo;
                }
            }

            return new InvalidOperationException(
                $"Failed to select a constructor for {type.FullName}", new InvalidRegistrationException());
        }

        #endregion


        #region Implementation            

        protected bool CanResolve(UnityContainer container, ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute;

            if (null != attribute)
                return CanResolve(container, info.ParameterType, attribute.Name);

            return CanResolve(container, info.ParameterType, null);
        }

        protected bool CanResolve(UnityContainer container, Type type, string? name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
#else
            var info = type;
#endif
            if (info.IsClass)
            {
                // Array could be either registered or Type can be resolved
                if (type.IsArray)
                {
                    return container._isExplicitlyRegistered(type, name) || CanResolve(container, type!.GetElementType(), name);
                }

                // Type must be registered if:
                // - String
                // - Enumeration
                // - Primitive
                // - Abstract
                // - Interface
                // - No accessible constructor
                if (DelegateType.IsAssignableFrom(info) ||
                    typeof(string) == type || info.IsEnum || info.IsPrimitive || info.IsAbstract
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    || !info.DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#else
                    || !type.GetTypeInfo().DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#endif
                    return container._isExplicitlyRegistered(type, name);

                return true;
            }

            // Can resolve if IEnumerable or factory is registered
            if (info.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(IEnumerable<>) || container._isExplicitlyRegistered(genericType, name))
                {
                    return true;
                }
            }

            // Check if Type is registered
            return container._isExplicitlyRegistered(type, name);
        }

        #endregion
    }
}
