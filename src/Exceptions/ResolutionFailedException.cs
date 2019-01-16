using System;
using System.Reflection;

namespace Unity
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
        public ResolutionFailedException(Type typeRequested, string nameRequested, string message, Exception innerException = null)
            : base(message, innerException)
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
    }
}
