using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Resolution
{
    [TestClass]
    public partial class Arrays : PatternBase
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => PatternBaseInitialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Test Data

        public interface IConstrained<TEntity>
            where TEntity : IService
        {
            TEntity Value { get; }
        }

        public class Constrained<TEntity> : IConstrained<TEntity>
            where TEntity : Service
        {
            public Constrained()
            {
            }

            public Constrained(TEntity value)
            {
                Value = value;
            }

            public TEntity Value { get; }
        }

        public class InjectedObject
        {
            public readonly object InjectedValue;

            public InjectedObject(object injectedValue)
            {
                this.InjectedValue = injectedValue;
            }
        }

        public class TypeWithArrayParameter
        {
            public readonly IService[] Loggers;

            public TypeWithArrayParameter(IService[] loggers)
            {
                Loggers = loggers;
            }
        }

        public class GenericTypeWithArrayParameter<T>
        {
            public readonly T[] Values;

            public GenericTypeWithArrayParameter(T[] values)
            {
                Values = values;
            }
        }

        public class ClassWithOneArrayGenericParameter<T>
        {
            private T[] injectedValue;
            public readonly bool DefaultConstructorCalled;

            public ClassWithOneArrayGenericParameter()
            {
                DefaultConstructorCalled = true;
            }

            public ClassWithOneArrayGenericParameter(T[] injectedValue)
            {
                DefaultConstructorCalled = false;

                this.injectedValue = injectedValue;
            }

            public T[] InjectedValue
            {
                get { return this.injectedValue; }
                set { this.injectedValue = value; }
            }
        }

        public class TypeWithArrayConstructorParameterOfRankTwo
        {
            private readonly IService[,] _unknown;

            public TypeWithArrayConstructorParameterOfRankTwo(IService[,] array)
            {
                _unknown = array;
            }
        }

        public class TypeWithArrayProperty
        {
            [Dependency]
            public IService[] Loggers { get; set; }
        }

        public class GenericTypeWithArrayProperty<T>
        {
            public T[] Prop { get; set; }
        }

        #endregion
    }
}
