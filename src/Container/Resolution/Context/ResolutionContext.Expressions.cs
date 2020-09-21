using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Container;

namespace Unity.Resolution
{
    public partial struct ResolutionContext
    {
        #region Fields

        private static readonly TypeInfo _contextTypeInfo = typeof(ResolutionContext).GetTypeInfo();
        private static readonly Type _contextRefType = typeof(Pipeline).GetTypeInfo()
                                                                       .GetDeclaredMethod("Invoke")!
                                                                       .GetParameters()[0]
                                                                       .ParameterType;
        #endregion

        public static readonly LabelTarget ReturnTarget = Expression.Label(typeof(object));


        #region Public Properties

        public static readonly ParameterExpression ContextExpression =
            Expression.Parameter(_contextRefType, "context");

        public static readonly MemberExpression TypeExpression =
            Expression.MakeMemberAccess(ContextExpression,
                _contextTypeInfo.GetDeclaredProperty(nameof(Type))!);

        public static readonly MemberExpression NameExpression =
            Expression.MakeMemberAccess(ContextExpression,
                _contextTypeInfo.GetDeclaredProperty(nameof(Name))!);

        //public static readonly MemberExpression ContainerExpression =
        //    Expression.MakeMemberAccess(ContextExpression,
        //        _contextTypeInfo.GetDeclaredProperty(nameof(ResolutionContext.Container)));

        public static readonly MemberExpression ExistingExpression =
            Expression.MakeMemberAccess(ContextExpression,
                _contextTypeInfo.GetDeclaredProperty(nameof(Existing))!);

        #endregion

    }
}
