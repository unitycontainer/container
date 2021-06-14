using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Regression
{
    public abstract partial class PatternBase
    {
        #region Fields

        protected IUnityContainer Container;

        private static string _type { get; set; }
        private static string _root { get; set; }
        private static string _prefix { get; set; }

        protected static string FullyQualifiedTestClassName;

        #endregion


        #region Properties

        public TestContext TestContext { get; set; }

        protected static string Category { get; private set; }

        protected static string Dependency { get; private set; }

        protected static string Member { get; private set; }

        protected virtual string DependencyName => string.Empty;

        #endregion


        #region Scaffolding

        // Pattern namespace should be in this format:
        // "ExecutedCategory.DependencyType.MemberTested"

        // Category could be:
        // 1. Import
        // 2. Selection
        // 3. etc.

        // Recognized Dependency Types:
        // 1. Implicit
        // 2. Optional
        // 3. Required

        // Members are:
        // 1. Constructors
        // 2. Methods
        // 3. Fields
        // 4. Properties

        // Example: Import.Implicit.Constructors

        protected static void PatternBaseInitialize(string name, Assembly assembly = null)
        {
            FullyQualifiedTestClassName = name;

            var type = assembly is null
                ? Type.GetType(FullyQualifiedTestClassName)
                : Type.GetType($"{FullyQualifiedTestClassName}, {assembly.GetName().Name}");

            var root = type.Namespace.Split('.');
            var @base = type.BaseType.Namespace.Split('.');

            Member = root.Last();
            Category = @base.First();
            Dependency = @base.Last();

            _type = type.Namespace;
            _prefix = root.First();
            _root = $"{type.BaseType.Namespace}.{Member}";

            LoadInjectionProxies();
        }

        public virtual void TestInitialize()
        {
            var container = new UnityContainer();
            Container = container;

#if  !UNITY_V4
#if   BEHAVIOR_ACTIVATED
            container.AddExtension(new ForceActivation());
#elif BEHAVIOR_RESOLVED
            container.AddExtension(new ForceResolution());
#elif BEHAVIOR_COMPILED
            container.AddExtension(new ForceCompillation());
#endif
#endif

            // In v4 compatibility mode add 'Legacy' extension
#if !UNITY_V4 && BEHAVIOR_V4
            container.AddExtension(new Unity.Extension.Legacy());
#endif
        }

        #endregion


        #region Get Test Type

        protected static Type GetTestType(string name)
        {
            return Type.GetType($"{Category}.{Dependency}.{Member}.{name}") ??
                   Type.GetType($"Regression.{Dependency}.{Member}.{name}");
        }

        protected static Type GetTestType(string dependency, string name)
        {
            return Type.GetType($"{Category}.{dependency}.{Member}.{name}") ??
                   Type.GetType($"Regression.{dependency}.{Member}.{name}");
        }

        protected static IEnumerable<Type> FromTestNamespaces(string name)
        {
            var regex = $"({_prefix}).*({Member}).*({name})";
            return Assembly.GetExecutingAssembly()
                           .DefinedTypes
                           .Where(t => t.Namespace is not null)
                           .Where(t => Regex.IsMatch(t.Namespace, regex));
        }

        #endregion


        #region Get Type

        protected static Type GetType(string name)
        {
            return Type.GetType($"{_root}.{name}") ??
                   Type.GetType($"{_type}.{name}");
        }

        protected static Type GetType(string @namespace, string name)
        {
            return Type.GetType($"{_prefix}.{@namespace}.{Member}.{name}") ??
                   Type.GetType($"{_type}.{@namespace}.{name}");
        }

        protected static Type GetTestType(string name, string annotation, string member)
        {
            return Type.GetType($"Regression.{annotation}.{member}.{name}");
        }

        protected static Type GetTestType(string name, string annotation, string member, string @namespace)
        {
            return Type.GetType($"{@namespace}.{annotation}.{member}.{name}") ??
                   Type.GetType($"Regression.{annotation}.{member}.{name}");
        }

        #endregion


        #region Registrations

        protected virtual void RegisterTypes()
        {
            Container.RegisterInstance(RegisteredInt)
                     .RegisterInstance(Name, NamedInt)
                     .RegisterInstance(RegisteredString)
                     .RegisterInstance(Name, NamedString)
                     .RegisterInstance(RegisteredUnresolvable)
                     .RegisterInstance(Name, NamedUnresolvable)
#if !BEHAVIOR_V4 && !UNITY_V4 // Only Unity v5 and up allow `null` as a value
                     .RegisterInstance(typeof(string), Null, (object)null)
                     .RegisterInstance(typeof(Unresolvable), Null, (object)null)
#endif
                     .RegisterInstance(typeof(TestStruct), RegisteredStruct)
                     .RegisterInstance(typeof(TestStruct), Name, NamedStruct);
        }

        protected virtual void RegisterUnResolvableTypes()
        {
            Container.RegisterInstance(RegisteredInt)
                     .RegisterInstance(RegisteredString)
                     .RegisterInstance(RegisteredUnresolvable)
                     .RegisterInstance(typeof(TestStruct), RegisteredStruct)
                     .RegisterInstance(RegisteredBool)
                     .RegisterInstance(RegisteredLong)
                     .RegisterInstance(RegisteredShort)
                     .RegisterInstance(RegisteredFloat)
                     .RegisterInstance(RegisteredDouble)
                     .RegisterInstance(RegisteredType)
                     .RegisterInstance(RegisteredICloneable)
                     .RegisterInstance(RegisteredDelegate);
        }

        #endregion


        #region Implementation

        protected static IEnumerable<Type> FromNamespace(string postfix)
        {
            var @namespace = $"{_root}.{postfix}";
            return Assembly.GetExecutingAssembly()
                           .DefinedTypes
                           .Where(t => (t.Namespace?.Equals(@namespace) ?? false));
        }

        protected static IEnumerable<Type> FromNamespaces(string prefix, string @namespace)
        {
            var regex = $"({_prefix}).*({prefix}).*({Member}).*({@namespace})$";
            return Assembly.GetExecutingAssembly()
                           .DefinedTypes
                           .Where(t => t.Namespace is not null)
                           .Where(t => Regex.IsMatch(t.Namespace, regex));
        }

        protected static IEnumerable<Type> FromPatternNamespace(string @namespace)
        {
            var regex = $"({_root}).*(.)({@namespace})$";
            return Assembly.GetExecutingAssembly()
                           .DefinedTypes
                           .Where(t => t.Namespace is not null)
                           .Where(t => Regex.IsMatch(t.Namespace, regex));
        }

        protected static IEnumerable<Type> FromNamespaces(string @namespace)
        {
            var regex = $"({_prefix}).*({Member}).*({@namespace})";
            return Assembly.GetExecutingAssembly()
                           .DefinedTypes
                           .Where(t => t.Namespace is not null)
                           .Where(t => Regex.IsMatch(t.Namespace, regex));
        }

        #endregion
    }
}

