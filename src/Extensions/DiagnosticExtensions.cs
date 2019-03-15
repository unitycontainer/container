using System;
using System.Diagnostics;

namespace Unity
{
    public static class DiagnosticExtensions
    {
        /// <summary>
        /// Enables diagnostic validations on the container built in DEBUG mode.
        /// </summary>
        /// <remarks>
        /// <para>This extension method adds <see cref="Diagnostic"/> extension to the 
        /// container and enables extended validation for all container's operations.</para>
        /// <para>This method will only work if the calling code is built with DEBUG
        /// symbol defined. In other word in you building in Debug mode. Conditional 
        /// methods can not return any values, so fluent notation can not be used with 
        /// this method.</para>
        /// </remarks>
        /// <example>
        /// This is how you could call this method to enable diagnostics:
        /// <code>
        /// var container = new UnityContainer();
        /// container.EnableDebugDiagnostic();
        /// ...
        /// </code>
        /// </example>
        /// <param name="container">The Unity Container instance</param>
        [Conditional("DEBUG")]
        public static void EnableDebugDiagnostic(this UnityContainer container)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));

            container.AddExtension(new Diagnostic());
        }

        /// <summary>
        /// Enables diagnostic validations on the container.
        /// </summary>
        /// <remarks>
        /// <para>This extension method adds <see cref="Diagnostic"/> extension to the 
        /// container and enables extended validation for all container's operations.</para>
        /// <para>This method works regardless of the build mode. In other word, it will 
        /// always enable validation. This method could be used with fluent notation.</para>
        /// </remarks>
        /// <example>
        /// This is how you could call this method to enable diagnostics:
        /// <code>
        /// var container = new UnityContainer().EnableDebugDiagnostic();
        /// ...
        /// </code>
        /// </example>
        /// <param name="container">The Unity Container instance</param>
        /// <returns></returns>
        public static UnityContainer EnableDiagnostic(this UnityContainer container)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));
               
            container.AddExtension(new Diagnostic());
            return container;
        }
    }
}
