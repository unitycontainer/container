using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Builder;
using Unity.Composition;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        // TODO: Remove
        internal delegate CompositionDelegate CompositionFactoryDelegate(ref CompositionContext context);
        private delegate object ComposeObjectDelegate(ref BuilderContext context);



        #region Object Composition

        private ResolveDelegate<BuilderContext> Compose { get; set; } =
            (ref BuilderContext context) =>
            {
                // Create Pipeline if required
                if (null == context.Registration.Pipeline)
                {
                    // Double Check Lock
                    lock (context.Registration)
                    {
                        // Make sure build plan was not yet created
                        if (null == context.Registration.Pipeline)
                        {
                            context.Registration.Pipeline = ((UnityContainer)context.Container).ComposerFactory(ref context);
                        }
                    }
                }

                try
                {
                    return context.Registration.Pipeline(ref context);
                }
                catch (Exception ex)
                {
                    context.RequiresRecovery?.Recover();

                    throw new ResolutionFailedException(context.RegistrationType, context.Name,
                        "For more information add Diagnostic extension: Container.AddExtension(new Diagnostic())", ex);
                }
            };

        private object? ValidatingComposePlan(ref BuilderContext context)
        {
            try
            {
#pragma warning disable CS8602 // Possible dereference of a null reference.
                return context.Registration.Pipeline(ref context);
#pragma warning restore CS8602 // Possible dereference of a null reference.
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
                        return $"·resolving type:  '{type.Name}'";

                    case Tuple<Type, string> tuple:
                        return $"•resolving type:  '{tuple.Item1.Name}' registered with name: '{tuple.Item2}'";

                    case Tuple<Type, Type> tuple:
                        return $"•resolving type:  '{tuple.Item1?.Name}' mapped to '{tuple.Item2?.Name}'";

                    case Tuple<Type, Type, string> tuple:
                        return $"•resolving type:  '{tuple.Item1?.Name}' mapped to '{tuple.Item2?.Name}' and registered with name: '{tuple.Item3}'";
                }

                return value.ToString();
            }
        }

        #endregion


        #region Composition Plan

        internal ResolveDelegateFactory ComposerFactory = (ref BuilderContext context) =>
        {
            PipelineContext builder = new PipelineContext(ref context);

            return builder.Pipeline();
        };


        internal ResolveDelegate<BuilderContext> CompilingComposition(ref BuilderContext context)
        {
            throw new NotImplementedException();
        }

        internal ResolveDelegate<BuilderContext> ResolvingComposition(ref BuilderContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
