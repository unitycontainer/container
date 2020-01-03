using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Processors
{
    public class FieldDiagnostic : FieldProcessor
    {
        #region Selection

        protected override object Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    if (injectionMember is InjectionMember<FieldInfo, object> && !memberSet.Add(injectionMember))
                        throw new InvalidOperationException($"Field injected more than once '{injectionMember}'");
                }
            }

            // Select Attributed members
            foreach (var member in type.DeclaredFields())
            {
                if (!member.IsDefined(typeof(DependencyResolutionAttribute)) || !memberSet.Add(member))
                    continue;

                if (member.IsStatic)
                    throw new InvalidOperationException(
                        $"Static field '{member.Name}' on type '{type?.Name}' is marked for injection. Static fields cannot be injected");

                if (member.IsInitOnly)
                    throw new InvalidOperationException(
                        $"Readonly field '{member.Name}' on type '{type?.Name}' is marked for injection. Readonly fields cannot be injected");

                if (member.IsPrivate)
                    throw new InvalidOperationException(
                        $"Private field '{member.Name}' on type '{type?.Name}' is marked for injection. Private fields cannot be injected");

                if (member.IsFamily)
                    throw new InvalidOperationException(
                        $"Protected field '{member.Name}' on type '{type?.Name}' is marked for injection. Protected fields cannot be injected");
            }

            return memberSet;
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<BuilderContext>(info);

            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.ResolveDiagnostic(info, attribute.Name, resolver));
                return context.Existing;
            };
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<BuilderContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return (ref BuilderContext context) =>
                {
                    info.SetValue(context.Existing, context.OverrideDiagnostic(info, attribute.Name, data));
                    return context.Existing;
                };
            }
            else
            {
                return (ref BuilderContext context) =>
                {
                    info.SetValue(context.Existing, context.ResolveDiagnostic(info, attribute.Name, resolver));
                    return context.Existing;
                };
            }
        }

        #endregion
    }
}
