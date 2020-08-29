using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Container;

namespace Resolution.Contexts
{
    [TestClass]
    public class ResolutionContextTests
    {
        [TestMethod]
        public void Baseline() 
        {
            ResolutionContext context = default;

            Assert.IsNull(context.Name);
            Assert.IsFalse(context.IsChild);
        }

        [TestMethod]
        public void NameCtor()
        {
            var id = "test";
            var context = new ResolutionContext(id);

            Assert.AreEqual(id, context.Name);
            Assert.IsFalse(context.IsChild);
        }

        [TestMethod]
        public void ParentCtor()
        {
            var id = "test";
            var parent = new ResolutionContext(id);
            var child  = new ResolutionContext(ref parent);

            Assert.IsFalse(parent.IsChild);
            Assert.IsTrue(child.IsChild);

            ref var refParent = ref parent;
            ref var refChild  = ref child;

            //Assert.IsTrue(ReferenceEquals(refParent, refChild));
            //Assert.AreSame(parent, child.Parent);
            Assert.AreSame(parent.Name, child.Parent.Name);
            Assert.AreEqual(parent.Id,   child.Parent.Id);
        }
    }
}
