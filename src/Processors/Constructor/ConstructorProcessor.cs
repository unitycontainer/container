using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : ParametersProcessor<ConstructorInfo>
    {
        #region Constructors

        public ConstructorProcessor(IPolicySet policySet, UnityContainer container)
            : base(policySet, typeof(InjectionConstructorAttribute), container)
        {
            SelectMethod = SmartSelector;
        }

        #endregion


        #region Public Properties

        public Func<Type, ConstructorInfo[], object> SelectMethod { get; set; }

        #endregion


        #region Overrides

        public override IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            // Select Injected Members
            if (null != ((InternalRegistration)registration).InjectionMembers)
            {
                foreach (var injectionMember in ((InternalRegistration)registration).InjectionMembers)
                {
                    if (injectionMember is InjectionMember<ConstructorInfo, object[]>)
                    {
                        return new[] { injectionMember };
                    }
                }
            }

            // Enumerate to array
            var constructors = DeclaredMembers(type).ToArray();
            if (1 >= constructors.Length)
                return constructors;

            // Select Attributed constructors
            foreach (var constructor in constructors)
            {
                for (var i = 0; i < AttributeFactories.Length; i++)
                {
#if NET40
                    if (!constructor.IsDefined(AttributeFactories[i].Type, true))
#else
                    if (!constructor.IsDefined(AttributeFactories[i].Type))
#endif
                        continue;

                    return new[] { constructor };
                }
            }

            // Select default
            return new[] { SelectMethod(type, constructors) };
        }

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(Type type)
        {
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(ctor => !ctor.IsFamily && !ctor.IsPrivate && !ctor.IsStatic);
        }

        #endregion


        #region Implementation                                               

        /// <summary>
        /// Selects default constructor
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be built</param>
        /// <param name="members">All public constructors this type implements</param>
        /// <returns></returns>
        public object LegacySelector(Type type, ConstructorInfo[] members)
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

        protected virtual object SmartSelector(Type type, ConstructorInfo[] constructors)
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

            return new InvalidOperationException(
                $"Failed to select a constructor for {type.FullName}", new InvalidRegistrationException());
        }

        #endregion
    }
}
