using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public class PipelineContextExpression : IResolveContextExpression<PipelineContext>
    {
        #region Fields

        public static readonly MethodInfo ResolvePropertyMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });

        public static readonly MethodInfo ResolveFieldMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });

        public static readonly MethodInfo ResolveParameterMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        public static readonly MethodInfo SetMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Set))
                .First(m => 2 == m.GetParameters().Length);

        #endregion


        #region Constructor

        static PipelineContextExpression()
        {
            var typeInfo = typeof(PipelineContext).GetTypeInfo();


            Existing  = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(PipelineContext.Existing)));
        }

        #endregion


        #region Public Properties

        public static readonly MemberExpression Existing;

        #endregion
    }
}
