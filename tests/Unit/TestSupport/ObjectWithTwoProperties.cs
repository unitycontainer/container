using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.v5.TestSupport
{
    public class ObjectWithTwoProperties
    {
        private object obj1;
        private object obj2;

        [Dependency]
        public object Obj1
        {
            get { return obj1; }
            set { obj1 = value; }
        }

        [Dependency]
        public object Obj2
        {
            get { return obj2; }
            set { obj2 = value; }
        }

        public void Validate()
        {
            Assert.IsNotNull(obj1);
            Assert.IsNotNull(obj2);
            Assert.AreNotSame(obj1, obj2);
        }
    }
}
