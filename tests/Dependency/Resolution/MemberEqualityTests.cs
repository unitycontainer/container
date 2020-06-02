using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using Unity.Policy.Tests;
using Unity.Resolution;

namespace Resolution.Overrides
{
    [TestClass]
    public class MemberEqualityTests
    {
        #region Fields

        public static object        OverrideValue { get; } = new object();
        public static FieldInfo     FieldInfo = typeof(PolicySet).GetField(nameof(PolicySet.NameField));
        public static PropertyInfo  PropertyInfo = typeof(PolicySet).GetProperty(nameof(PolicySet.NameProperty));
        public static NamedType     NamedType = new NamedType { Type = typeof(object), Name = string.Empty };

        #endregion

        [TestMethod]
        public void EqualsNull()
        {
            // Field
            var field = new FieldOverride(string.Empty, OverrideValue);
            Assert.IsFalse(field.Equals((object)null));
            Assert.IsFalse(field.Equals((FieldInfo)null));

            // Property
            var property = new PropertyOverride(string.Empty, OverrideValue);
            Assert.IsFalse(property.Equals((object)null));
            Assert.IsFalse(property.Equals((PropertyInfo)null));

            // Parameter
            var parameter = new ParameterOverride(string.Empty, OverrideValue);
            Assert.IsFalse(parameter.Equals((object)null));
            Assert.IsFalse(parameter.Equals((ParameterInfo)null));

            // Dependency
            Assert.IsFalse(new DependencyOverride(typeof(object), OverrideValue).Equals(null));
        }


        [DataTestMethod]
        [DynamicData(nameof(GetTestResolvers), DynamicDataSourceType.Method)]
        public void EqualsTest(ResolverOverride instance, object other, bool result)
        {
            // Validate
            Assert.AreEqual(result, instance.Equals(other));
        }


        #region Test Data

        public static IEnumerable<object[]> GetTestResolvers()
        {
            yield return new object[] { new FieldOverride(string.Empty,                OverrideValue),                           OverrideValue, false };
            yield return new object[] { new FieldOverride(string.Empty,                OverrideValue),                           FieldInfo,     false };
            yield return new object[] { new FieldOverride(null, OverrideValue),                                                  FieldInfo,     true  }; // TODO: Issue #156
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField), OverrideValue),                           FieldInfo,     true  };
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField), OverrideValue).OnType<FieldInfo>(),       FieldInfo,     false };
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField), OverrideValue).OnType<PolicySet>(),       FieldInfo,     true  };
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField), OverrideValue).OnType<PolicySet>(), 
                                        new FieldOverride(nameof(PolicySet.NameField), OverrideValue).OnType<PolicySet>(),                      true };

            yield return new object[] { new PropertyOverride(string.Empty, OverrideValue),                                       OverrideValue, false };
            yield return new object[] { new PropertyOverride(null, OverrideValue),                                               PropertyInfo,  true  }; // TODO: Issue #156
            yield return new object[] { new PropertyOverride(string.Empty, OverrideValue),                                       PropertyInfo,  false };
            yield return new object[] { new PropertyOverride(nameof(PolicySet.NameProperty), OverrideValue),                     PropertyInfo,  true  };
            yield return new object[] { new PropertyOverride(nameof(PolicySet.NameProperty), OverrideValue).OnType<FieldInfo>(), PropertyInfo,  false };
            yield return new object[] { new PropertyOverride(nameof(PolicySet.NameProperty), OverrideValue).OnType<PolicySet>(), 
                                        new PropertyOverride(nameof(PolicySet.NameProperty), OverrideValue).OnType<PolicySet>(),                true  };

            yield return new object[] { new DependencyOverride(typeof(object), OverrideValue),                                   OverrideValue, false };
            yield return new object[] { new DependencyOverride(string.Empty, OverrideValue),                                     NamedType,     false };
            yield return new object[] { new DependencyOverride(typeof(object), string.Empty, OverrideValue),                     NamedType,     true  };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(object), string.Empty, OverrideValue),     NamedType,     true  };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(object), string.Empty, OverrideValue),                          
                                        new DependencyOverride(typeof(object), typeof(object), string.Empty, OverrideValue),                    true  };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(string), string.Empty, OverrideValue),
                                        new DependencyOverride(typeof(object), typeof(object), string.Empty, OverrideValue),                    false };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(object), string.Empty, OverrideValue),                             // TODO: Issue #157
                                        new DependencyOverride(typeof(string), typeof(object), string.Empty, OverrideValue),                    true  };
            yield return new object[] { new DependencyOverride<object>(OverrideValue),             new NamedType { Type = typeof(object) },     true  }; // TODO: Issue #158

            //yield return new object[] { new ParameterOverride(string.Empty, OverrideValue),                 OverrideValue, false };
            //yield return new object[] { new ParameterOverride(typeof(object), OverrideValue),               OverrideValue, false };
            //yield return new object[] { new ParameterOverride(typeof(object), string.Empty, OverrideValue), OverrideValue, false };
        }

        #endregion
    }
}
