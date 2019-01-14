using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public class PropertyDiagnostic : PropertyProcessor
    {
        #region Constructors

        public PropertyDiagnostic(IPolicySet policySet) 
            : base(policySet)
        {
        }

        #endregion


        #region Overrides

        protected override Expression GetResolverExpression(PropertyInfo property, string name, object resolver)
        {
            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = 
                Expression.Block(property.PropertyType,
                    Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(property, typeof(object))),
                Expression.Rethrow(property.PropertyType));

            return Expression.TryCatch(base.GetResolverExpression(property, name, resolver),
                   Expression.Catch(ex, block));
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                try
                {
                    info.SetValue(context.Existing, context.Resolve(info, context.Name, value));
                    return context.Existing;
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }
            };
        }

        #endregion
    }
}
