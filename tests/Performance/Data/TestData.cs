using Unity;

namespace Performance.Tests
{
    #region Plain Old CLR object

    public class Poco00 { }
    public class Poco01 { }
    public class Poco02 { }
    public class Poco03 { }
    public class Poco04 { }
    public class Poco05 { }
    public class Poco06 { }
    public class Poco07 { }
    public class Poco08 { }
    public class Poco09 { }
    public class Poco10 { }
    public class Poco11 { }
    public class Poco12 { }
    public class Poco13 { }
    public class Poco14 { }
    public class Poco15 { }
    public class Poco16 { }
    public class Poco17 { }
    public class Poco18 { }
    public class Poco19 { }

    #endregion


    #region PocoWithDependency

    public class PocoWithDependency
    {
        [Dependency]
        public object Dependency { get; set; }

        [InjectionMethod]
        public object CallMe([Dependency]object data) => data;
    }

    public class PocoWithDependency00 : PocoWithDependency { }
    public class PocoWithDependency01 : PocoWithDependency { }
    public class PocoWithDependency02 : PocoWithDependency { }
    public class PocoWithDependency03 : PocoWithDependency { }
    public class PocoWithDependency04 : PocoWithDependency { }
    public class PocoWithDependency05 : PocoWithDependency { }
    public class PocoWithDependency06 : PocoWithDependency { }
    public class PocoWithDependency07 : PocoWithDependency { }
    public class PocoWithDependency08 : PocoWithDependency { }
    public class PocoWithDependency09 : PocoWithDependency { }
    public class PocoWithDependency10 : PocoWithDependency { }
    public class PocoWithDependency11 : PocoWithDependency { }
    public class PocoWithDependency12 : PocoWithDependency { }
    public class PocoWithDependency13 : PocoWithDependency { }
    public class PocoWithDependency14 : PocoWithDependency { }
    public class PocoWithDependency15 : PocoWithDependency { }
    public class PocoWithDependency16 : PocoWithDependency { }
    public class PocoWithDependency17 : PocoWithDependency { }
    public class PocoWithDependency18 : PocoWithDependency { }
    public class PocoWithDependency19 : PocoWithDependency { }

    #endregion


    #region IFoo

    public interface IFoo { }
    public interface IFoo00 { }
    public interface IFoo01 { }
    public interface IFoo02 { }
    public interface IFoo03 { }
    public interface IFoo04 { }
    public interface IFoo05 { }
    public interface IFoo06 { }
    public interface IFoo07 { }
    public interface IFoo08 { }
    public interface IFoo09 { }
    public interface IFoo10 { }
    public interface IFoo11 { }
    public interface IFoo12 { }
    public interface IFoo13 { }
    public interface IFoo14 { }
    public interface IFoo15 { }
    public interface IFoo16 { }
    public interface IFoo17 { }
    public interface IFoo18 { }
    public interface IFoo19 { }

    #endregion


    #region Foo

    public class Foo : IFoo { }
    public class Foo00 : IFoo00 { }
    public class Foo01 : IFoo01 { }
    public class Foo02 : IFoo02 { }
    public class Foo03 : IFoo03 { }
    public class Foo04 : IFoo04 { }
    public class Foo05 : IFoo05 { }
    public class Foo06 : IFoo06 { }
    public class Foo07 : IFoo07 { }
    public class Foo08 : IFoo08 { }
    public class Foo09 : IFoo09 { }
    public class Foo10 : IFoo10 { }
    public class Foo11 : IFoo11 { }
    public class Foo12 : IFoo12 { }
    public class Foo13 : IFoo13 { }
    public class Foo14 : IFoo14 { }
    public class Foo15 : IFoo15 { }
    public class Foo16 : IFoo16 { }
    public class Foo17 : IFoo17 { }
    public class Foo18 : IFoo18 { }
    public class Foo19 : IFoo19 { }

    #endregion


    #region IFoo<>

    public interface IFoo<T> { }
    public interface IFoo00<T> { }
    public interface IFoo01<T> { }
    public interface IFoo02<T> { }
    public interface IFoo03<T> { }
    public interface IFoo04<T> { }
    public interface IFoo05<T> { }
    public interface IFoo06<T> { }
    public interface IFoo07<T> { }
    public interface IFoo08<T> { }
    public interface IFoo09<T> { }
    public interface IFoo10<T> { }
    public interface IFoo11<T> { }
    public interface IFoo12<T> { }
    public interface IFoo13<T> { }
    public interface IFoo14<T> { }
    public interface IFoo15<T> { }
    public interface IFoo16<T> { }
    public interface IFoo17<T> { }
    public interface IFoo18<T> { }
    public interface IFoo19<T> { }

    #endregion


    #region IFoo<>

    public class Foo<T> : IFoo<T> { }
    public class Foo00<T> : IFoo00<T> { }
    public class Foo01<T> : IFoo01<T> { }
    public class Foo02<T> : IFoo02<T> { }
    public class Foo03<T> : IFoo03<T> { }
    public class Foo04<T> : IFoo04<T> { }
    public class Foo05<T> : IFoo05<T> { }
    public class Foo06<T> : IFoo06<T> { }
    public class Foo07<T> : IFoo07<T> { }
    public class Foo08<T> : IFoo08<T> { }
    public class Foo09<T> : IFoo09<T> { }
    public class Foo10<T> : IFoo10<T> { }
    public class Foo11<T> : IFoo11<T> { }
    public class Foo12<T> : IFoo12<T> { }
    public class Foo13<T> : IFoo13<T> { }
    public class Foo14<T> : IFoo14<T> { }
    public class Foo15<T> : IFoo15<T> { }
    public class Foo16<T> : IFoo16<T> { }
    public class Foo17<T> : IFoo17<T> { }
    public class Foo18<T> : IFoo18<T> { }
    public class Foo19<T> : IFoo19<T> { }

    #endregion
}
