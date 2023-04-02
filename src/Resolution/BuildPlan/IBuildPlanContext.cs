namespace Unity.Resolution;



/// <summary>
/// Represents the context in which a resolver factory runs.
/// </summary>
public interface IBuildPlanContext
{
    /// <summary>The owner container.</summary>
    UnityContainer Container { get; }

    /// <summary>
    /// The <see cref="Type"/> of the built object
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Registration associated with current resolution
    /// </summary>
    RegistrationManager? Registration { get; }

    /// <summary>
    /// Returns error condition
    /// </summary>
    bool IsFaulted { get; }

    /// <summary>
    /// Report error condition. This condition will be reported when exception
    /// <see cref="ResolutionFailedException"/> is thrown at the end of resolution.
    /// </summary>
    /// <param name="error">Error message</param>
    /// <returns><see cref="RegistrationManager.InvalidValue"/> object</returns>
    void Error(string error);
}

/// <summary>
/// Represents the context in which a resolver factory runs.
/// </summary>
public interface IBuildPlanContext<TTarget> : IBuildPlanContext
{
    /// <summary>
    /// The current resolver being built up.
    /// </summary>
    /// <value>
    /// The current object being manipulated by the build operation. May
    /// be null if the object hasn't been created yet.</value>
    TTarget? Target { get; set; }
}
