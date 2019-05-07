using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Pipeline;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region BuilderContext

        internal BuilderContext.ResolvePlanDelegate BuilderContextPipeline { get; set; } =
            (ref BuilderContext context, ResolveDelegate<BuilderContext> resolver) => resolver(ref context);

        private static object BuilderContextValidatingPipeline(ref BuilderContext thisContext, ResolveDelegate<BuilderContext> resolver)
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
                    Type = thisContext.Type,
                    ResolvePipeline = thisContext.ResolvePipeline,
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


        #region Execution

        private static object? ComposePipeline(ref BuilderContext context)
        {
            bool locked = false;

            try
            {
                // We will wait reasonable time to acquire the lock
                Monitor.TryEnter(context.Registration, DefaultTimeOut, ref locked);

                // Create the pipeline
                PipelineContext builder = new PipelineContext(ref context);
                var pipeline = builder.Pipeline();

                // Check again, just in case
                if (null == context.Registration.Pipeline)
                    context.Registration.Pipeline = pipeline;
            }
            finally
            {
                if (locked) Monitor.Exit(context.Registration);
            }

            return ExecutePipeline(ref context);
        }

        private static ResolveDelegate<BuilderContext> ExecutePipeline { get; set; } =
            (ref BuilderContext context) =>
            {
                Debug.Assert(null != context.Registration.Pipeline);

                try
                {
                    return context.Registration.Pipeline(ref context);
                }
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException)
                {
                    throw new ResolutionFailedException(context.Type, context.Name,
                        $"Resolution failed with error: {ex.Message}\n\nFor more detailed information run Unity in debug mode: new UnityContainer(enableDiagnostic: true)", ex);
                }
            };

        #endregion


        #region Validating Execution

        private static object? ExecuteValidatingPipeline(ref BuilderContext context)
        {
            Debug.Assert(null != context.Registration.Pipeline);

            try
            {
                return context.Registration.Pipeline(ref context);
            }
            catch (Exception ex)
            {
                var message = CreateMessage(ex);
                throw new ResolutionFailedException(context.Type, context.Name, message, ex);
            }
        }

        #endregion


        #region Error Message

        private static string CreateMessage(Exception ex, string? message = null)
        {
            const string line = "_____________________________________________________";

            var builder = new StringBuilder();
            builder.AppendLine(ex.Message);
            if (null == message)
            {
                builder.AppendLine(line);
                builder.AppendLine("Exception occurred while:");
            }

            foreach (DictionaryEntry item in ex.Data)
                builder.AppendLine(DataToString(item.Value));

            if (null != message)
            {
                builder.AppendLine(line);
                builder.AppendLine(message);
            }

            return builder.ToString();
        }

        private static string DataToString(object value)
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

                case Tuple<Type, string?> tuple:
                    return $"\n• while resolving:  {tuple.Item1.Name} registered with name: {tuple.Item2}";

                case Tuple<Type, Type> tuple:
                    return $"        mapped to:  {tuple.Item1?.Name}";
            }

            return value.ToString();
        }

        #endregion
    }
}
