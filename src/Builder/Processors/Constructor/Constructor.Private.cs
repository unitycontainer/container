using System.Reflection;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        #region Implementation

        protected static bool CanResolve(UnityContainer container, ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute<DependencyResolutionAttribute>();
            return attribute is null
                ? container.CanResolve(info.ParameterType, null)
                : container.CanResolve(attribute.ContractType ?? info.ParameterType,
                                       attribute.ContractName);
        }

        #endregion
    }
}
