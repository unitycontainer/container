using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System.Threading.Tasks;

namespace Resolution
{
    [TestClass]
    public partial class FromEmpty : PatternBase
    {
        #region Fields

        static ParallelOptions MaxDegreeOfParallelism = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        #endregion
    }
}
