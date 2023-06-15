﻿using BenchmarkDotNet.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Lifetime;

namespace Container.Scope
{
    public abstract partial class ScopeTests
    {
        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterType()
        {
            // Act
            Scope.Register(typeof(ScopeTests), null, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(1, Scope.Version);
            Assert.AreEqual(1, Scope.Count);
            Assert.AreEqual(1, Scope.ToArray().Length);
        }

        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterSameType()
        {
            // Act
            Scope.Register(typeof(ScopeTests), null, new ContainerControlledLifetimeManager());
            Scope.Register(typeof(ScopeTests), null, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(2, Scope.Version);
            Assert.AreEqual(2, Scope.Count);
            Assert.AreEqual(2, Scope.ToArray().Length);
        }


        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterNamedThenType()
        {
            // Act
            Scope.Register(typeof(ScopeTests), Name, new ContainerControlledLifetimeManager());
            Scope.Register(typeof(ScopeTests), null, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(2, Scope.Version);
            Assert.AreEqual(2, Scope.Count);
            Assert.AreEqual(2, Scope.ToArray().Length);
        }

        [Benchmark]
        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterTypeWithName()
        {
            // Act
            Scope.Register(typeof(ScopeTests), Name, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(1, Scope.Version);
            Assert.AreEqual(2, Scope.Count);
        }

        [Ignore]
        [DataTestMethod, DynamicData(nameof(Test_Contract_Data), typeof(ScopeTests))]
        public void Register(Type type, string name)
        {
            // Act
            Scope.Register(typeof(ScopeTests), Name, new ContainerControlledLifetimeManager());
            Scope.Register(typeof(ScopeTests), Name, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(2, Scope.Version);
            Assert.AreEqual(2, Scope.Count);
            Assert.AreEqual(2, Scope.ToArray().Length);
        }
    }
}