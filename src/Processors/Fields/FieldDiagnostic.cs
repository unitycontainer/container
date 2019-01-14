using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public class FieldDiagnostic : FieldProcessor
    {
        #region Constructors

        public FieldDiagnostic(IPolicySet policySet) : base(policySet)
        {
        }

        #endregion


        #region Overrides

        protected override Expression GetResolverExpression(FieldInfo field, string name, object resolver)
        {
            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = 
                Expression.Block(field.FieldType,
                    Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(field, typeof(object))),
                Expression.Rethrow(field.FieldType));

            return Expression.TryCatch(base.GetResolverExpression(field, name, resolver),
                   Expression.Catch(ex, block));
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object resolver)
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
