
namespace Unity.Tests.v5.ContainerRegistration
{
    internal class AnotherTypeImplementation : ITypeAnotherInterface
    {
        private readonly string name;

        public AnotherTypeImplementation()
        {
        }

        public AnotherTypeImplementation(string name)
        {
            this.name = name;
        }

        #region ITypeAnotherInterface Members

        public string GetName()
        {
            return name;
        }

        #endregion
    }
}
