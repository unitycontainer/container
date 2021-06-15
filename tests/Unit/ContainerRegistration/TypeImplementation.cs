namespace Unity.Tests.v5.ContainerRegistration
{
    internal class TypeImplementation : ITypeInterface
    {
        private string name;

        public TypeImplementation()
        {
        }

        public TypeImplementation(string name)
        {
            this.name = name;
        }

        #region ITypeInterface Members

        public string GetName()
        {
            return name;
        }

        #endregion
    }
}
