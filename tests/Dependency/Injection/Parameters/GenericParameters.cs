using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class GenericParameters 
    {
        private static Type ParameterType =
            typeof(List<>).GetMethod("Add")
                          .GetParameters()
                          .First()
                          .ParameterType;

        private static ParameterInfo AddStringInfo =
            typeof(List<string>).GetMethod("Add")
                                .GetParameters()
                                .First();

        private static Type ArrayParameterType = ParameterType.MakeArrayType();

        [DataTestMethod]
        [DynamicData(nameof(DynamicDataSource.GenericBases), typeof(DynamicDataSource), DynamicDataSourceType.Method)]
        public virtual void EqualsTest(GenericBase parameter)
        {
            // Validate
            //Assert.IsNotNull(parameter);

            //Assert.IsFalse(parameter.Equals(typeof(object)));

            //if (parameter is GenericResolvedArrayParameter)
            //{
            //    Assert.IsTrue(parameter.Equals(ArrayParameterType));
            //}
            //else
            //{
            //    var name = parameter.ParameterTypeName;
            //    Type type = parameter.ParameterTypeName switch
            //    {
            //        "T[]" => ArrayParameterType,
            //        _ => ParameterType,
            //    };

            //    Assert.IsTrue(parameter.Equals(type));
            //}
        }


        #region Test Data

        #endregion
    }
}
