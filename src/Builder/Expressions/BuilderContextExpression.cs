using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Container.Lifetime;
using Unity.Resolution;
using Unity.ResolverPolicy;

namespace Unity.Builder.Expressions
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public class BuilderContextExpression : IResolveContextExpression<BuilderContext>
    {
        #region Fields

        private static ConstructorInfo _perResolveInfo = typeof(InternalPerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();


        private static readonly MethodInfo _resolveField =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });

        private static readonly MethodInfo _resolveProperty =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });

        private static readonly MethodInfo _resolveParameter =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        #endregion


        #region Constructor

        static BuilderContextExpression()
        {
            var typeInfo = typeof(BuilderContext).GetTypeInfo();


            Existing  = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(BuilderContext.Existing)));
        }

        #endregion


        #region Public Properties

        public static readonly MemberExpression Existing;

        #endregion


        #region Methods

        public static Expression Resolve(FieldInfo field, string name, object resolver = null)
        {
            return Expression.Convert(
                Expression.Call(Context, _resolveField,
                    Expression.Constant(field, typeof(FieldInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                field.FieldType);
        }

        public static Expression Resolve(PropertyInfo property, string name, object resolver = null)
        {
            return Expression.Convert(
                Expression.Call(Context, _resolveProperty,
                    Expression.Constant(property, typeof(PropertyInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                property.PropertyType);
        }

        public static Expression Resolve(ParameterInfo parameter, string name, object resolver = null)
        {
            return Expression.Convert(
                Expression.Call(Context, _resolveParameter,
                    Expression.Constant(parameter, typeof(ParameterInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                parameter.ParameterType);
        }

        public static IEnumerable<Expression> GetParameters(MethodBase methodBase)
        {
            foreach (var parameter in methodBase.GetParameters())
            {
                // Resolve all DependencyAttributes on this parameter, if any
                var attribute = parameter.GetCustomAttributes(false)
                    .OfType<DependencyResolutionAttribute>()
                    .FirstOrDefault();

                if (null == attribute)
                    yield return Resolve(parameter, null, null);
                else
                    yield return attribute is OptionalDependencyAttribute
                        ? Resolve(parameter, attribute.Name, new OptionalDependencyResolvePolicy(parameter.ParameterType, attribute.Name))
                        : Resolve(parameter, attribute.Name, null);
            }
        }

        public static IEnumerable<Expression> GetParameters(ConstructorInfo info, object[] values)
        {
            var parameters = info.GetParameters();
            var resolvers = null != values && 0 == values.Length ? null : values;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                // Resolve all DependencyAttributes on this parameter, if any
                var attribute = parameter.GetCustomAttributes(false)
                    .OfType<DependencyResolutionAttribute>()
                    .FirstOrDefault();

                yield return Resolve(parameter, attribute?.Name, resolvers?[i]);
            }
        }

        public static Expression SetPerBuildSingleton(ref BuilderContext context)
        {
            return Set(context.RegistrationType, context.RegistrationName, typeof(LifetimeManager),
                Expression.New(_perResolveInfo, context.Variable));
        }

        #endregion
    }
}
