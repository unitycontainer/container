using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;

namespace Unity
{
    public partial class ConstructorPipeline
    {
        public override object Select(Type type, InjectionMember[]? injectionMembers)
        {
            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
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
                return new InvalidRegistrationException($"Type {type.FullName} has no accessible constructors");

            // One constructor
            if (1 == constructors.Length) 
                return constructors[0];

            // Check for constructors decorated with attribute
            foreach (var constructor in constructors)
            {
                if (!constructor.IsDefined(typeof(InjectionConstructorAttribute), true))
                    continue;

                return constructor;
            }

            return SelectMethod(type, constructors) ?? throw new InvalidOperationException($"Unable to select constructor for type {type.FullName}.");
        }

        /// <summary>
        /// Selects default constructor
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be built</param>
        /// <param name="members">All public constructors this type implements</param>
        /// <returns></returns>
        public virtual object? LegacySelector(Type type, ConstructorInfo[] members)
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
                        return new InvalidRegistrationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "The type {0} has multiple constructors of length {1}. Unable to disambiguate.",
                                type.GetTypeInfo().Name,
                                paramLength));
                    }
                    return members[0];
            }
        }

        protected virtual object? SmartSelector(Type type, ConstructorInfo[] constructors)
        {
            Array.Sort(constructors, (a, b) =>
            {
                var qtd = b.GetParameters().Length.CompareTo(a.GetParameters().Length);

                if (qtd == 0)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    return b.GetParameters().Sum(p => p.ParameterType.GetTypeInfo().IsInterface ? 1 : 0)
                        .CompareTo(a.GetParameters().Sum(p => p.ParameterType.GetTypeInfo().IsInterface ? 1 : 0));
#else
                    return b.GetParameters().Sum(p => p.ParameterType.IsInterface ? 1 : 0)
                        .CompareTo(a.GetParameters().Sum(p => p.ParameterType.IsInterface ? 1 : 0));
#endif
                }
                return qtd;
            });

            foreach (var ctorInfo in constructors)
            {
                var parameters = ctorInfo.GetParameters();
#if NET40
                if (parameters.All(p => (null != p.DefaultValue && !(p.DefaultValue is DBNull)) || CanResolve(p)))
#else
                if (parameters.All(p => p.HasDefaultValue || CanResolve(p)))
#endif
                {
                    return ctorInfo;
                }
            }

            return new InvalidRegistrationException(
                $"Failed to select a constructor for {type.FullName}");
        }

    }
}
