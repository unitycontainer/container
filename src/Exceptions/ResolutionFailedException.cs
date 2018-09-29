using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using Unity.Builder;

namespace Unity.Exceptions
{
    /// <summary>
    /// The exception thrown by the Unity container when
    /// an attempt to resolve a dependency fails.
    /// </summary>
    public partial class ResolutionFailedException : Exception
    {
        /// <summary>
        /// Create a new <see cref="ResolutionFailedException"/> that records
        /// the exception for the given type and name.
        /// </summary>
        /// <param name="typeRequested">Type requested from the container.</param>
        /// <param name="nameRequested">Name requested from the container.</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">The actual exception that caused the failure of the build.</param>
        public ResolutionFailedException(Type typeRequested, string nameRequested, string message, Exception innerException)
            : base(message, innerException)
        {
            TypeRequested = (typeRequested ?? throw new ArgumentNullException(nameof(typeRequested))).GetTypeInfo().Name;
            NameRequested = nameRequested;

            RegisterSerializationHandler();
        }

        /// <summary>
        /// Create a new <see cref="ResolutionFailedException"/> that records
        /// the exception for the given type and name.
        /// </summary>
        /// <param name="typeRequested">Type requested from the container.</param>
        /// <param name="nameRequested">Name requested from the container.</param>
        /// <param name="innerException">The actual exception that caused the failure of the build.</param>
        /// <param name="context">The build context representing the failed operation.</param>
        /// <param name="format">Custom format message</param>
        // TODO: TBuilderContext
        public ResolutionFailedException(Type typeRequested, string nameRequested, Exception innerException, 
                                         IBuilderContext context, string format = Constants.ResolutionFailed)
            : base(CreateMessage(typeRequested, nameRequested, innerException, context, format), innerException)
        {
            TypeRequested = (typeRequested ?? throw new ArgumentNullException(nameof(typeRequested))).GetTypeInfo().Name;
            NameRequested = nameRequested;

            RegisterSerializationHandler();
        }

        /// <summary>
        /// The type that was being requested from the container at the time of failure.
        /// </summary>
        public string TypeRequested { get; private set; }

        /// <summary>
        /// The name that was being requested from the container at the time of failure.
        /// </summary>
        public string NameRequested { get; private set; }

        partial void RegisterSerializationHandler();

        public static string CreateMessage(Type typeRequested, string nameRequested, Exception innerException, 
                                            IBuilderContext context, string format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(
                CultureInfo.CurrentCulture, format,
                typeRequested ?? throw new ArgumentNullException(nameof(typeRequested)),
                FormatName(nameRequested),
                ExceptionReason(ref context),
                innerException?.GetType().GetTypeInfo().Name ?? "ResolutionFailedException",
                innerException?.Message);
            builder.AppendLine();

            AddContextDetails(builder, context, 1);

            return builder.ToString();
        }

        private static void AddContextDetails(StringBuilder builder, IBuilderContext context, int depth)
        {
            if (context != null)
            {
                var indentation = new string(' ', depth * 2);
                var key = context.BuildKey;
                var originalKey = context.OriginalBuildKey;

                builder.Append(indentation);

                if (key == originalKey)
                {
                    builder.AppendFormat(
                        CultureInfo.CurrentCulture,
                        Constants.ResolutionTraceDetail,
                        key.Type, FormatName(key.Name));
                }
                else
                {
                    builder.AppendFormat(
                        CultureInfo.CurrentCulture,
                        Constants.ResolutionWithMappingTraceDetail,
                        key.Type, FormatName(key.Name),
                        originalKey.Type, FormatName(originalKey.Name));
                }

                builder.AppendLine();

                if (context.CurrentOperation != null)
                {
                    builder.Append(indentation);
                    builder.AppendFormat(
                        CultureInfo.CurrentCulture,
                        context.CurrentOperation.ToString());
                    builder.AppendLine();
                }

                AddContextDetails(builder, context.ChildContext, depth + 1);
            }
        }

        private static string FormatName(string name)
        {
            return string.IsNullOrEmpty(name) ? "(none)" : name;
        }

        private static string ExceptionReason<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            // TODO: where TBuilderContext : IBuilderContext
            //ref var deepestContext = ref context;
            //while (deepestContext.ChildContext != null)
            //{
            //    deepestContext = deepestContext.ChildContext;
            //}

            //if (deepestContext.CurrentOperation != null)
            //{
            //    return deepestContext.CurrentOperation.ToString();
            //}
            return Constants.NoOperationExceptionReason;
        }
    }
}
