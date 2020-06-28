using Unity.Lifetime;

namespace Unity.Scope
{
    public partial class RegistrationScope
    {
        #region Fields

        protected RegistrationScope? _parent;

        #endregion


        #region Constructors

        internal RegistrationScope()
        {
        }

        protected RegistrationScope(RegistrationScope parent)
        {
            // Parent
            _parent = parent;
        }

        #endregion


        public virtual RegistrationScope CreateChildScope()
            => new RegistrationScope(this);
    }
}
