using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Performance.Tests
{
    [BenchmarkCategory("Basic")]
    [Config(typeof(BenchmarkConfiguration))]
    public class BasicBase
    {
        protected IUnityContainer _container;
        protected object[] _storage = new object[20];
        private object foo = new Foo();

        public virtual void SetupContainer()
        {
            _container.RegisterType<PocoWithDependency00>();
            _container.RegisterType<PocoWithDependency01>();
            _container.RegisterType<PocoWithDependency02>();
            _container.RegisterType<PocoWithDependency03>();
            _container.RegisterType<PocoWithDependency04>();
            _container.RegisterType<PocoWithDependency05>();
            _container.RegisterType<PocoWithDependency06>();
            _container.RegisterType<PocoWithDependency07>();
            _container.RegisterType<PocoWithDependency08>();
            _container.RegisterType<PocoWithDependency09>();
            _container.RegisterType<PocoWithDependency10>();
            _container.RegisterType<PocoWithDependency11>();
            _container.RegisterType<PocoWithDependency12>();
            _container.RegisterType<PocoWithDependency13>();
            _container.RegisterType<PocoWithDependency14>();
            _container.RegisterType<PocoWithDependency15>();
            _container.RegisterType<PocoWithDependency16>();
            _container.RegisterType<PocoWithDependency17>();
            _container.RegisterType<PocoWithDependency18>();
            _container.RegisterType<PocoWithDependency19>();

            _container.RegisterType<IFoo00, Foo00>();
            _container.RegisterType<IFoo01, Foo01>();
            _container.RegisterType<IFoo02, Foo02>();
            _container.RegisterType<IFoo03, Foo03>();
            _container.RegisterType<IFoo04, Foo04>();
            _container.RegisterType<IFoo05, Foo05>();
            _container.RegisterType<IFoo06, Foo06>();
            _container.RegisterType<IFoo07, Foo07>();
            _container.RegisterType<IFoo08, Foo08>();
            _container.RegisterType<IFoo09, Foo09>();
            _container.RegisterType<IFoo10, Foo10>();
            _container.RegisterType<IFoo11, Foo11>();
            _container.RegisterType<IFoo12, Foo12>();
            _container.RegisterType<IFoo13, Foo13>();
            _container.RegisterType<IFoo14, Foo14>();
            _container.RegisterType<IFoo15, Foo15>();
            _container.RegisterType<IFoo16, Foo16>();
            _container.RegisterType<IFoo17, Foo17>();
            _container.RegisterType<IFoo18, Foo18>();
            _container.RegisterType<IFoo19, Foo19>();

            _container.RegisterType<IFoo00, Foo00>("1");
            _container.RegisterType<IFoo01, Foo01>("1");
            _container.RegisterType<IFoo02, Foo02>("1");
            _container.RegisterType<IFoo03, Foo03>("1");
            _container.RegisterType<IFoo04, Foo04>("1");
            _container.RegisterType<IFoo05, Foo05>("1");
            _container.RegisterType<IFoo06, Foo06>("1");
            _container.RegisterType<IFoo07, Foo07>("1");
            _container.RegisterType<IFoo08, Foo08>("1");
            _container.RegisterType<IFoo09, Foo09>("1");
            _container.RegisterType<IFoo10, Foo10>("1");
            _container.RegisterType<IFoo11, Foo11>("1");
            _container.RegisterType<IFoo12, Foo12>("1");
            _container.RegisterType<IFoo13, Foo13>("1");
            _container.RegisterType<IFoo14, Foo14>("1");
            _container.RegisterType<IFoo15, Foo15>("1");
            _container.RegisterType<IFoo16, Foo16>("1");
            _container.RegisterType<IFoo17, Foo17>("1");
            _container.RegisterType<IFoo18, Foo18>("1");
            _container.RegisterType<IFoo19, Foo19>("1");

            _container.RegisterFactory<IFoo00>("2", c => new Foo00());
            _container.RegisterFactory<IFoo01>("2", c => new Foo01());
            _container.RegisterFactory<IFoo02>("2", c => new Foo02());
            _container.RegisterFactory<IFoo03>("2", c => new Foo03());
            _container.RegisterFactory<IFoo04>("2", c => new Foo04());
            _container.RegisterFactory<IFoo05>("2", c => new Foo05());
            _container.RegisterFactory<IFoo06>("2", c => new Foo06());
            _container.RegisterFactory<IFoo07>("2", c => new Foo07());
            _container.RegisterFactory<IFoo08>("2", c => new Foo08());
            _container.RegisterFactory<IFoo09>("2", c => new Foo09());
            _container.RegisterFactory<IFoo10>("2", c => new Foo10());
            _container.RegisterFactory<IFoo11>("2", c => new Foo11());
            _container.RegisterFactory<IFoo12>("2", c => new Foo12());
            _container.RegisterFactory<IFoo13>("2", c => new Foo13());
            _container.RegisterFactory<IFoo14>("2", c => new Foo14());
            _container.RegisterFactory<IFoo15>("2", c => new Foo15());
            _container.RegisterFactory<IFoo16>("2", c => new Foo16());
            _container.RegisterFactory<IFoo17>("2", c => new Foo17());
            _container.RegisterFactory<IFoo18>("2", c => new Foo18());
            _container.RegisterFactory<IFoo19>("2", c => new Foo19());

            _container.RegisterType(typeof(IFoo00<>), typeof(Foo00<>));
            _container.RegisterType(typeof(IFoo01<>), typeof(Foo01<>));
            _container.RegisterType(typeof(IFoo02<>), typeof(Foo02<>));
            _container.RegisterType(typeof(IFoo03<>), typeof(Foo03<>));
            _container.RegisterType(typeof(IFoo04<>), typeof(Foo04<>));
            _container.RegisterType(typeof(IFoo05<>), typeof(Foo05<>));
            _container.RegisterType(typeof(IFoo06<>), typeof(Foo06<>));
            _container.RegisterType(typeof(IFoo07<>), typeof(Foo07<>));
            _container.RegisterType(typeof(IFoo08<>), typeof(Foo08<>));
            _container.RegisterType(typeof(IFoo09<>), typeof(Foo09<>));
            _container.RegisterType(typeof(IFoo10<>), typeof(Foo10<>));
            _container.RegisterType(typeof(IFoo11<>), typeof(Foo11<>));
            _container.RegisterType(typeof(IFoo12<>), typeof(Foo12<>));
            _container.RegisterType(typeof(IFoo13<>), typeof(Foo13<>));
            _container.RegisterType(typeof(IFoo14<>), typeof(Foo14<>));
            _container.RegisterType(typeof(IFoo15<>), typeof(Foo15<>));
            _container.RegisterType(typeof(IFoo16<>), typeof(Foo16<>));
            _container.RegisterType(typeof(IFoo17<>), typeof(Foo17<>));
            _container.RegisterType(typeof(IFoo18<>), typeof(Foo18<>));
            _container.RegisterType(typeof(IFoo19<>), typeof(Foo19<>));

            _container.RegisterInstance(typeof(IFoo<Foo00>), new Foo<Foo00>());
            _container.RegisterInstance(typeof(IFoo<Foo01>), new Foo<Foo01>());
            _container.RegisterInstance(typeof(IFoo<Foo02>), new Foo<Foo02>());
            _container.RegisterInstance(typeof(IFoo<Foo03>), new Foo<Foo03>());
            _container.RegisterInstance(typeof(IFoo<Foo04>), new Foo<Foo04>());
            _container.RegisterInstance(typeof(IFoo<Foo05>), new Foo<Foo05>());
            _container.RegisterInstance(typeof(IFoo<Foo06>), new Foo<Foo06>());
            _container.RegisterInstance(typeof(IFoo<Foo07>), new Foo<Foo07>());
            _container.RegisterInstance(typeof(IFoo<Foo08>), new Foo<Foo08>());
            _container.RegisterInstance(typeof(IFoo<Foo09>), new Foo<Foo09>());
            _container.RegisterInstance(typeof(IFoo<Foo10>), new Foo<Foo10>());
            _container.RegisterInstance(typeof(IFoo<Foo11>), new Foo<Foo11>());
            _container.RegisterInstance(typeof(IFoo<Foo12>), new Foo<Foo12>());
            _container.RegisterInstance(typeof(IFoo<Foo13>), new Foo<Foo13>());
            _container.RegisterInstance(typeof(IFoo<Foo14>), new Foo<Foo14>());
            _container.RegisterInstance(typeof(IFoo<Foo15>), new Foo<Foo15>());
            _container.RegisterInstance(typeof(IFoo<Foo16>), new Foo<Foo16>());
            _container.RegisterInstance(typeof(IFoo<Foo17>), new Foo<Foo17>());
            _container.RegisterInstance(typeof(IFoo<Foo18>), new Foo<Foo18>());
            _container.RegisterInstance(typeof(IFoo<Foo19>), new Foo<Foo19>());


            _container.RegisterInstance(new Foo<object>());
            _container.RegisterType(typeof(IFoo<object>), typeof(Foo<object>));

            _container.RegisterFactory<IFoo>(c => foo);
            _container.RegisterType<IFoo,   Foo>(  "1");
            _container.RegisterType<PocoWithDependency>();
            _container.RegisterType(typeof(IFoo<>),   typeof(Foo<>));
        }

        public virtual object UnityContainer()
        {
            _storage[00] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[01] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[02] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[03] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[04] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[05] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[06] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[07] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[08] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[09] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[10] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[11] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[12] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[13] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[14] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[15] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[16] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[17] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[18] = _container.Resolve(typeof(IUnityContainer), null);
            _storage[19] = _container.Resolve(typeof(IUnityContainer), null);

            return _storage;
        }

        public virtual object UnityContainerAsync()
        {
            _storage[00] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[01] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[02] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[03] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[04] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[05] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[06] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[07] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[08] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[09] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[10] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[11] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[12] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[13] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[14] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[15] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[16] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[17] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[18] = _container.Resolve(typeof(IUnityContainerAsync), null);
            _storage[19] = _container.Resolve(typeof(IUnityContainerAsync), null);

            return _storage;
        }

        public virtual object Unregistered()
        {
            _storage[00] = _container.Resolve(typeof(Poco00), null);
            _storage[01] = _container.Resolve(typeof(Poco01), null);
            _storage[02] = _container.Resolve(typeof(Poco02), null);
            _storage[03] = _container.Resolve(typeof(Poco03), null);
            _storage[04] = _container.Resolve(typeof(Poco04), null);
            _storage[05] = _container.Resolve(typeof(Poco05), null);
            _storage[06] = _container.Resolve(typeof(Poco06), null);
            _storage[07] = _container.Resolve(typeof(Poco07), null);
            _storage[08] = _container.Resolve(typeof(Poco08), null);
            _storage[09] = _container.Resolve(typeof(Poco09), null);
            _storage[10] = _container.Resolve(typeof(Poco10), null);
            _storage[11] = _container.Resolve(typeof(Poco11), null);
            _storage[12] = _container.Resolve(typeof(Poco12), null);
            _storage[13] = _container.Resolve(typeof(Poco13), null);
            _storage[14] = _container.Resolve(typeof(Poco14), null);
            _storage[15] = _container.Resolve(typeof(Poco15), null);
            _storage[16] = _container.Resolve(typeof(Poco16), null);
            _storage[17] = _container.Resolve(typeof(Poco17), null);
            _storage[18] = _container.Resolve(typeof(Poco18), null);
            _storage[19] = _container.Resolve(typeof(Poco19), null);

            return _storage;
        }

        public virtual object Transient()
        {
            _storage[00] = _container.Resolve(typeof(PocoWithDependency00), null);
            _storage[01] = _container.Resolve(typeof(PocoWithDependency01), null);
            _storage[02] = _container.Resolve(typeof(PocoWithDependency02), null);
            _storage[03] = _container.Resolve(typeof(PocoWithDependency03), null);
            _storage[04] = _container.Resolve(typeof(PocoWithDependency04), null);
            _storage[05] = _container.Resolve(typeof(PocoWithDependency05), null);
            _storage[06] = _container.Resolve(typeof(PocoWithDependency06), null);
            _storage[07] = _container.Resolve(typeof(PocoWithDependency07), null);
            _storage[08] = _container.Resolve(typeof(PocoWithDependency08), null);
            _storage[09] = _container.Resolve(typeof(PocoWithDependency09), null);
            _storage[10] = _container.Resolve(typeof(PocoWithDependency10), null);
            _storage[11] = _container.Resolve(typeof(PocoWithDependency11), null);
            _storage[12] = _container.Resolve(typeof(PocoWithDependency12), null);
            _storage[13] = _container.Resolve(typeof(PocoWithDependency13), null);
            _storage[14] = _container.Resolve(typeof(PocoWithDependency14), null);
            _storage[15] = _container.Resolve(typeof(PocoWithDependency15), null);
            _storage[16] = _container.Resolve(typeof(PocoWithDependency16), null);
            _storage[17] = _container.Resolve(typeof(PocoWithDependency17), null);
            _storage[18] = _container.Resolve(typeof(PocoWithDependency18), null);
            _storage[19] = _container.Resolve(typeof(PocoWithDependency19), null);

            return _storage;
        }

        public virtual object Mapping()
        {
            _storage[00] = _container.Resolve(typeof(IFoo00), null);
            _storage[01] = _container.Resolve(typeof(IFoo01), null);
            _storage[02] = _container.Resolve(typeof(IFoo02), null);
            _storage[03] = _container.Resolve(typeof(IFoo03), null);
            _storage[04] = _container.Resolve(typeof(IFoo04), null);
            _storage[05] = _container.Resolve(typeof(IFoo05), null);
            _storage[06] = _container.Resolve(typeof(IFoo06), null);
            _storage[07] = _container.Resolve(typeof(IFoo07), null);
            _storage[08] = _container.Resolve(typeof(IFoo08), null);
            _storage[09] = _container.Resolve(typeof(IFoo09), null);
            _storage[10] = _container.Resolve(typeof(IFoo10), null);
            _storage[11] = _container.Resolve(typeof(IFoo11), null);
            _storage[12] = _container.Resolve(typeof(IFoo12), null);
            _storage[13] = _container.Resolve(typeof(IFoo13), null);
            _storage[14] = _container.Resolve(typeof(IFoo14), null);
            _storage[15] = _container.Resolve(typeof(IFoo15), null);
            _storage[16] = _container.Resolve(typeof(IFoo16), null);
            _storage[17] = _container.Resolve(typeof(IFoo17), null);
            _storage[18] = _container.Resolve(typeof(IFoo18), null);
            _storage[19] = _container.Resolve(typeof(IFoo19), null);

            return _storage;
        }

        public virtual object MappingToSingleton()
        {
            _storage[00] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[01] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[02] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[03] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[04] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[05] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[06] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[07] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[08] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[09] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[10] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[11] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[12] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[13] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[14] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[15] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[16] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[17] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[18] = _container.Resolve(typeof(IFoo<object>), null);
            _storage[19] = _container.Resolve(typeof(IFoo<object>), null);

            return _storage;
        }

        public virtual object GenericInterface()
        {
            _storage[00] = _container.Resolve(typeof(IFoo00<IFoo00>), null);
            _storage[01] = _container.Resolve(typeof(IFoo01<IFoo01>), null);
            _storage[02] = _container.Resolve(typeof(IFoo02<IFoo02>), null);
            _storage[03] = _container.Resolve(typeof(IFoo03<IFoo03>), null);
            _storage[04] = _container.Resolve(typeof(IFoo04<IFoo04>), null);
            _storage[05] = _container.Resolve(typeof(IFoo05<IFoo05>), null);
            _storage[06] = _container.Resolve(typeof(IFoo06<IFoo06>), null);
            _storage[07] = _container.Resolve(typeof(IFoo07<IFoo07>), null);
            _storage[08] = _container.Resolve(typeof(IFoo08<IFoo08>), null);
            _storage[09] = _container.Resolve(typeof(IFoo09<IFoo09>), null);
            _storage[10] = _container.Resolve(typeof(IFoo10<IFoo10>), null);
            _storage[11] = _container.Resolve(typeof(IFoo11<IFoo11>), null);
            _storage[12] = _container.Resolve(typeof(IFoo12<IFoo12>), null);
            _storage[13] = _container.Resolve(typeof(IFoo13<IFoo13>), null);
            _storage[14] = _container.Resolve(typeof(IFoo14<IFoo14>), null);
            _storage[15] = _container.Resolve(typeof(IFoo15<IFoo15>), null);
            _storage[16] = _container.Resolve(typeof(IFoo16<IFoo16>), null);
            _storage[17] = _container.Resolve(typeof(IFoo17<IFoo17>), null);
            _storage[18] = _container.Resolve(typeof(IFoo18<IFoo18>), null);
            _storage[19] = _container.Resolve(typeof(IFoo19<IFoo19>), null);

            return _storage;
        }

        public virtual object Factory()
        {
            _storage[00] = _container.Resolve(typeof(IFoo), null);
            _storage[01] = _container.Resolve(typeof(IFoo), null);
            _storage[02] = _container.Resolve(typeof(IFoo), null);
            _storage[03] = _container.Resolve(typeof(IFoo), null);
            _storage[04] = _container.Resolve(typeof(IFoo), null);
            _storage[05] = _container.Resolve(typeof(IFoo), null);
            _storage[06] = _container.Resolve(typeof(IFoo), null);
            _storage[07] = _container.Resolve(typeof(IFoo), null);
            _storage[08] = _container.Resolve(typeof(IFoo), null);
            _storage[09] = _container.Resolve(typeof(IFoo), null);
            _storage[10] = _container.Resolve(typeof(IFoo), null);
            _storage[11] = _container.Resolve(typeof(IFoo), null);
            _storage[12] = _container.Resolve(typeof(IFoo), null);
            _storage[13] = _container.Resolve(typeof(IFoo), null);
            _storage[14] = _container.Resolve(typeof(IFoo), null);
            _storage[15] = _container.Resolve(typeof(IFoo), null);
            _storage[16] = _container.Resolve(typeof(IFoo), null);
            _storage[17] = _container.Resolve(typeof(IFoo), null);
            _storage[18] = _container.Resolve(typeof(IFoo), null);
            _storage[19] = _container.Resolve(typeof(IFoo), null);

            return _storage;
        }

        public virtual object Instance()
        {
            _storage[00] = _container.Resolve(typeof(IFoo<Foo00>), null);
            _storage[01] = _container.Resolve(typeof(IFoo<Foo01>), null);
            _storage[02] = _container.Resolve(typeof(IFoo<Foo02>), null);
            _storage[03] = _container.Resolve(typeof(IFoo<Foo03>), null);
            _storage[04] = _container.Resolve(typeof(IFoo<Foo04>), null);
            _storage[05] = _container.Resolve(typeof(IFoo<Foo05>), null);
            _storage[06] = _container.Resolve(typeof(IFoo<Foo06>), null);
            _storage[07] = _container.Resolve(typeof(IFoo<Foo07>), null);
            _storage[08] = _container.Resolve(typeof(IFoo<Foo08>), null);
            _storage[09] = _container.Resolve(typeof(IFoo<Foo09>), null);
            _storage[10] = _container.Resolve(typeof(IFoo<Foo10>), null);
            _storage[11] = _container.Resolve(typeof(IFoo<Foo11>), null);
            _storage[12] = _container.Resolve(typeof(IFoo<Foo12>), null);
            _storage[13] = _container.Resolve(typeof(IFoo<Foo13>), null);
            _storage[14] = _container.Resolve(typeof(IFoo<Foo14>), null);
            _storage[15] = _container.Resolve(typeof(IFoo<Foo15>), null);
            _storage[16] = _container.Resolve(typeof(IFoo<Foo16>), null);
            _storage[17] = _container.Resolve(typeof(IFoo<Foo17>), null);
            _storage[18] = _container.Resolve(typeof(IFoo<Foo18>), null);
            _storage[19] = _container.Resolve(typeof(IFoo<Foo19>), null);

            return _storage;
        }

        public virtual object LegacyFactory()
        {
            _storage[00] = _container.Resolve(typeof(IFoo00), "2");
            _storage[01] = _container.Resolve(typeof(IFoo01), "2");
            _storage[02] = _container.Resolve(typeof(IFoo02), "2");
            _storage[03] = _container.Resolve(typeof(IFoo03), "2");
            _storage[04] = _container.Resolve(typeof(IFoo04), "2");
            _storage[05] = _container.Resolve(typeof(IFoo05), "2");
            _storage[06] = _container.Resolve(typeof(IFoo06), "2");
            _storage[07] = _container.Resolve(typeof(IFoo07), "2");
            _storage[08] = _container.Resolve(typeof(IFoo08), "2");
            _storage[09] = _container.Resolve(typeof(IFoo09), "2");
            _storage[10] = _container.Resolve(typeof(IFoo10), "2");
            _storage[11] = _container.Resolve(typeof(IFoo11), "2");
            _storage[12] = _container.Resolve(typeof(IFoo12), "2");
            _storage[13] = _container.Resolve(typeof(IFoo13), "2");
            _storage[14] = _container.Resolve(typeof(IFoo14), "2");
            _storage[15] = _container.Resolve(typeof(IFoo15), "2");
            _storage[16] = _container.Resolve(typeof(IFoo16), "2");
            _storage[17] = _container.Resolve(typeof(IFoo17), "2");
            _storage[18] = _container.Resolve(typeof(IFoo18), "2");
            _storage[19] = _container.Resolve(typeof(IFoo19), "2");

            return _storage;
        }

        public virtual object Array()
        {
            _storage[00] = _container.Resolve(typeof(IFoo00[]), null);
            _storage[01] = _container.Resolve(typeof(IFoo01[]), null);
            _storage[02] = _container.Resolve(typeof(IFoo02[]), null);
            _storage[03] = _container.Resolve(typeof(IFoo03[]), null);
            _storage[04] = _container.Resolve(typeof(IFoo04[]), null);
            _storage[05] = _container.Resolve(typeof(IFoo05[]), null);
            _storage[06] = _container.Resolve(typeof(IFoo06[]), null);
            _storage[07] = _container.Resolve(typeof(IFoo07[]), null);
            _storage[08] = _container.Resolve(typeof(IFoo08[]), null);
            _storage[09] = _container.Resolve(typeof(IFoo09[]), null);
            _storage[10] = _container.Resolve(typeof(IFoo10[]), null);
            _storage[11] = _container.Resolve(typeof(IFoo11[]), null);
            _storage[12] = _container.Resolve(typeof(IFoo12[]), null);
            _storage[13] = _container.Resolve(typeof(IFoo13[]), null);
            _storage[14] = _container.Resolve(typeof(IFoo14[]), null);
            _storage[15] = _container.Resolve(typeof(IFoo15[]), null);
            _storage[16] = _container.Resolve(typeof(IFoo16[]), null);
            _storage[17] = _container.Resolve(typeof(IFoo17[]), null);
            _storage[18] = _container.Resolve(typeof(IFoo18[]), null);
            _storage[19] = _container.Resolve(typeof(IFoo19[]), null);

            return _storage;
        }

        public virtual object Enumerable()
        {
            _storage[00] = (_container.Resolve(typeof(IEnumerable<IFoo00>), null) as IEnumerable<IFoo00>)?.Count();
            _storage[01] = (_container.Resolve(typeof(IEnumerable<IFoo01>), null) as IEnumerable<IFoo01>)?.Count();
            _storage[02] = (_container.Resolve(typeof(IEnumerable<IFoo02>), null) as IEnumerable<IFoo02>)?.Count();
            _storage[03] = (_container.Resolve(typeof(IEnumerable<IFoo03>), null) as IEnumerable<IFoo03>)?.Count();
            _storage[04] = (_container.Resolve(typeof(IEnumerable<IFoo04>), null) as IEnumerable<IFoo04>)?.Count();
            _storage[05] = (_container.Resolve(typeof(IEnumerable<IFoo05>), null) as IEnumerable<IFoo05>)?.Count();
            _storage[06] = (_container.Resolve(typeof(IEnumerable<IFoo06>), null) as IEnumerable<IFoo06>)?.Count();
            _storage[07] = (_container.Resolve(typeof(IEnumerable<IFoo07>), null) as IEnumerable<IFoo07>)?.Count();
            _storage[08] = (_container.Resolve(typeof(IEnumerable<IFoo08>), null) as IEnumerable<IFoo08>)?.Count();
            _storage[09] = (_container.Resolve(typeof(IEnumerable<IFoo09>), null) as IEnumerable<IFoo09>)?.Count();
            _storage[10] = (_container.Resolve(typeof(IEnumerable<IFoo10>), null) as IEnumerable<IFoo10>)?.Count();
            _storage[11] = (_container.Resolve(typeof(IEnumerable<IFoo11>), null) as IEnumerable<IFoo11>)?.Count();
            _storage[12] = (_container.Resolve(typeof(IEnumerable<IFoo12>), null) as IEnumerable<IFoo12>)?.Count();
            _storage[13] = (_container.Resolve(typeof(IEnumerable<IFoo13>), null) as IEnumerable<IFoo13>)?.Count();
            _storage[14] = (_container.Resolve(typeof(IEnumerable<IFoo14>), null) as IEnumerable<IFoo14>)?.Count();
            _storage[15] = (_container.Resolve(typeof(IEnumerable<IFoo15>), null) as IEnumerable<IFoo15>)?.Count();
            _storage[16] = (_container.Resolve(typeof(IEnumerable<IFoo16>), null) as IEnumerable<IFoo16>)?.Count();
            _storage[17] = (_container.Resolve(typeof(IEnumerable<IFoo17>), null) as IEnumerable<IFoo17>)?.Count();
            _storage[18] = (_container.Resolve(typeof(IEnumerable<IFoo18>), null) as IEnumerable<IFoo18>)?.Count();
            _storage[19] = (_container.Resolve(typeof(IEnumerable<IFoo19>), null) as IEnumerable<IFoo19>)?.Count();

            return _storage;
        }
    }
}
