using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer
    {
        #region Dynamic Registrations

        private IPolicySet GetDynamicRegistration(Type type, string name)
        {
            var registration = _get(type, name);
            if (null != registration) return registration;

            var info = type.GetTypeInfo();
            return !info.IsGenericType
                ? _root.GetOrAdd(type, name)
                : GetOrAddGeneric(type, name, info.GetGenericTypeDefinition());
        }

        private IPolicySet CreateRegistration(Type type, string name)
        {

            var registration = new InternalRegistration(type, name);

            if (type.GetTypeInfo().IsGenericType)
            {
                var factory = (InternalRegistration)_get(type.GetGenericTypeDefinition(), name);
                if (null != factory)
                {
                    registration.InjectionMembers = factory.InjectionMembers;
                    registration.Map = factory.Map;
                }
            }

            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        private IPolicySet CreateRegistration(Type type, Type policyInterface, object policy)
        {
            var registration = new InternalRegistration(policyInterface, policy);
            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        #endregion


        #region Resolving Collections

        internal static object ResolveEnumerable<TElement>(ref BuilderContext context)
        {
            var type = typeof(TElement);
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;
#else
            var generic = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
#endif
            var set = generic == type ? GetRegistrations((UnityContainer)context.Container, type)
                                      : GetRegistrations((UnityContainer)context.Container, type, generic);

            return ResolveRegistrations<TElement>(ref context, set);
        }

        internal static object ResolveArray<TElement>(ref BuilderContext context)
        {
            var type = typeof(TElement);
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;
#else
            var generic = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
#endif
            var set = generic == type ? GetNamedRegistrations((UnityContainer)context.Container, type)
                                      : GetNamedRegistrations((UnityContainer)context.Container, type, generic);
            return ResolveRegistrations<TElement>(ref context, set).ToArray();
        }

        private static IList<TElement> ResolveRegistrations<TElement>(ref BuilderContext context, RegistrationSet registrations)
        {
            var type = typeof(TElement);
            var list = new List<TElement>();
            for (var i = 0; i < registrations.Count; i++)
            {
                ref var entry = ref registrations[i];
                try
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    if (entry.RegisteredType.GetTypeInfo().IsGenericTypeDefinition)
#else
                    if (entry.RegisteredType.IsGenericTypeDefinition)
#endif
                        list.Add((TElement)context.Resolve(type, entry.Name));
                    else
                        list.Add((TElement)context.Resolve(type, entry.Name, entry.Registration));
                }
                catch (ArgumentException ex)
                {
                    if (!(ex.InnerException is TypeLoadException))
                        throw;
                    // TODO: Check if required and why
                }
            }

            return list;
        }

        #endregion


        #region Resolving Generic Collections

        internal static object ResolveGenericEnumerable<TElement>(ref BuilderContext context, Type type)
        {
            var set = GetRegistrations((UnityContainer)context.Container, typeof(TElement), type);
            return ResolveGenericRegistrations<TElement>(ref context, set);
        }

        internal static object ResolveGenericArray<TElement>(ref BuilderContext context, Type type)
        {
            var set = GetNamedRegistrations((UnityContainer)context.Container, typeof(TElement), type);
            return ResolveGenericRegistrations<TElement>(ref context, set).ToArray();
        }

        private static IList<TElement> ResolveGenericRegistrations<TElement>(ref BuilderContext context, RegistrationSet registrations)
        {
            var list = new List<TElement>();
            for (var i = 0; i < registrations.Count; i++)
            {
                ref var entry = ref registrations[i];
                try
                {
                    list.Add((TElement)context.Resolve(typeof(TElement), entry.Name));
                }
                catch (MakeGenericTypeFailedException) { /* Ignore */ }
                catch (InvalidOperationException ex)
                {
                    if (!(ex.InnerException is InvalidRegistrationException))
                        throw;
                    // TODO: Check if required and why
                }
            }

            return list;
        }

        #endregion


        #region Build Plan

        private ResolveDelegate<BuilderContext> GetCompiledResolver(ref BuilderContext context)
        {
            var expressions = new List<Expression>();

            foreach (var processor in _processorsChain)
            {
                foreach (var step in processor.GetBuildSteps(ref context))
                    expressions.Add(step);
            }

            expressions.Add(BuilderContextExpression.Existing);

            var lambda = Expression.Lambda<ResolveDelegate<BuilderContext>>(
                Expression.Block(expressions), BuilderContextExpression.Context);

            return lambda.Compile();
        }

        private ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context)
        {
            ResolveDelegate<BuilderContext> seed = null;
            foreach (var processor in _processorsChain)
            {
                seed = processor.GetResolver(ref context, seed);
            }

            return seed;
        }

        #endregion


        #region Exceptions

        public static string CreateMessage(Type typeRequested, string nameRequested, Exception innerException,
                                            ref BuilderContext context, string format = Constants.ResolutionFailed)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Resolution of the dependency failed for type = '{typeRequested}', name = '{FormatName(nameRequested)}'.");
            builder.AppendLine($"Exception occurred while: {Constants.NoOperationExceptionReason}.");
            builder.AppendLine($"Exception is: {innerException?.GetType().GetTypeInfo().Name ?? "ResolutionFailedException"} - {innerException?.Message}");
            builder.AppendLine("-----------------------------------------------");
            builder.AppendLine("At the time of the exception, the container was: ");

            AddContextDetails(builder, ref context, 1);

            var message = builder.ToString();
            return message;
        }

        private static void AddContextDetails(StringBuilder builder, ref BuilderContext context, int depth)
        {
            var indentation = new string(' ', depth * 2);

            builder.Append(indentation);

            builder.Append(context.Type == context.RegistrationType
                ? $"Resolving {context.Type},{FormatName(context.Name)}"
                : $"Resolving {context.Type},{FormatName(context.Name)} (mapped from {context.RegistrationType}, {FormatName(context.Name)})");

            builder.AppendLine();


            // TODO: 5.9.0 ReEnable
            //AddContextDetails(builder, context.ChildContext, depth + 1);
        }

        private static string FormatName(string name)
        {
            return string.IsNullOrEmpty(name) ? "(none)" : name;
        }

        #endregion

    }
}
