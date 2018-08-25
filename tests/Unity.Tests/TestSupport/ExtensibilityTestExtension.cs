
using Unity.Extension;

namespace Unity.Tests.v5.TestSupport
{
    public interface IConfigOne : IUnityContainerExtensionConfigurator
    {
        IConfigOne SetText(string text);
    }

    public interface IConfigTwo : IUnityContainerExtensionConfigurator
    {
        IConfigTwo SetMessage(string text);
    }

    public class ExtensibilityTestExtension : UnityContainerExtension, IConfigOne, IConfigTwo
    {
        public string ConfigOneText { get; private set; }
        public string ConfigTwoText { get; private set; }

        protected override void Initialize()
        {
        }

        public IConfigOne SetText(string text)
        {
            this.ConfigOneText = text;
            return this;
        }

        public IConfigTwo SetMessage(string text)
        {
            this.ConfigTwoText = text;
            return this;
        }
    }
}
