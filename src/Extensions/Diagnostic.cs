using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Extension;
using Unity.Factories;
using Unity.Policy;

namespace Unity
{
    /// <summary>
    /// Diagnostic extension implements validating when calling <see cref="IUnityContainer.RegisterType"/>, 
    /// <see cref="IUnityContainer.Resolve"/>, and <see cref="IUnityContainer.BuildUp"/> methods. When executed 
    /// these methods provide extra layer of verification and validation as well 
    /// as more detailed reporting of error conditions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unity uses reflection to gather information about types, members, and parameters. 
    /// It is quite obvious that it takes significant amount of time during execution. So, 
    /// to optimize performance all these verifications where moved to the Diagnostic
    /// extension. It is recommended to include this extension only during 
    /// development cycle and refrain from executing it in production 
    /// environment.
    /// </para>
    /// <para>
    /// This extension can be registered in two ways: by adding an extension or by calling
    /// <c>EnableDiagnostic()</c> extension method on container. 
    /// Adding extension to container will work in any build, where <c>EnableDiagnostic()</c>
    /// will only enable it in DEBUG mode. 
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    ///     var container = new UnityContainer();
    /// #if DEBUG
    ///     container.AddExtension(new Diagnostic());
    /// #endif
    /// </code>
    /// <code>
    /// var container = new UnityContainer();
    /// container.EnableDiagnostic();
    /// </code>
    /// </example>
    public class Diagnostic : UnityContainerExtension
    {
        protected override void Initialize()
        {
            ((UnityContainer)Container).SetDefaultPolicies = UnityContainer.SetDiagnosticPolicies;
            ((UnityContainer)Container).SetDefaultPolicies((UnityContainer)Container);

            EnumerableResolver.EnumerableMethod = typeof(EnumerableResolver).GetTypeInfo()
                                      .GetDeclaredMethod(nameof(EnumerableResolver.DiagnosticResolver));

            EnumerableResolver.EnumerableFactory = typeof(EnumerableResolver).GetTypeInfo()
                                      .GetDeclaredMethod(nameof(EnumerableResolver.DiagnosticResolverFactory));
        }
    }

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
