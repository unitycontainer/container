using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Constructors
{
    [TestClass]
    public class Selecting_Annotated : Selection.Annotated.Pattern
    {
        #region Properties

        protected override string DependencyName => "value";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void Selecting_Annotated_Initialize(TestContext context) 
            => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Constructor specific behavior

        [ExpectedException(typeof(ResolutionFailedException))]
        [DynamicData(nameof(NoPublicMember_Data), typeof(Selection.Annotated.Pattern))]
        [PatternTestMethod("Validation ({2})"), TestCategory("Validation")]
        public override void Selection_Validation_Throwing(string test, Type type) 
            => Assert_ResolutionSuccess(type);
        
        #endregion
    }
}
