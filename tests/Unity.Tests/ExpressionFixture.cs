using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;
using Unity.ResolverPolicy;

namespace Unity.Tests.v5
{
    [TestClass]
    public class ExpressionFixture
    {
        public IUnityContainer _container;
        public MockContext _context = new MockContext();

        [TestInitialize]
        public void Setup()
        {
            _container = new UnityContainer();
            _context.Existing = null;
            _context.Type = typeof(Service);
        }

        [TestMethod]
        public void CreateNewExpression()
        {
            ref var context = ref _context;

            var fieldInfo = context.Type.GetField(nameof(Service.Field));
            var propInfo = context.Type.GetProperty(nameof(Service.Property));

            var varExpr = Expression.Variable(context.Type, "instance");

            var fieldExpr = Expression.Field(varExpr, fieldInfo);
            var propExpr = Expression.Property(varExpr, propInfo);

            var blockExpr = Expression.Block(new ParameterExpression[] { varExpr },  new Expression[]
            {
                Expression.IfThenElse(Expression.NotEqual( Expression.Constant(null), BuilderContextExpression.Existing),
                    Expression.Assign( varExpr, Expression.Convert(BuilderContextExpression.Existing, context.Type)),
                    Expression.Assign( varExpr, Expression.New(typeof(Service)))),

                Expression.Assign(fieldExpr, BuilderContextExpression.Resolve(fieldInfo, null, null)),
                Expression.Assign(propExpr, BuilderContextExpression.Resolve(propInfo, null, null)),

                varExpr
            });


            var lambda = Expression.Lambda<ResolveDelegate<MockContext>>(blockExpr, BuilderContextExpression.Context);
            var plan = lambda.Compile();
            var instance = (Service)plan(ref context);

            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.Field, Service.First);
            Assert.AreEqual(instance.Property, Service.Other);
        }

        [TestMethod]
        public void BaselineTest()
        {
            ResolveDelegate<BuilderContext> plan = (ref BuilderContext context) =>
            {
                Service instance;
                if (null == context.Existing)
                {
                    instance = new Service();
                }
                else
                {
                    instance = (Service)context.Existing;
                }

                instance.Field = Service.First;
                instance.Property = Service.Other;

                return instance;
            };

            var result = new Service();
            result.Field = Service.First;
            result.Property = Service.Other;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Field, Service.First);
            Assert.AreEqual(result.Property, Service.Other);
        }


        public interface IService { }
        public class Service : IService
        {
            public static string First = "name";
            public static string Other = "other";

            public string Field;

            public string Property { get; set; }
        }

        public class BuilderContextExpression : IResolveContextExpression<MockContext>
        {
            public static readonly MemberExpression Existing = 
                Expression.MakeMemberAccess(Context, typeof(MockContext).GetTypeInfo()
                                                                        .GetDeclaredProperty(nameof(MockContext.Existing)));
        }

        public struct MockContext : IResolveContext
        {

            #region ResolveContext

            public IUnityContainer Container => null;

            public object Existing { get; set; }

            public object Resolve(Type type, string name)
            {
                throw new NotImplementedException();
            }

            public object Resolve(FieldInfo field, string name, object value) => Service.First;

            public object Resolve(PropertyInfo property, string name, object value) => Service.Other;

            public object Resolve(ParameterInfo parameter, string name, object value)
            {
                throw new NotImplementedException();
            }

            #endregion


            #region Build

            public Type Type { get; set; }

            public string Name { get; set; }

            public object Get(Type type, string name, Type policyInterface)
            {
                throw new NotImplementedException();
            }

            public void Set(Type type, string name, Type policyInterface, object policy)
            {
                throw new NotImplementedException();
            }

            public void Clear(Type type, string name, Type policyInterface)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
