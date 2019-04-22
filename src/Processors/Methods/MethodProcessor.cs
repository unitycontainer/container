using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Processors
{
    public class MethodProcessor : ParametersProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(DefaultPolicies defaults, UnityContainer container)
            : base(defaults, typeof(InjectionMethodAttribute), container)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
            return type.GetDeclaredMethods()
                       .Where(member => !member.IsFamily && 
                                        !member.IsPrivate && 
                                        !member.IsStatic);
        }

        public override IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != ((ImplicitRegistration)registration).InjectionMembers)
            {
                foreach (var injectionMember in ((ImplicitRegistration)registration).InjectionMembers)
                {
                    if (injectionMember is InjectionMember<MethodInfo, object[]> && memberSet.Add(injectionMember))
                        yield return injectionMember;
                }
            }

            // Select Attributed members
            IEnumerable<MethodInfo> members = DeclaredMembers(type);

            if (null == members) yield break;
            foreach (var member in members)
            {
                foreach (var attribute in Markers)
                {
#if NET40
                    if (!member.IsDefined(attribute, true) ||
#else
                    if (!member.IsDefined(attribute) ||
#endif
                        !memberSet.Add(member)) continue;

                    yield return member;
                    break;
                }
            }
        }

        public override ISelect<MethodInfo> GetOrDefault(IPolicySet registration) => 
            registration.Get<ISelect<MethodInfo>>() ?? Defaults.MethodsSelector;

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(MethodInfo info, object resolvers)
        {
            return Expression.Call(
                Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                info, CreateParameterExpressions(info.GetParameters(), resolvers));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object resolvers)
        {
            var parameterResolvers = CreateParameterResolvers(info.GetParameters(), resolvers).ToArray();
            return (ref BuilderContext c) =>
            {
                if (null == c.Existing) return c.Existing;

                var parameters = new object[parameterResolvers.Length];
                for (var i = 0; i < parameters.Length; i++)
                    parameters[i] = parameterResolvers[i](ref c);

                info.Invoke(c.Existing, parameters);

                return c.Existing;
            };
        }

        #endregion
    }
}
