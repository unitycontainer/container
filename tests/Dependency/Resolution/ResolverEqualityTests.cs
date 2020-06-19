using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy.Tests;
using Unity.Resolution;

namespace Resolution.Overrides
{
    [TestClass]
    public class ResolverEqualityTests
    {
        #region Fields

        public static object        TestValue = new object();
        public static FieldInfo     FieldInfo = typeof(PolicySet).GetField(nameof(PolicySet.NameField));
        public static PropertyInfo  PropertyInfo = typeof(PolicySet).GetProperty(nameof(PolicySet.NameProperty));
        public static NamedType     NamedType = new NamedType { Type = typeof(object), Name = string.Empty };
        public static ParameterInfo ParameterInfo = typeof(PolicySet).GetConstructor(new Type[] { typeof(string) })
                                                                     .GetParameters()
                                                                     .First();
        #endregion


        [TestMethod]
        public void EqualsNull()
        {
            // Field
            var field = new FieldOverride(string.Empty, TestValue);
            Assert.IsFalse(field.Equals((object)null));
            Assert.IsFalse(field.Equals((FieldInfo)null));

            // Property
            var property = new PropertyOverride(string.Empty, TestValue);
            Assert.IsFalse(property.Equals((object)null));
            Assert.IsFalse(property.Equals((PropertyInfo)null));

            // Parameter
            var parameter = new ParameterOverride(string.Empty, TestValue);
            Assert.IsFalse(parameter.Equals((object)null));
            Assert.IsFalse(parameter.Equals((ParameterInfo)null));

            // Dependency
            Assert.IsFalse(new DependencyOverride(typeof(object), TestValue).Equals(null));
        }


        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public void EqualsObjectTest(ResolverOverride resolver)
        {
            // Act
            var result = resolver.Equals((object)resolver);

            // Validate
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public void EqualsWrongTest(ResolverOverride resolver)
        {
            // Validate
            Assert.IsFalse(resolver.Equals(this));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public void EqualsOperatorTest(ResolverOverride resolver)
        {
            // Validate
            Assert.IsFalse(null == resolver);
            Assert.IsFalse(resolver == null);

            Assert.IsTrue(null != resolver);
            Assert.IsTrue(resolver != null);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetEqualsTestData), DynamicDataSourceType.Method)]
        public void EqualsTest(ResolverOverride instance, object other, bool result)
        {
            // Validate
            Assert.AreEqual(result, instance.Equals(other));
        }


        #region Test Data

        public static IEnumerable<object[]> GetAllResolvers()
        {

            yield return new object[] { new FieldOverride(string.Empty, TestValue) };

            yield return new object[] { new PropertyOverride(string.Empty, TestValue) };

            yield return new object[] { new DependencyOverride(typeof(object), TestValue) };
            yield return new object[] { new DependencyOverride(string.Empty, TestValue) };
            yield return new object[] { new DependencyOverride(typeof(object), string.Empty, TestValue) };
            yield return new object[] { new DependencyOverride(typeof(ResolverOverride), typeof(object), string.Empty, TestValue) };
            yield return new object[] { new DependencyOverride<object>(TestValue) };
            yield return new object[] { new DependencyOverride<object>(string.Empty, TestValue) };

            yield return new object[] { new ParameterOverride(string.Empty, TestValue) };
            yield return new object[] { new ParameterOverride(typeof(object), TestValue) };
            yield return new object[] { new ParameterOverride(typeof(object), string.Empty, TestValue) };

        }

