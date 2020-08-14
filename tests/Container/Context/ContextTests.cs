using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using Unity.Container;

namespace Container.Contexts
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void StructSizes()
        {
            var size = Environment.Is64BitProcess 
                ? Unity.Container.Defaults.CONTEXT_STRUCT_SIZE
                : Unity.Container.Defaults.CONTEXT_STRUCT_SIZE / 2;

            Assert.AreEqual(size, Marshal.SizeOf(typeof(ResolveContext)));
            Assert.AreEqual(size, Marshal.SizeOf(typeof(PipelineContext)));
            Assert.AreEqual(Marshal.SizeOf(typeof(PipelineContext)), 
                            Marshal.SizeOf(typeof(ResolveContext)));
        }

        [TestMethod]
        public void DefaultValues()
        {
            ResolutionFrame context = default;

            Assert.IsNull(context.Resolve.Container);
            Assert.IsNull(context.Resolve.Manager);
            Assert.IsNull(context.Resolve.Existing);
            Assert.IsNull(context.Resolve.Overrides);
            Assert.IsNull(context.Resolve.Contract.Type);
            Assert.IsNull(context.Resolve.Contract.Name);
            Assert.AreEqual(0, context.Resolve.Contract.HashCode);

            Assert.IsNull(context.Pipeline.Container);
            Assert.IsNull(context.Pipeline.Manager);
            Assert.IsNull(context.Pipeline.Existing);
            Assert.IsNull(context.Pipeline.Overrides);
            Assert.IsNull(context.Pipeline.Contract.Type);
            Assert.IsNull(context.Pipeline.Contract.Name);
            Assert.AreEqual(0, context.Pipeline.Contract.HashCode);
        }

    }
}
