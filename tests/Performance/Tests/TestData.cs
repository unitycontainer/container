using Unity;

namespace Runner.Tests
{

    public class Poco
    {
        [Dependency]
        public object Dependency { get; set; }

        [InjectionMethod]
        public void CallMe() { }
    }

    public interface IFoo { }
    public class Foo : IFoo
    {
        [Dependency]
        public object Dependency { get; set; }

        [InjectionMethod]
        public void CallMe() { }
    }


    public interface IService { }

    public class Service : IService
    {
        [Dependency]
        public object Dependency { get; set; }

        [InjectionMethod]
        public void CallMe() { }
    }
}
