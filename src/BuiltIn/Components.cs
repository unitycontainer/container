using Unity.Extension;

namespace Unity.BuiltIn
{
    internal static class Components
    {
        public static void Setup(ExtensionContext context)
        {
            #region Factories

            LazyFactory.Setup(context);
            FuncFactory.Setup(context);

            #endregion
        }
    }
}
