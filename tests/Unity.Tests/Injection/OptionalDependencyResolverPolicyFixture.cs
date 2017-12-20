// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.ResolverPolicy;
using Unity.Strategy;

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyResolverPolicyFixture
    /// </summary>
    [TestClass]
    public class OptionalDependencyResolverPolicyFixture
    {
        [TestMethod]
        public void CanCreateResolverWithNoName()
        {
            var resolver = new OptionalDependencyResolverPolicy(typeof(object));
            Assert.AreEqual(typeof(object), resolver.DependencyType);
            Assert.IsNull(resolver.Name);
        }

        [TestMethod]
        public void CanCreateResolverWithName()
        {
            var resolver = new OptionalDependencyResolverPolicy(typeof(object), "name");
            Assert.AreEqual(typeof(object), resolver.DependencyType);
            Assert.AreEqual("name", resolver.Name);
        }

        [TestMethod]
        public void ResolverReturnsNullWhenDependencyIsNotResolved()
        {
            IBuilderContext context = GetMockContextThatThrows();
            var resolver = new OptionalDependencyResolverPolicy(typeof(object));

            object result = resolver.Resolve(context);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ResolverReturnsBuiltObject()
        {
            string expected = "Here's the string to resolve";
            IBuilderContext context = GetMockContextThatResolvesUnnamedStrings(expected);
            var resolver = new OptionalDependencyResolverPolicy(typeof(string));

            object result = resolver.Resolve(context);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void ResolverReturnsProperNamedObject()
        {
            string expected = "We want this one";
            string notExpected = "Not this one";

            var expectedKey = NamedTypeBuildKey.Make<string>("expected");
            var notExpectedKey = NamedTypeBuildKey.Make<string>();

            var mainContext = new MockContext();
            mainContext.NewBuildupCallback = (k) =>
            {
                if ((NamedTypeBuildKey)k == expectedKey)
                {
                    return expected;
                }
                if ((NamedTypeBuildKey)k == notExpectedKey)
                {
                    return notExpected;
                }
                return null;
            };

            var resolver = new OptionalDependencyResolverPolicy(typeof(string), "expected");

            object result = resolver.Resolve(mainContext);

            Assert.AreSame(expected, result);
        }

        #region Helper methods and classes to get appropriate OB mock contexts

        private IBuilderContext GetMockContextThatThrows()
        {
            var mockContext = new MockContext();
            mockContext.NewBuildupCallback = (c) => { throw new InvalidOperationException(); };
            return mockContext;
        }

        private IBuilderContext GetMockContextThatResolvesUnnamedStrings(string expected)
        {
            var mockContext = new MockContext();
            mockContext.NewBuildupCallback = (c) =>
            {
                return expected;
            };
            return mockContext;
        }

        public class MockContext : IBuilderContext
        {
            public Func<INamedType, object> NewBuildupCallback;

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
                get { throw new NotImplementedException(); }
            }

            public INamedType BuildKey
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public IPolicyList PersistentPolicies
            {
                get { throw new NotImplementedException(); }
            }

            public IPolicyList Policies
            {
                get { throw new NotImplementedException(); }
            }

            public IRecoveryStack RecoveryStack
            {
                get { throw new NotImplementedException(); }
            }

            public object Existing
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool BuildComplete
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public object CurrentOperation
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public IBuilderContext ChildContext
            {
                get { throw new NotImplementedException(); }
            }

            public IUnityContainer Container { get; set; }

            public IBuilderContext ParentContext => throw new NotImplementedException();

            public void AddResolverOverrides(System.Collections.Generic.IEnumerable<ResolverOverride> newOverrides)
            {
                throw new NotImplementedException();
            }

            public IResolverPolicy GetOverriddenResolver(Type dependencyType)
            {
                throw new NotImplementedException();
            }

            public object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null)
            {
                return NewBuildupCallback(new NamedTypeBuildKey(type, name));
            }
        }

        #endregion
    }
}
