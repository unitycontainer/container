using Unity.Extension;

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
    /// It is quite abvious that it takes significant amount of time during execution. So, 
    /// to optimize performance all these verifications where mooved to the Diagnostic
    /// extension. It is recommended to include this extension only during 
    /// development cycle and refrain from executing it in production 
    /// environment.
    /// </para>
    /// <para>
    /// This extenson can be registered in two ways: by adding an extention or by calling
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
        }
    }
}
