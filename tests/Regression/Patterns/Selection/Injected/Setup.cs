using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Selection.Injected
{
    public abstract partial class Pattern : Selection.Pattern
    {
        #region Constants

        const string SELECTION    = "Selection";
        const string BY_COUNT     = "By Count";
        const string BY_TYPE      = "By Type";
        const string BY_VALUE     = "By Value";
        const string BY_PARAMETER = "By Parameter";

        #endregion


        #region Fields

        Type[] TypesForward = new[] { typeof(IUnityContainer), typeof(string), typeof(object), };
        Type[] TypesReverse = new[] { typeof(object), typeof(string), typeof(IUnityContainer) };
        

        #endregion
    }
}
