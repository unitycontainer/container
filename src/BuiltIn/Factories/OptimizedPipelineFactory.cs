using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class OptimizedPipelineFactory
    {
        #region Fields

        public static MethodInfo FactoryMethodInfo = typeof(OptimizedPipelineFactory).GetMethod(nameof(Factory))!;

        #endregion


        #region Scaffolding

        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;

            // Default activating pipelines
            policies.Set(typeof(Defaults.OptimizedPipelineFactory), FactoryMethodInfo.CreateDelegate(typeof(Defaults.OptimizedPipelineFactory), policies));
        }

        #endregion


        public static Pipeline Factory(Defaults defaults, ref ResolutionContext context)
        {
//            var builder = new PipelineBuilder(ref context);

//            var expressions = new List<Expression>();



//#if NET45 || NET46 || NET47 || NET48
//            var generator = DebugInfoGenerator.CreatePdbGenerator();
//            var document = Expression.SymbolDocument("debug.txt");
//            var addDebugInfo = Expression.DebugInfo(document, 6, 9, 6, 22);

//            expressions.Add(addDebugInfo);
//            expressions.Add(Expression.Label(ResolutionContext.ReturnTarget, ResolutionContext.ExistingExpression));

//            var lambda = Expression.Lambda<Pipeline>(Expression.Block(expressions), ResolutionContext.ContextExpression);

//            return lambda.Compile(generator);
//#else
//            expressions.Add(Expression.Label(ResolutionContext.ReturnTarget, ResolutionContext.ExistingExpression));
//            var lambda = Expression.Lambda<Pipeline>(Expression.Block(expressions), ResolutionContext.ContextExpression);
//            return lambda.Compile();
//#endif

            return (ref ResolutionContext c) => { };
        }
    }
}
