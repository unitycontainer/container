namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that used to provide
    /// Per-Resolve lifetime management.
    /// </summary>
    internal class PerResolveOverride : DependencyOverride
    {
        public PerResolveOverride(in Contract contract, object? value)
            : base(contract.Type, contract.Name, value)
        {
        }
    }
}
