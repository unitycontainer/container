using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Builder.Policy;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.ResolverPolicy;
using Unity.Storage;
using Unity.Strategy;
using Unity.Tests.v5.Generics;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5.ObjectBuilder
{
    /// <summary>
    /// Summary description for SpecifiedConstructorSelectorPolicyFixture
    /// </summary>
    [TestClass]
    public class SpecifiedConstructorSelectorPolicyFixture
    {
        [TestMethod]
        public void SelectConstructorWithNoParameters()
        {
            ConstructorInfo ctor = typeof(ClassWithSimpleConstructor).GetMatchingConstructor(new Type[0]);

            var policy = new SpecifiedConstructorSelectorPolicy(ctor, new InjectionParameterValue[0]);
            var builderContext = new BuilderContextMock(new NamedTypeBuildKey(typeof(ClassWithSimpleConstructor)));

            SelectedConstructor selectedCtor = policy.SelectConstructor(builderContext);

            Assert.AreEqual(ctor, selectedCtor.Constructor);
            Assert.AreEqual(0, selectedCtor.GetParameterResolvers().Length);
        }

        [TestMethod]
        public void SelectConstructorWith2Parameters()
        {
            ConstructorInfo ctor = typeof(ClassWithConstructorParameters).GetMatchingConstructor(Types(typeof(int), typeof(string)));

            var policy = new SpecifiedConstructorSelectorPolicy(ctor,
                new InjectionParameterValue[]
                {
                    new InjectionParameter<int>(37),
                    new InjectionParameter<string>("abc")
                });

            var builderContext = new BuilderContextMock(new NamedTypeBuildKey(typeof(ClassWithConstructorParameters)));

            SelectedConstructor selectedCtor = policy.SelectConstructor(builderContext);

            Assert.AreEqual(ctor, selectedCtor.Constructor);
            Assert.AreEqual(2, selectedCtor.GetParameterResolvers().Length);

            var resolvers = selectedCtor.GetParameterResolvers();
            Assert.AreEqual(2, resolvers.Length);
            foreach (var resolverPolicy in resolvers)
            {
                AssertPolicyIsCorrect(resolverPolicy);
            }
        }

        [TestMethod]
        public void CanSelectConcreteConstructorGivenGenericConstructor()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            var policy = new SpecifiedConstructorSelectorPolicy(
                ctor,
                new InjectionParameterValue[]
                {
                    new ResolvedParameter(typeof(ICommand<>), "concrete")
                });

            var ctx = new BuilderContextMock
                {
                    BuildKey = new NamedTypeBuildKey(typeof(LoggingCommand<User>))
                };

            SelectedConstructor result = policy.SelectConstructor(ctx);

            ConstructorInfo expectedCtor = typeof(LoggingCommand<User>).GetMatchingConstructor(Types(typeof(ICommand<User>)));
            Assert.AreSame(expectedCtor, result.Constructor);
        }

        private static void AssertPolicyIsCorrect(IResolverPolicy policy)
        {
            Assert.IsNotNull(policy);
            AssertExtensions.IsInstanceOfType(policy, typeof(LiteralValueDependencyResolverPolicy));
        }

        private Type[] Types(params Type[] types)
        {
            return types;
        }

        private class ClassWithSimpleConstructor
        {
        }

        private class ClassWithConstructorParameters
        {
            public ClassWithConstructorParameters(int param1, string param2)
            {
            }
        }

        private class BuilderContextMock : IBuilderContext
        {
            private readonly IPolicyList persistentPolicies = new PolicyList();

            public BuilderContextMock()
            {
            }

            public BuilderContextMock(NamedTypeBuildKey buildKey)
            {
                this.BuildKey = buildKey;
            }

            public IStrategyChain Strategies
            {
                get { throw new NotImplementedException(); }
            }

            public ILifetimeContainer Lifetime
            {
                get { throw new NotImplementedException(); }
            }

            public INamedType OriginalBuildKey
            {
                get { return BuildKey; }
            }

            public IPolicyList PersistentPolicies
            {
                get { return persistentPolicies; }
            }

            public IPolicyList Policies
            {
                get { throw new NotImplementedException(); }
            }

            public IRecoveryStack RecoveryStack
            {
                get { throw new NotImplementedException(); }
            }

            public INamedType BuildKey { get; set; }

            public object Existing
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public bool BuildComplete
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public object CurrentOperation
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public IBuilderContext ChildContext
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public IUnityContainer Container { get; set; }

            public IBuilderContext ParentContext => throw new NotImplementedException();

            public IRequiresRecovery RequiresRecovery { get; set; }

            public BuilderStrategy[] BuildChain => throw new NotImplementedException();

            public IPolicySet Registration => throw new NotImplementedException();

            public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
            {
                throw new NotImplementedException();
            }

            public IResolverPolicy GetOverriddenResolver(Type dependencyType)
            {
                throw new NotImplementedException();
            }

            public object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