        public static IEnumerable<object[]> GetEqualsTestData()
        {
            yield return new object[] { new FieldOverride(string.Empty,                      TestValue),                      TestValue,     false };
            yield return new object[] { new FieldOverride(string.Empty,                      TestValue),                      FieldInfo,     false };
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField),       TestValue),                      FieldInfo,     true  };
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField),       TestValue).OnType<FieldInfo>(),  FieldInfo,     false };
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField),       TestValue).OnType<PolicySet>(),  FieldInfo,     true  };
            yield return new object[] { new FieldOverride(nameof(PolicySet.NameField),       TestValue).OnType<PolicySet>(), 
                                        new FieldOverride(nameof(PolicySet.NameField),       TestValue).OnType<PolicySet>(),                 true  };
            yield return new object[] { new FieldOverride(string.Empty,                      TestValue).OnType<PolicySet>(),
                                        new FieldOverride(nameof(PolicySet.NameField),       TestValue).OnType<PolicySet>(),                 false  };

            yield return new object[] { new PropertyOverride(string.Empty,                   TestValue),                      TestValue,     false };
            yield return new object[] { new PropertyOverride(string.Empty,                   TestValue),                      PropertyInfo,  false };
            yield return new object[] { new PropertyOverride(nameof(PolicySet.NameProperty), TestValue),                      PropertyInfo,  true  };
            yield return new object[] { new PropertyOverride(nameof(PolicySet.NameProperty), TestValue).OnType<FieldInfo>(),  PropertyInfo,  false };
            yield return new object[] { new PropertyOverride(nameof(PolicySet.NameProperty), TestValue).OnType<PolicySet>(), 
                                        new PropertyOverride(nameof(PolicySet.NameProperty), TestValue).OnType<PolicySet>(),                true  };
            yield return new object[] { new PropertyOverride(string.Empty,                   TestValue).OnType<PolicySet>(),
                                        new PropertyOverride(nameof(PolicySet.NameProperty), TestValue).OnType<PolicySet>(),                false  };

            yield return new object[] { new DependencyOverride(typeof(object), TestValue),                                   TestValue,     false };
            yield return new object[] { new DependencyOverride(string.Empty, TestValue),                                     NamedType,     false };
            yield return new object[] { new DependencyOverride(typeof(object), string.Empty, TestValue),                     NamedType,     true  };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(object), string.Empty, TestValue),     NamedType,     true  };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(object), string.Empty, TestValue),                          
                                        new DependencyOverride(typeof(object), typeof(object), string.Empty, TestValue),                    true  };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(string), string.Empty, TestValue),
                                        new DependencyOverride(typeof(object), typeof(object), string.Empty, TestValue),                    false };
            yield return new object[] { new DependencyOverride(typeof(object), typeof(object), string.Empty, TestValue),                          
                                        new DependencyOverride(typeof(string), typeof(object), string.Empty, TestValue),                    false };
            yield return new object[] { new DependencyOverride<object>(TestValue),                 new NamedType { Type = typeof(object) }, true };
            yield return new object[] { new DependencyOverride<object>(string.Empty, TestValue),                             NamedType,     true };

            yield return new object[] { new ParameterOverride(string.Empty, TestValue),                                      TestValue,     false };
            yield return new object[] { new ParameterOverride(typeof(string), TestValue),                                    TestValue,     false };
            yield return new object[] { new ParameterOverride(typeof(string), string.Empty, TestValue),                      TestValue,     false };
            yield return new object[] { new ParameterOverride((string)null, TestValue),                                      ParameterInfo, true };
            yield return new object[] { new ParameterOverride(string.Empty, TestValue),                                      ParameterInfo, false };
            yield return new object[] { new ParameterOverride(ParameterInfo.Name, TestValue),                                ParameterInfo, true  };
            yield return new object[] { new ParameterOverride(ParameterInfo.Name, TestValue).OnType<PolicySet>(),            ParameterInfo, true };
            yield return new object[] { new ParameterOverride(typeof(string), TestValue),                                    ParameterInfo, true  };
            yield return new object[] { new ParameterOverride(typeof(string), ParameterInfo.Name, TestValue),                ParameterInfo, true  };
            yield return new object[] { new ParameterOverride(ParameterInfo.Name, TestValue), 
                                        new ParameterOverride(ParameterInfo.Name, TestValue),                                               true };
            yield return new object[] { new ParameterOverride(typeof(string), TestValue),
                                        new ParameterOverride(typeof(string), TestValue),                                                   true };
            yield return new object[] { new ParameterOverride(typeof(string), ParameterInfo.Name, TestValue),
                                        new ParameterOverride(typeof(string), ParameterInfo.Name, TestValue),                               true };
            yield return new object[] { new ParameterOverride(typeof(object), ParameterInfo.Name, TestValue),
                                        new ParameterOverride(typeof(string), ParameterInfo.Name, TestValue),                               false };
            yield return new object[] { new ParameterOverride(typeof(string), ParameterInfo.Name, TestValue),
                                        new ParameterOverride(typeof(string), string.Empty, TestValue),                                     false };
            yield return new object[] { new ParameterOverride(typeof(string), ParameterInfo.Name, TestValue).OnType<PolicySet>(),
                                        new ParameterOverride(typeof(string), ParameterInfo.Name, TestValue).OnType(typeof(PolicySet)),     true };
        }

        #endregion
    }
}
