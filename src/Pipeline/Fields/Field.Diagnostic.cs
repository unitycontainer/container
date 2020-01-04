using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    public partial class FieldDiagnostic : FieldPipeline
    {
        #region Constructors

        public FieldDiagnostic(UnityContainer container) : base(container)
        {
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
                    if (injectionMember is InjectionMember<FieldInfo, object> && !memberSet.Add(injectionMember))
                    { 
                        return new[] { new InvalidRegistrationException($"Field injected more than once '{injectionMember}'") };
                    }
                }
            }

            // Select Attributed members
            foreach (var member in type.DeclaredFields())
            {
                if (!member.IsDefined(typeof(DependencyResolutionAttribute)) || !memberSet.Add(member))
                    continue;

                if (member.IsStatic)
                { 
                    return new [] { new InvalidRegistrationException(
                        $"Static field '{member.Name}' on type '{type?.FullName}' is marked for injection. Static fields cannot be injected") };
                }

                if (member.IsInitOnly)
                { 
                    return  new [] { new InvalidRegistrationException(
                        $"Readonly field '{member.Name}' on type '{type?.FullName}' is marked for injection. Readonly fields cannot be injected") };
                }

                if (member.IsPrivate)
                { 
                    return  new [] { new InvalidRegistrationException(
                        $"Private field '{member.Name}' on type '{type?.FullName}' is marked for injection. Private fields cannot be injected") };
                }

                if (member.IsFamily)
                { 
                    return  new [] { new InvalidRegistrationException(
                        $"Protected field '{member.Name}' on type '{type?.FullName}' is marked for injection. Protected fields cannot be injected") };
                }
            }

            return memberSet;
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(FieldInfo info)
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

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(FieldInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

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
