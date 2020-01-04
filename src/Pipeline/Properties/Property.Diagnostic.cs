using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    public partial class PropertyDiagnostic : PropertyPipeline
    {
        #region Constructors

        public PropertyDiagnostic(UnityContainer container) 
            : base(container)
        {
            container.Defaults.Set(typeof(Func<Type, InjectionMember, PropertyInfo>), InjectionValidatingSelector);
        }

        #endregion


        #region Overrides

        public override object Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    if (injectionMember is InjectionMember<PropertyInfo, object> && !memberSet.Add(injectionMember))
                    { 
                        return new[] { new InvalidRegistrationException($"Property injected more than once '{injectionMember}'") };
                    }
                }
            }

            // Select Attributed members
            foreach (var member in type.DeclaredProperties())
            {
                if (!member.IsDefined(typeof(DependencyResolutionAttribute)) || !memberSet.Add(member))
                    continue;

                if (!member.CanWrite)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Readonly property '{member.Name}' on type '{type?.FullName}' is marked for injection. Readonly properties cannot be injected") };
                }

                if (0 != member.GetIndexParameters().Length)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Indexer '{member.Name}' on type '{type?.FullName}' is marked for injection. Indexers cannot be injected") };
                }

                var setter = member.GetSetMethod(true);

                if (null == setter)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Readonly property '{member.Name}' on type '{type?.FullName}' is marked for injection. Static properties cannot be injected") };
                }

                if (setter.IsStatic)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Static property '{member.Name}' on type '{type?.FullName}' is marked for injection. Static properties cannot be injected") };
                }

                if (setter.IsPrivate)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Private property '{member.Name}' on type '{type?.FullName}' is marked for injection. Private properties cannot be injected") };
                }

                if (setter.IsFamily)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Protected property '{member.Name}' on type '{type?.FullName}' is marked for injection. Protected properties cannot be injected") };
                }
            }

            return memberSet;
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(PropertyInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return (ref PipelineContext context) =>
            {
                info.SetValue(context.Existing, context.ResolveDiagnostic(info, attribute.Name, resolver));
                return context.Existing;
            };
        }

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(PropertyInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return (ref PipelineContext context) =>
                {
                    info.SetValue(context.Existing, context.OverrideDiagnostic(info, attribute.Name, data));
                    return context.Existing;
                };
            }
            else
            {
                return (ref PipelineContext context) =>
                {
                    info.SetValue(context.Existing, context.ResolveDiagnostic(info, attribute.Name, resolver));
                    return context.Existing;
                };
            }
        }

        #endregion
    }
}
