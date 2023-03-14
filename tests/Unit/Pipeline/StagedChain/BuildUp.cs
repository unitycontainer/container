using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Extension;
using Unity.Resolution;

namespace Pipeline
{
    public partial class StagedChain
    {
        [TestMethod("Empty chain"), TestProperty(TEST, BUILDUP)]
        public void BuildUp_FromEmpty()
        {
            // Act
            var visitor = Chain.BuildUpPipeline<FakeContext>();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(0, Context.Count);
        }

        [TestMethod("No overridden methods"), TestProperty(TEST, BUILDUP)]
        public void BuildUp_NoStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new NoStrategy());

            // Act
            var visitor = Chain.BuildUpPipeline<FakeContext>();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(0, Context.Count);
        }

        [TestMethod("Strategy with overridden PreBuildUp"), TestProperty(TEST, BUILDUP)]
        public void BuildUp_PreBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PreBuildUpStrategy());

            // Act
            var visitor = Chain.BuildUpPipeline<FakeContext>();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(1, Context.Count);
        }

        [TestMethod("Strategy with overridden PostBuildUp"), TestProperty(TEST, BUILDUP)]
        public void BuildUp_PostBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PostBuildUpStrategy());

            // Act
            var visitor = Chain.BuildUpPipeline<FakeContext>();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(1, Context.Count);
        }

        [TestMethod("Strategy with both methods overridden"), TestProperty(TEST, BUILDUP)]
        public void BuildUp_BothStrategies()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());

            // Act
            var visitor = Chain.BuildUpPipeline<FakeContext>();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(2, Context.Count);
        }

        [TestMethod("Multiple Strategies"), TestProperty(TEST, BUILDUP)]
        public void BuildUp_Multiple()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation,  new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation,     new BothStrategies());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var visitor = Chain.BuildUpPipeline<FakeContext>();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(4, Context.Count);

            var array = Context.Existing as IList<string>;
            Assert.IsNotNull(array);

            Assert.AreSame(PreBuildUpStrategy.PreName,   array[0]);
            Assert.AreSame(BothStrategies.PreName,       array[1]);
            Assert.AreSame(PostBuildUpStrategy.PostName, array[2]);
            Assert.AreSame(BothStrategies.PostName,      array[3]);
        }

        [TestMethod("Strategy with fault"), TestProperty(TEST, BUILDUP)]
        public void BuildUp_Faulted()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation,  new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation,     new FaultedStrategy());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var visitor = Chain.BuildUpPipeline<FakeContext>();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(2, Context.Count);

            var array = Context.Existing as IList<string>;
            Assert.IsNotNull(array);

            Assert.AreSame(PreBuildUpStrategy.PreName, array[0]);
            Assert.AreSame(FaultedStrategy.PreName,    array[1]);
        }
    }
}
