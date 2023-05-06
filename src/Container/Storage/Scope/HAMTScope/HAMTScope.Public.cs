namespace Unity.Storage
{
    public partial class HAMTScope
    {
        #region Adding Registrations

        /// <inheritdoc />
        public override void BuiltIn(Type type, RegistrationManager manager)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public override RegistrationManager? Register(Type type, string? name, RegistrationManager manager)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Get

        /// <inheritdoc />
        public override RegistrationManager? Get(in Contract contract)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public override RegistrationManager? GetBoundGeneric(in Contract contract, in Contract generic)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public override RegistrationManager GetCache(in Contract contract, Func<RegistrationManager> factory)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Contains

        /// <inheritdoc />
        public override bool Contains(Type type)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public override bool Contains(in Contract contract)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Child Scope

        /// <inheritdoc />
        public override Scope CreateChildScope(int capacity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
