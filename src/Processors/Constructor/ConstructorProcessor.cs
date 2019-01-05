using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : MethodBaseProcessor<ConstructorInfo>
    {
        #region Fields

        private readonly Func<Type, bool> _isTypeRegistered;
        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();

        #endregion


        #region Constructors

        public ConstructorProcessor(IPolicySet policySet, Func<Type, bool> isTypeRegistered)
            : base(policySet, typeof(InjectionConstructorAttribute))
        {
            _isTypeRegistered = isTypeRegistered;
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
                    if (!constructor.IsDefined(AttributeFactories[i].Type))
                        continue;

                    return new[] { constructor };
                }
            }

            // Select default
            return new[] { SelectMethod(type, constructors) };
        }

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic);
#else
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
#endif
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
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "The type {0} has multiple constructors of length {1}. Unable to disambiguate.",
                                type.GetTypeInfo().Name,
                                paramLength));
                    }
                    return members[0];
            }
        }

        private object SmartSelector(Type type, ConstructorInfo[] constructors)
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

            int parametersCount = 0;
            ConstructorInfo bestCtor = null;
            HashSet<Type> bestCtorParameters = null;

            foreach (var ctorInfo in constructors)
            {
                var parameters = ctorInfo.GetParameters();

                if (null != bestCtor && parametersCount > parameters.Length) return bestCtor;
                parametersCount = parameters.Length;
#if NET40
                if (parameters.All(p => _container.CanResolve(p.ParameterType) || null != p.DefaultValue && !(p.DefaultValue is DBNull)))
#else
                if (parameters.All(p => p.HasDefaultValue || CanResolve(p.ParameterType)))
#endif
                {
                    if (bestCtor == null)
                    {
                        bestCtor = ctorInfo;
                    }
                    else
                    {
                        // Since we're visiting constructors in decreasing order of number of parameters,
                        // we'll only see ambiguities or supersets once we've seen a 'bestConstructor'.

                        if (null == bestCtorParameters)
                        {
                            bestCtorParameters = new HashSet<Type>(
                                bestCtor.GetParameters().Select(p => p.ParameterType));
                        }

                        if (!bestCtorParameters.IsSupersetOf(parameters.Select(p => p.ParameterType)))
                        {
                            if (bestCtorParameters.All(p => p.GetTypeInfo().IsInterface) &&
                                !parameters.All(p => p.ParameterType.GetTypeInfo().IsInterface))
                                return bestCtor;

                            throw new InvalidOperationException($"Failed to select a constructor for {type.FullName}");
                        }

                        return bestCtor;
                    }
                }
            }

            if (bestCtor == null)
            {
                //return null;
                throw new InvalidOperationException(
                    $"Builder not found for { type.FullName}");
            }

            return bestCtor;
        }

        private bool CanResolve(Type type)
        {
            var info = type.GetTypeInfo();

            if (info.IsClass)
            {
                if (DelegateType.IsAssignableFrom(info) ||
                    typeof(string) == type || info.IsEnum || info.IsPrimitive || info.IsAbstract)
                {
                    return _isTypeRegistered(type);
                }

                if (type.IsArray)
                {
                    return _isTypeRegistered(type) || CanResolve(type.GetElementType());
                }

                return true;
            }

            if (info.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(IEnumerable<>) || _isTypeRegistered(genericType))
                {
                    return true;
                }
            }

            return _isTypeRegistered(type);
        }

        #endregion
    }
}
