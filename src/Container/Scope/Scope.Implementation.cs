

namespace Unity.Container
{
    public abstract partial class Scope
    {
        #region Registration

        protected abstract void RegisterAnonymous(in RegistrationData data);

        protected abstract void RegisterContracts(in RegistrationData data);

        #endregion


        #region Implementation

        protected int FactorLoad(int required) => (int)(required / LoadFactor);
        
        #endregion
    }
}
