using Unity.Extension;

namespace Unity.BuiltIn
{
    internal static class Components
    {
        public static void Setup(ExtensionContext context)
        {
            #region Factories

            PipelineFactory.Setup(context);
            LazyFactory.Setup(context);
            FuncFactory.Setup(context);

            #endregion
        }
    }
}
