using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Processors
{
    public class PropertyDiagnostic : PropertyProcessor
    {
        #region Selection

        protected override object Select(Type type, InternalRegistration registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != registration.InjectionMembers)
            {
                foreach (var injectionMember in registration.InjectionMembers)
                {
                    if (injectionMember is InjectionMember<PropertyInfo, object> && !memberSet.Add(injectionMember))
                        throw new InvalidOperationException($"Property injected more than once '{injectionMember}'");
                }
            }

            // Select Attributed members
            foreach (var member in type.DeclaredProperties())
            {
                if (!member.IsDefined(typeof(DependencyResolutionAttribute)) || !memberSet.Add(member))
                    continue;

                if (!member.CanWrite)
                    throw new InvalidOperationException(
                        $"Readonly property '{member.Name}' on type '{type?.FullName}' is marked for injection. Readonly properties cannot be injected");

                if (0 != member.GetIndexParameters().Length)
                    throw new InvalidOperationException(
                        $"Indexer '{member.Name}' on type '{type?.FullName}' is marked for injection. Indexers cannot be injected");

                var setter = member.GetSetMethod(true);

                if (null == setter)
                    throw new InvalidOperationException(
                        $"Readonly property '{member.Name}' on type '{type?.FullName}' is marked for injection. Static properties cannot be injected");

                if (setter.IsStatic)
                    throw new InvalidOperationException(
                        $"Static property '{member.Name}' on type '{type?.FullName}' is marked for injection. Static properties cannot be injected");

                if (setter.IsPrivate)
                    throw new InvalidOperationException(
                        $"Private property '{member.Name}' on type '{type?.FullName}' is marked for injection. Private properties cannot be injected");

                if (setter.IsFamily)
                    throw new InvalidOperationException(
                        $"Protected property '{member.Name}' on type '{type?.FullName}' is marked for injection. Protected properties cannot be injected");
            }

            return memberSet;
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info)
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

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info, object? data)
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
