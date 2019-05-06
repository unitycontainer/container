using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Pipeline;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region BuilderContext

        internal BuilderContext.ResolvePlanDelegate ContextResolvePlan { get; set; } =
            (ref BuilderContext context, ResolveDelegate<BuilderContext> resolver) => resolver(ref context);

        private static object ContextValidatingResolvePlan(ref BuilderContext thisContext, ResolveDelegate<BuilderContext> resolver)
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
                    if (thisContext.Type == parentRef.Type && thisContext.Name == parentRef.Name)
                        throw new InvalidOperationException($"Circular reference for Type: {thisContext.Type}, Name: {thisContext.Name}",
                            new CircularDependencyException());

                    parent = parentRef.Parent;
                }

                var context = new BuilderContext
                {
                    ContainerContext = thisContext.ContainerContext,
                    Registration = thisContext.Registration,
                    Name = thisContext.Name,
                    Type = thisContext.Type,
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


        #region Build Plans

        private static ResolveDelegate<BuilderContext> ExecutePipeline { get; set; } =
            (ref BuilderContext context) =>
            {
                try
                {
                    if (null == context.Registration.Pipeline)
                    {
                        // TODO: Add timeout
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
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException)
                {
                    throw new ResolutionFailedException(context.Type, context.Name,
                        "For more information add Diagnostic extension: Container.AddExtension(new Diagnostic())", ex);
                }
            };

        private static object? ExecuteValidatingPipeline(ref BuilderContext context)
        {
            try
            {
                if (null == context.Registration.Pipeline)
                {
                    // TODO: Add timeout
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
                var builder = new StringBuilder();
                builder.AppendLine(ex.Message);
                builder.AppendLine("_____________________________________________________");
                builder.AppendLine("Exception occurred while:");

                foreach (DictionaryEntry item in ex.Data)
                    builder.AppendLine(CreateErrorMessage(item.Value));

                var message = builder.ToString();

                throw new ResolutionFailedException(context.Type, context.Name, message, ex);
            }


            string CreateErrorMessage(object value)
            {
                switch (value)
                {
                    case ParameterInfo parameter:
                        return $"    for parameter:  {parameter.Name}";

                    case ConstructorInfo constructor:
                        var ctorSignature = string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        return $"   on constructor:  {constructor.DeclaringType.Name}({ctorSignature})";

                    case MethodInfo method:
                        var methodSignature = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        return $"        on method:  {method.Name}({methodSignature})";

                    case PropertyInfo property:
                        return $"    for property:   {property.Name}";

                    case FieldInfo field:
                        return $"       for field:   {field.Name}";

                    case Type type:
                        return $"\n• while resolving:  {type.Name}";

                    case Tuple<Type, string> tuple:
                        return $"\n• while resolving:  {tuple.Item1.Name} registered with name: {tuple.Item2}";

                    case Tuple<Type, Type> tuple:
                        return $"        mapped to:  {tuple.Item1?.Name}";
                }

                return value.ToString();
            }
        }

        #endregion
    }
}
