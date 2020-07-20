

namespace Unity.Container
{
    public abstract partial class Scope
    {
        #region Implementation

        protected int FactorLoad(int required) => (int)(required / LoadFactor);
        
        #endregion
    }
}
