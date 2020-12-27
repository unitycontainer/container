using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(IPolicies policies)
            : base(policies)
        {
        }

        #endregion
    }
}
