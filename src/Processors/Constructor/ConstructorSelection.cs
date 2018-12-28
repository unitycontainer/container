using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : MethodBaseInfoProcessor<ConstructorInfo>
    {
        #region Overrides

        protected override ConstructorInfo[] DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic)
                       .ToArray();
#else
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                       .ToArray();
#endif
        }

        protected override IEnumerable<object> SelectMembers(Type type, ConstructorInfo[] members, InjectionMember[] injectors)
        {
            // Select Injected Members
            if (null != injectors)
            {
                foreach (var injectionMember in injectors)
                {
                    if (injectionMember is InjectionMember<ConstructorInfo, object[]>)
                    {
                        yield return injectionMember;
                        yield break;
                    }
                }
            }

            // Select Attributed members
            if (null != members)
            {
                foreach (var member in members)
                {
                    foreach (var pair in ResolverFactories)
                    {
                        if (!member.IsDefined(pair.type)) continue;

                        yield return member;
                        yield break;
                    }
                }
            }

            // Select default
            yield return GetDefault(type, members);
        }

        /// <summary>
        /// Selects default constructor
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be built</param>
        /// <param name="members">All public constructors this type implements</param>
        /// <returns></returns>
        protected override object GetDefault(Type type, ConstructorInfo[] members)
        {
            Array.Sort(members, ConstructorComparer);

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
                                Constants.AmbiguousInjectionConstructor,
                                type.GetTypeInfo().Name,
                                paramLength));
                    }
                    return members[0];
            }
        }

        #endregion
    }
}
