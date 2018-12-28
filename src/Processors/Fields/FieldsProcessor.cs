using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public class FieldsProcessor : BuildMemberProcessor<FieldInfo, object>
    {
        #region Fields

        private static readonly MethodInfo ResolveField =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });

        #endregion


        #region Overrides

        protected override FieldInfo[] DeclaredMembers(Type type)
        {
            return new FieldInfo[0];
        }

        protected override Type MemberType(FieldInfo info) => info.FieldType;

        protected override ResolveDelegate<BuilderContext> GetResolver(FieldInfo info, string name, object resolver)
        {
            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, name, resolver));
                return context.Existing;
            };
        }

        protected override Expression GetExpression(FieldInfo field, string name, object resolver)
        {
            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context, ResolveField,
                    Expression.Constant(field, typeof(FieldInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                field.FieldType);
        }

        protected override MemberExpression CreateMemberExpression(FieldInfo info) 
            => Expression.Field(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info);

        #endregion


        #region Parameter Resolver Factories

        protected override ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, string name, object resolver, object defaultValue)
        {
            return (ref BuilderContext context) =>
            {
                ((FieldInfo)info).SetValue(context.Existing,
                    context.Resolve((FieldInfo)info, ((DependencyResolutionAttribute)attribute).Name ?? name, resolver));

                return context.Existing;
            };
        }

        protected override ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, string name, object resolver, object defaultValue)
        {
            return (ref BuilderContext context) =>
            {
                try
                {
                    ((PropertyInfo)info).SetValue(context.Existing,
                        context.Resolve((FieldInfo)info, ((DependencyResolutionAttribute)attribute).Name ?? name, resolver));
                    return context.Existing;
                }
                catch
                {
                    ((FieldInfo)info).SetValue(context.Existing, defaultValue);
                    return context.Existing;
                }
            };
        }

        #endregion
    }
}
