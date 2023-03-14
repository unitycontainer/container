namespace Unity.Resolution;



/// <summary>
/// A strategy that is used at build plan execution time
/// to resolve a dependent value.
/// </summary>
public interface IDependencyResolverPolicy
{
    /// <summary>
    /// Get the value for a dependency.
    /// </summary>
    /// <param name="context">Current build context.</param>
    /// <returns>The value for the dependency.</returns>
    object Resolve(ref IBuilderContext context);
}
