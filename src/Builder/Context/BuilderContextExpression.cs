using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Builder
{
    public class BuilderContextExpression : IResolveContextExpression<BuilderContext>
    {
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
    }
}
