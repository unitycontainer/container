using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Container;
using Unity.Extension;

namespace Pipeline
{
    public partial class Builder
    {
        [TestMethod("Empty chain"), TestProperty(TEST, ANALYSIS)]
        public void Analysis_FromEmpty()
        {
        }
    }
}
