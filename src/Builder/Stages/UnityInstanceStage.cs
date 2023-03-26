namespace Unity.Builder
{
    /// <summary>
    /// Enumeration to represent the object composition stages.
    /// </summary>
    /// <remarks>
    /// The order of steps in this enumeration is the order in which the stages are run
    /// </remarks>
    public enum UnityInstanceStage : int
    {
        /// <summary>
        /// First stage. By default, nothing happens here.
        /// </summary>
        Setup = 0,

        /// <summary>
        /// Verification and diagnostic step.
        /// </summary>
        Diagnostic,

        /// <summary>
        /// Lifetime managers used to be checked here. 
        /// </summary>
        Lifetime,


        PreInstance,

        /// <summary>
        /// Stage where instances are intercepted
        /// </summary>
        InterceptInstance,


        PostInstance,


        PreType,

        /// <summary>
        /// Stage where instances are intercepted
        /// </summary>
        InterceptType,


        PostType,


        /// <summary>
        /// Type interception step.
        /// </summary>
        TypeMapping,

        /// <summary>
        /// Strategy in this stage run before creation.
        /// Strategy would later use.
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
        /// Step before fields are initialized.
        /// </summary>
        PreFields,

        /// <summary>
        /// Strategy in this stage initialize fields.
        /// </summary>
        Fields,

        /// <summary>
        /// Step after fields are initialized.
        /// </summary>
        PostFields,

        /// <summary>
        /// Step before properties are initialized.
        /// </summary>
        PreProperties,

        /// <summary>
        /// Strategy in this stage initialize properties.
        /// </summary>
        Properties,

        /// <summary>
        /// Step after properties are initialized.
        /// </summary>
        PostProperties,

        /// <summary>
        /// Step before methods are invoked.
        /// </summary>
        PreMethods,

        /// <summary>
        /// Strategy in this stage invoke methods.
        /// </summary>
        Methods,

        /// <summary>
        /// Step after methods are invoked.
        /// </summary>
        PostMethods,

        /// <summary>
        /// Step before additional initialization is performed.
        /// </summary>
        PreInitialization,

        /// <summary>
        /// Strategy in this stage do additional initialization.
        /// </summary>
        Initialization,

        /// <summary>
        /// Strategy in this stage work on objects that are already initialized. Typical work done in
        /// this stage might include looking to see if the object implements some notification interface
        /// to discover when its initialization stage has been completed.
        /// </summary>
        PostInitialization,

        /// <summary>
        /// The process is complete. By default, nothing happens here.
        /// </summary>
        Complete
    }
}
