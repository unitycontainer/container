

namespace Unity
{
    /// <summary>
    /// Enumeration to represent the object composition stages.
    /// </summary>
    /// <remarks>
    /// <para>The order of the values in the enumeration is the order in which the stages are run.</para>
    /// </remarks>
    public enum PipelineStage
    {
        /// <summary>
        /// By default, nothing happens here.
        /// </summary>
        Setup,

        /// <summary>
        /// Verification and diagnostic step.
        /// </summary>
        Diagnostic,

        /// <summary>
        /// Lifetime managers are checked here. If value is available the rest of the pipeline is skipped.
        /// If request is async and no value stored in the manager new task is created and scheduled.
        /// </summary>
        Lifetime,

        /// <summary>
        /// At this point pipeline intercepts Instances.
        /// </summary>
        Instance,

        /// <summary>
        /// Type mapping and type conversion occurs here.
        /// </summary>
        TypeMapping,

        /// <summary>
        /// Stage where Resolver is located if present.
        /// </summary>
        Resolver,

        /// <summary>
        /// Stage where Resolver Factory is located if present.
        /// </summary>
        Factory,

        /// <summary>
        /// Strategies in this stage run before creation. Typical work done in this stage might
        /// include strategies that use reflection to set policies into the context that other
        /// strategies would later use.
        /// </summary>
        PreCreation,

        /// <summary>
        /// Objects are created at this stage.
        /// </summary>
        Creation,

        /// <summary>
        /// Object is created but not initialized.
        /// </summary>
        PostCreation,

        /// <summary>
        /// Strategies in this stage initialize fields.
        /// </summary>
        Fields,

        /// <summary>
        /// Strategies in this stage initialize properties.
        /// </summary>
        Properties,

        /// <summary>
        /// Strategies in this stage call required methods.
        /// </summary>
        Methods,

        /// <summary>
        /// Strategies in this stage do additional initialization.
        /// </summary>
        Initialization,

        /// <summary>
        /// Strategies in this stage work on objects that are already initialized. Typical work done in
        /// this stage might include looking to see if the object implements some notification interface
        /// to discover when its initialization stage has been completed.
        /// </summary>
        PostInitialization
    }
}
