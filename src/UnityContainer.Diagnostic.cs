using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Default Policies

        internal void SetDiagnosticPolicies()
        {
            // Builders
            var lifetimeBuilder    = new LifetimeBuilder();
            var mappingBuilder     = new MappingBuilder();
            var factoryBuilder     = new FactoryBuilder();
            var constructorBuilder = new ConstructorDiagnostic(this);
            var fieldsBuilder      = new FieldDiagnostic(this);
            var propertiesBuilder  = new PropertyDiagnostic(this);
            var methodsBuilder     = new MethodDiagnostic(this);

            // Pipelines
            Context.TypePipeline = new StagedStrategyChain<PipelineBuilder, PipelineStage>
            {
                { lifetimeBuilder,    PipelineStage.Lifetime },
                { mappingBuilder,     PipelineStage.TypeMapping },
                { factoryBuilder,     PipelineStage.Factory },
                { constructorBuilder, PipelineStage.Creation },
                { fieldsBuilder,      PipelineStage.Fields },
                { propertiesBuilder,  PipelineStage.Properties },
                { methodsBuilder,     PipelineStage.Methods }
            };

            Context.FactoryPipeline = new StagedStrategyChain<PipelineBuilder, PipelineStage>
            {
                { lifetimeBuilder, PipelineStage.Lifetime },
                { factoryBuilder,  PipelineStage.Factory }
            };

            Context.InstancePipeline = new StagedStrategyChain<PipelineBuilder, PipelineStage>
            {
                { lifetimeBuilder, PipelineStage.Lifetime },
                { factoryBuilder,  PipelineStage.Factory },
            };

            // Default policies
            Compose = ValidatingComposePlan;
            BuildPipeline = ValidatingPipeline;
            ContextResolvePlan = ContextValidatingResolvePlan;


            // Validators
            var validators = new PolicySet(this);

            validators.Set(typeof(Func<Type, InjectionMember, ConstructorInfo>), Validating.ConstructorSelector);
            validators.Set(typeof(Func<Type, InjectionMember, MethodInfo>),      Validating.MethodSelector);
            validators.Set(typeof(Func<Type, InjectionMember, FieldInfo>),       Validating.FieldSelector);
            validators.Set(typeof(Func<Type, InjectionMember, PropertyInfo>),    Validating.PropertySelector);

            _validators = validators;

            // Registration Validator
            TypeValidator = (typeFrom, typeTo) =>
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeFrom != null && !typeFrom.GetTypeInfo().IsGenericType && !typeTo.GetTypeInfo().IsGenericType && 
                                    !typeFrom.GetTypeInfo().IsAssignableFrom(typeTo.GetTypeInfo()))
#else
                if (typeFrom != null && !typeFrom.IsGenericType && !typeTo.IsGenericType &&
                    !typeFrom.IsAssignableFrom(typeTo))
#endif
                {
                    throw new ArgumentException($"The type {typeTo} cannot be assigned to variables of type {typeFrom}.");
                }
            };
        }

        #endregion

        
        #region Pipeline

        private object? ValidatingPipeline(ref BuilderContext context)
        {
            try
            {
                if (null == context.Registration.Pipeline)
                {
                    lock (context.Registration)
                    {
                        if (null == context.Registration.Pipeline)
                        {
                            PipelineContext builder = new PipelineContext(ref context);

                            context.Registration.Pipeline = builder.Pipeline() ?? DefaultResolver;
                        }
                    }
                }

                return context.Registration.Pipeline(ref context);
            }
            catch (Exception ex)
            {
                context.RequiresRecovery?.Recover();
                ex.Data.Add(Guid.NewGuid(), null == context.Name
                    ? context.RegistrationType == context.Type
                        ? (object)context.Type
                        : new Tuple<Type, Type>(context.RegistrationType, context.Type)
                    : context.RegistrationType == context.Type
                        ? (object)new Tuple<Type, string>(context.Type, context.Name)
                        : new Tuple<Type, Type, string>(context.RegistrationType, context.Type, context.Name));

                var builder = new StringBuilder();
                builder.AppendLine(ex.Message);
                builder.AppendLine("_____________________________________________________");
                builder.AppendLine("Exception occurred while:");
                builder.AppendLine();

                var indent = 0;
                foreach (DictionaryEntry item in ex.Data)
                {
                    for (var c = 0; c < indent; c++) builder.Append(" ");
                    builder.AppendLine(CreateErrorMessage(item.Value));
                    indent += 1;
                }

                var message = builder.ToString();

                throw new ResolutionFailedException(context.RegistrationType, context.Name, message, ex);
            }

            return context.Existing;


            string CreateErrorMessage(object value)
            {
                switch (value)
                {
                    case ParameterInfo parameter:
                        return $" for parameter:  '{parameter.Name}'";

                    case ConstructorInfo constructor:
                        var ctorSignature = string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        return $"on constructor:  {constructor.DeclaringType.Name}({ctorSignature})";

                    case MethodInfo method:
                        var methodSignature = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        return $"     on method:  {method.Name}({methodSignature})";

                    case PropertyInfo property:
                        return $"  for property:  '{property.Name}'";

                    case FieldInfo field:
                        return $"     for field:  '{field.Name}'";

                    case Type type:
                        return $"�resolving type:  '{type.Name}'";

                    case Tuple<Type, string> tuple:
                        return $"�resolving type:  '{tuple.Item1.Name}' registered with name: '{tuple.Item2}'";

                    case Tuple<Type, Type> tuple:
                        return $"�resolving type:  '{tuple.Item1?.Name}' mapped to '{tuple.Item2?.Name}'";

                    case Tuple<Type, Type, string> tuple:
                        return $"�resolving type:  '{tuple.Item1?.Name}' mapped to '{tuple.Item2?.Name}' and registered with name: '{tuple.Item3}'";
                }

                return value.ToString();
            }
        }

        #endregion


        #region BuilderContext

        internal BuilderContext.ResolvePlanDelegate ContextResolvePlan { get; set; } =
            (ref BuilderContext context, ResolveDelegate<BuilderContext> resolver) => resolver(ref context);

        internal static object ContextValidatingResolvePlan(ref BuilderContext thisContext, ResolveDelegate<BuilderContext> resolver)
        {
            if (null == resolver) throw new ArgumentNullException(nameof(resolver));

#if NET40
            return resolver(ref thisContext);
#else
            unsafe
            {
                var parent = thisContext.Parent;
                while (IntPtr.Zero != parent)
                {
                    var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                    if (thisContext.RegistrationType == parentRef.RegistrationType && thisContext.Name == parentRef.Name)
                        throw new InvalidOperationException($"Circular reference for Type: {thisContext.Type}, Name: {thisContext.Name}",
                            new CircularDependencyException());

                    parent = parentRef.Parent;
                }

                var context = new BuilderContext
                {
                    ContainerContext = thisContext.ContainerContext,
                    Registration = thisContext.Registration,
                    RegistrationType = thisContext.Type,
                    Name = thisContext.Name,
                    Type = thisContext.Type,
                    Compose = thisContext.Compose,
                    ResolvePlan = thisContext.ResolvePlan,
                    List = thisContext.List,
                    Overrides = thisContext.Overrides,
                    DeclaringType = thisContext.Type,
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
                };

                return resolver(ref context);
            }
#endif
        }

        #endregion
    }
}
