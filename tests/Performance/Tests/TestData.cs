using Unity;

namespace Runner.Tests
{

    public class Poco
    {
        [Dependency]
        public object Dependency { get; set; }

        [InjectionMethod]
        public object CallMe([Dependency]object data) => data;
    }


    public interface IFoo { }

    public class Foo : IFoo
    {
        [Dependency]
        public object Dependency { get; set; }

        [InjectionMethod]
        public object CallMe([Dependency]object data) => data;
    }
}
