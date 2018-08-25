using Unity.Extension;
using Unity.Policy;

namespace Unity.Tests.v5.Extension
{
    internal class MyCustomExtension : UnityContainerExtension, IMyCustomConfigurator
    {
        private bool checkExtensionAdded = false;
        private bool checkPolicyAdded = false;

        public bool CheckExtensionValue
        {
            get { return this.checkExtensionAdded; }
            set { this.checkExtensionAdded = value; }
        }

        public bool CheckPolicyValue
        {
            get { return this.checkPolicyAdded; }
            set { this.checkPolicyAdded = value; }
        }

        protected override void Initialize()
        {
            this.checkExtensionAdded = true;
            this.AddPolicy();
        }

        public IMyCustomConfigurator AddPolicy()
        {
            Context.Policies.Set(null, null, typeof(IBuildPlanPolicy), new MyCustomPolicy());
            this.checkPolicyAdded = true;
            return this;
        }
    }
}
