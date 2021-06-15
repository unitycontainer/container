using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Selection.Annotated
{
    public abstract partial class Pattern
    {
        #region Constants

        protected const string SELECTION_EDGE = "Edge Cases";

        #endregion


        #region Successful

        [DynamicData(nameof(TestCases_Data))]
        [PatternTestMethod("Test Cases ({2})"), TestCategory(SELECTION_EDGE)]
        public virtual void Selection_TestCases_Successful(string test, Type type)
        {
            var instance = Assert_ResolutionSuccess(type);

            if (instance is not SelectionBaseType selection) return;

            Assert.IsTrue(selection.IsSuccessful);
        }

        #endregion


        #region Failing

        [ExpectedException(typeof(ResolutionFailedException))]
        [DynamicData(nameof(TestCases_Throwing_Data))]
        [PatternTestMethod("Test Cases ({2})"), TestCategory(SELECTION_EDGE)]
        public virtual void Selection_TestCases_Throwing(string test, Type type)
            => Assert_ResolutionSuccess(type);


        [DynamicData(nameof(NoPublicMember_Data))]
        [PatternTestMethod("Validation ({2})"), TestCategory("Validation")]
        public virtual void Selection_Validation_Throwing(string test, Type type)
        // TODO:    => Assert_ResolutionSuccess(type);
        { }

        #endregion
    }
}
