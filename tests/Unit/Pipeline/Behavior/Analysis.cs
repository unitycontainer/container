//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using Unity.Builder;
//using Unity.Resolution;
//using Unity.Strategies;

//namespace Pipeline
//{
//    public partial class Behavior
//    {
//        [TestMethod("Empty chain"), TestProperty(TEST, ANALYSIS)]
//        public void Analysis_FromEmpty()
//        {
//            // Act
//            var visitor = Chain.AnalyzePipeline<FakeContext>();

//            // Validate
//            Assert.IsNotNull(visitor);
//            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

//            var result = visitor(ref Context) as object[];

//            Assert.IsNotNull(result);
//            Assert.AreEqual(1, result.Length);
//            Assert.AreEqual(Chain.Version, result[0]);
//        }

//        [TestMethod("No overridden methods"), TestProperty(TEST, ANALYSIS)]
//        public void Analysis_NoStrategy()
//        {
//            // Arrange
//            Chain.Add(UnityBuildStage.Creation, new NoStrategy());

//            // Act
//            var visitor = Chain.AnalyzePipeline<FakeContext>();

//            // Validate
//            Assert.IsNotNull(visitor);
//            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

//            var result = visitor(ref Context) as object[];

//            Assert.IsNotNull(result);
//            Assert.AreEqual(2, result.Length);
//            Assert.AreEqual(Chain.Version, result[1]);
//        }

//        [TestMethod("Strategy with overridden PreBuildUp"), TestProperty(TEST, ANALYSIS)]
//        public void Analysis_PreBuildUpStrategy()
//        {
//            // Arrange
//            Chain.Add(UnityBuildStage.Creation, new PreBuildUpStrategy());

//            // Act
//            var visitor = Chain.AnalyzePipeline<FakeContext>();

//            // Validate
//            Assert.IsNotNull(visitor);
//            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

//            var result = visitor(ref Context) as object[];

//            Assert.IsNotNull(result);
//            Assert.AreEqual(2, result.Length);
//            Assert.AreEqual(Chain.Version, result[1]);
//        }

//        [TestMethod("Multiple Strategies"), TestProperty(TEST, ANALYSIS)]
//        public void Analysis_Multiple()
//        {
//            // Arrange
//            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
//            Chain.Add(UnityBuildStage.Creation, new BothStrategies());
//            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

//            // Act
//            var visitor = Chain.AnalyzePipeline<FakeContext>();

//            // Validate
//            Assert.IsNotNull(visitor);
//            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

//            var result = visitor(ref Context) as object[];

//            Assert.IsNotNull(result);
//            Assert.AreEqual(4, result.Length);

//            Assert.AreSame(nameof(PreBuildUpStrategy), result[0]);
//            Assert.AreSame(nameof(BothStrategies), result[1]);
//            Assert.AreSame(nameof(PostBuildUpStrategy), result[2]);
//            Assert.AreEqual(Chain.Version, result[3]);
//        }


//        [TestMethod("Multiple with empty slot"), TestProperty(TEST, ANALYSIS)]
//        public void Analysis_Multiple_WithGap()
//        {
//            // Arrange
//            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
//            Chain.Add(UnityBuildStage.Creation, new NoStrategy());
//            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

//            // Act
//            var visitor = Chain.AnalyzePipeline<FakeContext>();

//            // Validate
//            Assert.IsNotNull(visitor);
//            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

//            var result = visitor(ref Context) as object[];

//            Assert.IsNotNull(result);
//            Assert.AreEqual(4, result.Length);

//            Assert.AreSame(nameof(PreBuildUpStrategy), result[0]);
//            Assert.IsNull(result[1]);
//            Assert.AreSame(nameof(PostBuildUpStrategy), result[2]);
//            Assert.AreEqual(Chain.Version, result[3]);
//        }

//        [TestMethod("Strategy with fault"), TestProperty(TEST, ANALYSIS)]
//        public void Analysis_Faulted()
//        {
//            // Arrange
//            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
//            Chain.Add(UnityBuildStage.Creation, new FaultedStrategy());
//            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

//            // Act
//            var visitor = Chain.AnalyzePipeline<FakeContext>();

//            // Validate
//            Assert.IsNotNull(visitor);
//            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

//            var result = visitor(ref Context) as object[];

//            Assert.IsNotNull(result);
//            Assert.AreEqual(4, result.Length);

//            Assert.AreSame(nameof(PreBuildUpStrategy), result[0]);
//            Assert.IsInstanceOfType(result[1], typeof(Exception));
//            Assert.AreSame(nameof(PostBuildUpStrategy), result[2]);
//            Assert.AreEqual(Chain.Version, result[3]);
//        }
//    }
//}
