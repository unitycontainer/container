using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Policy;

namespace Container
{
    public partial class Defaults
    {
        [TestMethod("Empty Count"), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Baseline() => Assert.AreEqual(DefaultPolicies, Policies.Span.Length);


        #region Get

        [TestMethod("Safe to call Get(..., (PolicyChangeHandler)null)"), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_type_null()
        {
            Policies.Get(typeof(object), (PolicyChangeHandler)null);

            Policies.Set(typeof(object), Instance);

            Assert.AreSame(Instance, Policies.Get(typeof(object)));
        }

        #endregion


        #region Set


        [TestMethod("Safe to call Set(..., (PolicyChangeHandler)null)"), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Set_type_null()
        {
            Policies.Set(typeof(object), Instance, (PolicyChangeHandler)null);

            Assert.AreSame(Instance, Policies.Get(typeof(object)));
        }

        #endregion
    }
}
