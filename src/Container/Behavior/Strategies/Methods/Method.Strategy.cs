using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public partial class MethodStrategy : ParameterStrategy<MethodInfo>
    {
        #region Constructors

        public MethodStrategy(IPolicies policies)
            : base(policies)
        {
        }

        #endregion
    }
}
