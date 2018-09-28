using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.Policy;
using Unity.Tests.v5.TestDoubles;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5.ObjectBuilder
{
    [TestClass]
    public class PropertySelectorFixture
    {
        [TestMethod]
        public void SelectorReturnsEmptyListWhenObjectHasNoSettableProperties()
        {
            List<PropertyInfo> properties = SelectProperties(typeof(object));

            Assert.AreEqual(0, properties.Count);
        }

        [TestMethod]
        public void SelectorReturnsOnlySettablePropertiesMarkedWithAttribute()
        {
            List<PropertyInfo> properties = SelectProperties(typeof(ClassWithProperties));

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual("PropTwo", properties[0].Name);
        }

        [TestMethod]
        public void SelectorIgnoresIndexers()
        {
            List<PropertyInfo> properties = SelectProperties(typeof(ClassWithIndexer));

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual("Key", properties[0].Name);
        }

        private List<PropertyInfo> SelectProperties(Type t)
        {
            IPropertySelectorPolicy selector = new PropertySelectorPolicy<DependencyAttribute>();
            IBuilderContext context = GetContext(t);
            var properties = new List<SelectedProperty>(selector.SelectProperties(context));
            return properties.Select(sp => sp.Property).ToList();
        }

        private MockBuilderContext GetContext(Type t)
        {
            var context = new MockBuilderContext();
            context.BuildKey = new NamedTypeBuildKey(t);
            return context;
        }
    }

    internal class ClassWithProperties
    {
        private string two;
        private string three;

        [Dependency]
        public string PropOne
        {
            get { return null; }
        }

        [Dependency]
        public string PropTwo
        {
            get { return two; }
            set { two = value; }
        }

        public string PropThree
        {
            get { return three; }
            set { three = value; }
        }
    }

    internal class ClassWithIndexer
    {
        private string key;

        [Dependency]
        public string this[int index]
        {
            get { return null; }
            set { key = index.ToString() + value; }
        }

        [Dependency]
        public string Key
        {
            get { return key; }
            set { key = value; }
        }
    }
}
